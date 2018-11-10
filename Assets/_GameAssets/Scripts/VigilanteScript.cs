using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VigilanteScript : MonoBehaviour {

    //Estados del vigilante
    enum State {Idle, TurningRight, TurningLeft, Walking };
    private State state;


    public Transform target;
    public Transform[] targets = new Transform[4];

    NavMeshAgent agent;
    private Animator animator;

    void Start () {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
	}
	
	void Update () {
        switch(state) {
            case State.Idle:
                Idle();
                break;
            case State.TurningRight:
                RotateNextTarget();
                break;
        }
    }

    private void Idle()
    {
        agent.isStopped = true;
        Invoke("ChangeState", 6);
    }
    
    private void ChangeState()
    {
        state = State.TurningRight;
        animator.SetBool("turningRight", true);
    }
    private void RotateNextTarget()
    {
        transform.Rotate(0, 1, 0);
    }

}
