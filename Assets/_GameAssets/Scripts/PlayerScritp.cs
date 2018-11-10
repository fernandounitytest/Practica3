using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerScritp : MonoBehaviour {
    enum Estado { Idle, Walk, Run, Attack, Tired, Dead, InLove };

    [Header("Entorno")]
    public GameObject targetCircle;

    [Header("Comportamiento")]
    public int speed;
    public int followDistance;
    public int rotationSpeed;

    
    //ANIMATOR CONSTANTS
    const string ANIM_WALKING = "andando";
    

    NavMeshAgent agent;
    Animator animator;
    Vector3 prevPosition;//Posición del último frame
    float xPos;//Posicion del ratón
    Estado estado = Estado.Idle;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update () {
        xPos = Input.GetAxis("Mouse X");
        if (Input.GetButtonDown("Fire1"))
        {
            ManageMouseClick();
        }
        switch (estado) {
            case Estado.Walk:
                if (!agent.pathPending)
                {
                    CheckTarget();
                }
                break;
        }
       
        //Vector3 forwardXZ = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        //transform.rotation = Quaternion.LookRotation(forwardXZ);
    }

    private void ManageMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rch;
        bool hasTouch = Physics.Raycast(ray, out rch);
        switch (estado)
        {
            case Estado.Idle:
                StartWalk(rch);
                break;

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
}
