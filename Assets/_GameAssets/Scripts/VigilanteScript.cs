using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VigilanteScript : MonoBehaviour {

    //Estados del vigilante
    enum State {Idle, Walking };
    private State state;


    private Transform target;
    public Transform[] targets = new Transform[4];

    NavMeshAgent agent;
    private Animator animator;
    private int MIN_TIME_TO_NEW_TARGET = 3;
    private int MAX_TIME_TO_NEW_TARGET = 5;

    void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        SetNewTarget();
    }

    private void SetNewTarget()
    {
        target = targets[Random.Range(0, targets.Length)];
        state = State.Walking;
        agent.destination = target.position;
        animator.SetBool("walking", true);

    }

    void Update () {
        switch(state) {
            case State.Idle:
                Idle();
                break;
            case State.Walking:
                CheckTargets();
                break;
        }
    }

    private void Idle()
    {
   
    }
    private void CheckTargets()
    {
        print("Checkingtargets");
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            state = State.Idle;
            animator.SetBool("walking", false);
            Invoke("SetNewTarget", Random.Range(MIN_TIME_TO_NEW_TARGET, MAX_TIME_TO_NEW_TARGET));
        }
    }

    
    

}
