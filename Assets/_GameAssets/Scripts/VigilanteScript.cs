using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VigilanteScript : MonoBehaviour {

    //Estados del vigilante
    enum State {Idle, Walking, Death };
    private bool obsessed = false;
    private State state;

    //ANIMATOR
    private Animator animator;
    private const string ESTADO_WALKING = "walking";
    private const string ESTADO_DEAD = "death";

    private Vector3 target;

    [Header("Puntos de patrulla")]
    public Transform[] targets = new Transform[4];
    
    [Header("Cameras")]
    public CamerasManager cm;


    private NavMeshAgent agent;
    private int MIN_TIME_TO_NEW_TARGET = 3;
    private int MAX_TIME_TO_NEW_TARGET = 5;
    private Transform playerTransform;
    private PlayerScript playerScript;
    private float detectionDistance = 10f; //Distancia de detección
    private float detectionDegrees = 25f; //Grados de vision

    void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        SetNewTarget();
        playerTransform = GameObject.Find("Paniagua").transform;
        playerScript = GameObject.Find("Paniagua").GetComponent<PlayerScript>();
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
        CheckPlayer();
        EvaluarRuido();
    }

    private void CheckPlayer()
    {
        float vDistanceToPlayer = Vector3.Distance(transform.position, playerTransform.transform.position);
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

    public void SetExternalTarget(Vector3 targetPosition)
    {
        print("setting external target");
        CancelInvoke();
        if (state != State.Walking)
        {
            animator.SetBool(ESTADO_WALKING, true);
            state = State.Walking;
        }
        target = targetPosition;
        agent.destination = target;
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
                    agent.destination = target;
                } while (Vector3.Distance(this.transform.position, target) < agent.stoppingDistance*2);
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
            obsessed = false;
            Invoke("SetNewTarget", Random.Range(MIN_TIME_TO_NEW_TARGET, MAX_TIME_TO_NEW_TARGET));
        }
    }

    private void Idle()
    {

    }

    public void Kill()
    {
        state = State.Death;
        animator.SetBool(ESTADO_DEAD, true);
    }

    private void EvaluarRuido()
    {
        switch(playerScript.GetEstado()){
            case PlayerScript.Estado.Walk:
                print("DESDE VIGILANTE, PLAYER ANDANDO");
                break;
            case PlayerScript.Estado.Run:
                print("DESDE VIGILANTE, PLAYER CORRIENDO");
                break;
        }
    }
}
