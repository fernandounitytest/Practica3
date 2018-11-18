using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour {
    enum Estado { Idle, Walk, Jump, Run, Attack, Tired, Dead, InLove };

    [Header("Environment")]
    public GameObject targetCircle;
    public LayerMask layerSuelo;


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

    //ANIMATOR CONSTANTS
    const string ANIM_WALKING = "andando";
    const string ANIM_JUMPING = "saltando";


    NavMeshAgent agent;
    Animator animator;
    Vector3 prevPosition;//Posición del último frame
    Estado estado = Estado.Idle;//Estado del player
    float floorDetectorRadius = 1f;//Radio de deteccion del suelo
    int timeToReload = 5;//Tiempo para recargar la escena

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
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
                }
                CheckJump();
                break;
            case Estado.Jump:
                CheckEndJump();
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
    

    private void ManageMouseClick()
    {
        print(CamerasManager.activeCamera);
        Ray ray = CamerasManager.activeCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit rch;
        bool hasTouch = Physics.Raycast(ray, out rch, Mathf.Infinity, layerSuelo);
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

    private void ManageTouchTap()
    {
        Ray ray = CamerasManager.activeCamera.ScreenPointToRay(Input.touches[0].position);
        RaycastHit rch;
        bool hasTouch = Physics.Raycast(ray, out rch, Mathf.Infinity, layerSuelo);
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

    private void StartWalk(RaycastHit rch)
    {
        estado = Estado.Walk;
        targetCircle.transform.position = rch.point;
        targetCircle.transform.rotation = Quaternion.LookRotation(rch.normal);
        agent.destination = targetCircle.transform.position;
        animator.SetBool(ANIM_WALKING, true);
    }

    private void CheckTarget()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            estado = Estado.Idle;
            animator.SetBool(ANIM_WALKING, false);
        }
    }

    private void CheckJump()
    {
        Collider[] col = Physics.OverlapSphere(posPies.position, floorDetectorRadius, layerSuelo);
        if (col.Length == 0)
        {
            estado = Estado.Jump;
            animator.SetBool(ANIM_JUMPING, true);
        } 
    }

    private void CheckEndJump()
    {
        Collider[] col = Physics.OverlapSphere(posPies.position, floorDetectorRadius, layerSuelo);
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
            gameOverCamera.transform.position = CamerasManager.activeCamera.transform.position;
            gameOverCamera.transform.rotation = CamerasManager.activeCamera.transform.rotation;
            gameOverCamera.GetComponent<Camera>().fieldOfView = CamerasManager.activeCamera.GetComponent<Camera>().fieldOfView;
            gameOverCamera.gameObject.SetActive(true);
            CamerasManager.activeCamera.gameObject.SetActive(false);
            animator.SetTrigger("muerto");
            this.estado = Estado.Dead;
            textGameOver.SetActive(true);
            textToReload.SetActive(true);
            InvokeRepeating("CountDownToReload", 1, 1);
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

}
