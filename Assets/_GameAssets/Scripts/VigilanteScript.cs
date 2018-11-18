using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VigilanteScript : MonoBehaviour {

    //Estados del vigilante
    enum State {Idle, Walking, Obsessed };
    private State state;

    private Vector3 target;

    [Header("Puntos de patrulla")]
    public Transform[] targets = new Transform[4];
    
    [Header("Cameras")]
    public CamerasManager cm;


    private NavMeshAgent agent;
    private Animator animator;
    private int MIN_TIME_TO_NEW_TARGET = 3;
    private int MAX_TIME_TO_NEW_TARGET = 5;
    private Transform player;
    private float detectionDistance = 10f; //Distancia de detección
    private float detectionDegrees = 25f; //Grados de vision

    void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        SetNewTarget();
        player = GameObject.Find("Paniagua").transform;
    }

    void Update()
    {
        CheckPlayer();

        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Walking:
                CheckTargets();
                break;
        }
    }

    private void CheckPlayer()
    {
        float vDistanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        Vector3 direccion = Vector3.Normalize(player.transform.position - transform.position);
        float angle = Vector3.Angle(direccion, transform.forward);
        if (vDistanceToPlayer < detectionDistance && angle < detectionDegrees)
        {
            Debug.DrawLine(transform.position, player.transform.position, Color.red, 1);
            RaycastHit rch;
            if (Physics.Raycast(
                transform.position,
                direccion,
                out rch,
                Mathf.Infinity))
            {
                if (rch.transform.gameObject.name == "Paniagua")
                {
                    player.GetComponent<PlayerScript>().Kill();
                }
            }
        }
    }


    public void SetExternalTarget(Vector3 targetPosition)
    {
        if (state != State.Walking)
        {
            state = State.Obsessed;
            animator.SetBool("walking", true);
        }
        target = targetPosition;
        agent.destination = target;
    }

    private void SetNewTarget()
    {
        //Cuando está distraido por un ruido o evento, abandona la patrualla
        if (state != State.Obsessed)
        {
            target = targets[Random.Range(0, targets.Length)].position;
            agent.destination = target;
            state = State.Walking;
            animator.SetBool("walking", true);
        }
    }

    private void Idle()
    {
   
    }
    private void CheckTargets()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            state = State.Idle;
            animator.SetBool("walking", false);
            Invoke("SetNewTarget", Random.Range(MIN_TIME_TO_NEW_TARGET, MAX_TIME_TO_NEW_TARGET));
        }
    }

    
    

}
