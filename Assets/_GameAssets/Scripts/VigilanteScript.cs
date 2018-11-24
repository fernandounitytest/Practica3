using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class VigilanteScript : MonoBehaviour {

    //Estados del vigilante
    enum State {Idle, Walking, Death };
    private bool obsessed = false;
    private State state;

    //ANIMATOR
    private Animator animator;
    private const string ESTADO_WALKING = "walking";
    private const string ESTADO_DEAD = "death";

    //UI
    public Text textAdvertencia;
    private string textoInicial;

    private Vector3 target;

    [Header("Puntos de patrulla")]
    public Transform[] targets = new Transform[8];
    
    [Header("Cameras")]
    public CamerasManager cm;


    private NavMeshAgent agent;
    private int MIN_TIME_TO_NEW_TARGET = 3;
    private int MAX_TIME_TO_NEW_TARGET = 5;
    private Transform playerTransform;
    private PlayerScript playerScript;
    private float detectionDistance = 10f; //Distancia de detección
    private float detectionDegrees = 25f; //Grados de vision
    private float walkingDetectionDistance = 5f;
    private float runningDetectionDistance = 10f;

    void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        playerTransform = GameObject.Find("Paniagua").transform;
        playerScript = GameObject.Find("Paniagua").GetComponent<PlayerScript>();
        textoInicial = textAdvertencia.text;
        SetNewTarget();
    }

    void Update()
    {
        switch (state)
        {
            case State.Death:
                agent.enabled = false;
                return;
            case State.Idle:
                //Idle();
                break;
            case State.Walking:
                CheckTargets();
                break;
        }
        if (state == State.Death)
        {
            return;
        }
        CheckPlayer();
        EvaluarRuido();
    }

    private void CheckPlayer()
    {
        float vDistanceToPlayer = GetDistanceToPlayer();
        Vector3 direccion = Vector3.Normalize(playerTransform.transform.position - transform.position);
        float angle = Vector3.Angle(direccion, transform.forward);
        if (vDistanceToPlayer < detectionDistance && angle < detectionDegrees)
        {
            //Debug.DrawLine(transform.position, player.transform.position, Color.red, 1);
            RaycastHit rch;
            if (Physics.Raycast(
                transform.position,
                direccion,
                out rch,
                Mathf.Infinity))
            {
                if (rch.transform.gameObject.name == "Paniagua")
                {
                    playerTransform.GetComponent<PlayerScript>().Kill();
                }
            }
        }
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, playerTransform.transform.position);
    }

    public void SetExternalTarget(Vector3 targetPosition)
    {
        CancelInvoke();
        target = targetPosition;
        agent.isStopped = false;
        agent.destination = target;
        animator.SetBool(ESTADO_WALKING, true);
        state = State.Walking;
        obsessed = true;
    }

    private void SetNewTarget()
    {
        //Cuando está distraido por un ruido o evento, abandona la patrulla
        if (!obsessed)
        {
            if (agent.isActiveAndEnabled == true)
            {
                do
                {
                    target = targets[Random.Range(0, targets.Length)].position;
                } while (Vector3.Distance(this.transform.position, target) < (agent.stoppingDistance*1.2f));
                agent.isStopped = false;
                agent.destination = target;
                state = State.Walking;
                animator.SetBool(ESTADO_WALKING, true);
            }
        }
    }

    private void CheckTargets()
    {
        if (agent.isActiveAndEnabled==true && agent.remainingDistance <= agent.stoppingDistance)
        {
            state = State.Idle;
            animator.SetBool(ESTADO_WALKING, false);
            agent.isStopped = true;
            obsessed = false;
            Invoke("SetNewTarget", Random.Range(MIN_TIME_TO_NEW_TARGET, MAX_TIME_TO_NEW_TARGET));
        }
    }

    private void Idle()
    {

    }

    public void Kill()
    {
        textAdvertencia.text = textoInicial + "eliminado";
        state = State.Death;
        animator.SetBool(ESTADO_DEAD, true);
    }

    private void EvaluarRuido()
    {
        bool teEscucha = false;
        switch(playerScript.GetEstado()){
            case PlayerScript.Estado.Walk:
                if (GetDistanceToPlayer() < walkingDetectionDistance)
                {
                    teEscucha = true;
                    textAdvertencia.text = textoInicial + "te oye andar";
                    SetExternalTarget(playerTransform.position);
                }
                break;
            case PlayerScript.Estado.Run:
                if (GetDistanceToPlayer() < runningDetectionDistance)
                {
                    teEscucha = true;
                    textAdvertencia.text = textoInicial + "te oye correr";
                    SetExternalTarget(playerTransform.position);
                }
                break;
        }
        if (!teEscucha)
        {
            textAdvertencia.text = textoInicial + "patrullando";
        }
    }
}
