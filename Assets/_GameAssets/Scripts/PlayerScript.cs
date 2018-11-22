using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour {
    public enum Estado { Idle, Walk, Jump, Run, Shoot, Tired, Dead, InLove };

    [Header("Environment")]
    public GameObject targetCircle;
    public LayerMask floorLayerMask;
    public LayerMask enemyLayerMask;

    [Header("Behavior")]
    public int speed;
    public int followDistance;
    public int rotationSpeed;

    [Header("ReferencePoints")]
    public Transform posPies;

    [Header("Distraccion")]
    public Transform puntoGeneracionDistraccion;
    public GameObject prefabDistraccion;
    public int fuerzaLanzamientoDistraccion;

    [Header("Cameras")]
    public Camera gameOverCamera;

    [Header("UI")]
    public GameObject textGameOver;
    public GameObject textToReload;

    [Header("Audios")]
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip shotSound;

    //ANIMATOR CONSTANTS
    const string ANIM_WALKING = "andando";
    const string ANIM_JUMPING = "saltando";
    const string ANIM_RUNNING = "corriendo";
    const string SHOOT_TRIGGER = "disparando";
    const string ANIM_WALKING_ENEMY = "walking";
    const int WALK_SPEED = 2;
    const int RUN_SPEED = 5;


    AudioSource audioSource;
    NavMeshAgent agent;
    Animator animator;
    Vector3 prevPosition;//Posición del último frame
    Estado estado = Estado.Idle;//Estado del player
    float floorDetectorRadius = 1f;//Radio de deteccion del suelo
    int timeToReload = 5;//Tiempo para recargar la escena
    const int ID_FLOOR_LAYER = 8;
    const int ID_ENEMY_LAYER = 9;
    const int SHOOT_DISTANCE = 10; 

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update ()
    {
        if (estado == Estado.Dead)
        {
            return;
        }

        EvaluarGeneracionDistraccion();

        if (Input.GetButtonDown("Fire1"))
        {
            ManageMouseClick();
        }
        else if (Input.touchCount > 0)
        {
            ManageTouchTap();
        }

        switch (estado)
        {
            case Estado.Walk:
                if (!agent.pathPending)
                {
                    CheckTarget();
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        estado = Estado.Run;
                        animator.SetBool(ANIM_RUNNING, true);
                        audioSource.Stop();
                        audioSource.loop = true;
                        audioSource.PlayOneShot(runSound);
                        agent.speed = RUN_SPEED;
                    }
                }
                CheckJump();
                break;
            case Estado.Jump:
                CheckEndJump();
                break;
            case Estado.Run:
                if (!agent.pathPending)
                {
                    CheckTarget();
                    if (Input.GetKeyUp(KeyCode.R))
                    {
                        estado = Estado.Walk;
                        animator.SetBool(ANIM_RUNNING, false);
                        agent.speed = WALK_SPEED;
                        audioSource.Stop();
                        audioSource.loop = true;
                        audioSource.PlayOneShot(walkSound);
                    }
                }
                CheckJump();
                break;
        }

        //Vector3 forwardXZ = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        //transform.rotation = Quaternion.LookRotation(forwardXZ);
    }

    private void EvaluarGeneracionDistraccion()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            GenerarDistraccion();
        }
    }

    private void GenerarDistraccion()
    {
        GameObject nuevoPetardo = Instantiate(
                        prefabDistraccion,
                        puntoGeneracionDistraccion.position,
                        puntoGeneracionDistraccion.rotation);
        nuevoPetardo.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * fuerzaLanzamientoDistraccion);
    }

    public Estado GetEstado()
    {
        return this.estado;
    }

    private void ManageMouseClick()
    {
        Ray ray = CamerasManager.activeCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit rch;
        bool hasTouch = Physics.Raycast(ray, out rch);
        if (hasTouch && rch.transform.gameObject.CompareTag("Vigilante"))
        {
            switch (estado)
            {
                case Estado.Idle:
                    ShootEnemy(rch);
                    break;
            }
        }
        else
        {
            hasTouch = Physics.Raycast(ray, out rch, Mathf.Infinity, floorLayerMask);
            if (hasTouch)
            {
                switch (estado)
                {
                    case Estado.Idle:
                        StartWalk(rch);
                        break;
                }
            }
        }
    }
        
    private void ManageTouchTap()
    {
        Ray ray = CamerasManager.activeCamera.ScreenPointToRay(Input.touches[0].position);
        RaycastHit rch;
        bool hasTouch = Physics.Raycast(ray, out rch, Mathf.Infinity);
        if (hasTouch && rch.transform.gameObject.layer == ID_FLOOR_LAYER)
        {
            switch (estado)
            {
                case Estado.Idle:
                    StartWalk(rch);
                    break;
            }
        }
    }

    private void StartWalk(RaycastHit rch)
    {
        estado = Estado.Walk;
        targetCircle.transform.position = rch.point;
        targetCircle.transform.rotation = Quaternion.LookRotation(Vector3.up);
        agent.destination = targetCircle.transform.position;
        animator.SetBool(ANIM_WALKING, true);
        audioSource.Stop();
        audioSource.loop = true;
        audioSource.PlayOneShot(walkSound);
    }

    private void CheckTarget()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            estado = Estado.Idle;
            animator.SetBool(ANIM_RUNNING, false);
            animator.SetBool(ANIM_WALKING, false);
            audioSource.Stop();
            audioSource.loop = false;
            
        }
    }

    private void CheckJump()
    {
        Collider[] col = Physics.OverlapSphere(posPies.position, floorDetectorRadius, floorLayerMask);
        if (col.Length == 0)
        {
            estado = Estado.Jump;
            animator.SetBool(ANIM_JUMPING, true);
        } 
    }

    private void CheckEndJump()
    {
        Collider[] col = Physics.OverlapSphere(posPies.position, floorDetectorRadius, floorLayerMask);
        if (col.Length > 0)
        {
            estado = Estado.Walk;
            animator.SetBool(ANIM_JUMPING, false);
        }
    }

    public void Kill()
    {
        if (this.estado != Estado.Dead)
        {
            DesactivarVigilantes();
            gameOverCamera.transform.position = CamerasManager.activeCamera.transform.position;
            gameOverCamera.transform.rotation = CamerasManager.activeCamera.transform.rotation;
            gameOverCamera.GetComponent<Camera>().fieldOfView = CamerasManager.activeCamera.GetComponent<Camera>().fieldOfView;
            gameOverCamera.gameObject.SetActive(true);
            CamerasManager.activeCamera.gameObject.SetActive(false);
            audioSource.Stop();
            animator.SetTrigger("muerto");
            this.estado = Estado.Dead;
            textGameOver.SetActive(true);
            textToReload.SetActive(true);
            agent.isStopped = true;
            InvokeRepeating("CountDownToReload", 1, 1);
        }
    }

    private void DesactivarVigilantes()
    {
        GameObject[] vigilantes = GameObject.FindGameObjectsWithTag("Vigilante");
        foreach (GameObject vigilante in vigilantes)
        {
            vigilante.GetComponent<VigilanteScript>().enabled = false;
            vigilante.GetComponent<Animator>().SetBool(ANIM_WALKING_ENEMY, false);
            vigilante.GetComponent<NavMeshAgent>().enabled = false;
        }
    }

    private void CountDownToReload()
    {
        timeToReload--;
        textToReload.GetComponent<Text>().text = timeToReload.ToString();
        if (timeToReload == 0)
        {
            SceneManager.LoadScene(0);
        }
    }

    private void ShootEnemy(RaycastHit rch)
    {
        transform.LookAt(rch.transform.position);
        animator.SetTrigger(SHOOT_TRIGGER);
        Invoke("PlayShootSound", 1);
        Ray ray = new Ray(transform.position, (rch.transform.position - transform.position));
        //Debug.DrawRay(transform.position, (rch.transform.position - transform.position), Color.red, 5);
        bool impacto = Physics.Raycast(ray, out rch, SHOOT_DISTANCE);
        if (impacto && rch.transform.gameObject.CompareTag("Vigilante"))
        {
            rch.transform.gameObject.GetComponent<VigilanteScript>().Kill();
        }
    }

    public void PlayShootSound()
    {
        audioSource.loop = false;
        audioSource.PlayOneShot(shotSound);
    }

}
