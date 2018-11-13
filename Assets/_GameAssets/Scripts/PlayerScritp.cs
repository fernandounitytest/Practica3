using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerScritp : MonoBehaviour {
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

    
    //ANIMATOR CONSTANTS
    const string ANIM_WALKING = "andando";
    const string ANIM_JUMPING = "saltando";


    NavMeshAgent agent;
    Animator animator;
    Vector3 prevPosition;//Posición del último frame
    float xPos;//Posicion del ratón
    Estado estado = Estado.Idle;//Estado del player
    float floorDetectorRadius = 1f;//Radio de deteccion del suelo

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
        } else if (Input.touchCount > 0)
        {
            ManageTouchTap();
        }

        switch (estado) {
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

    private void ManageMouseClick()
    {
        Ray ray = GlobalGameManager.activeCamera.ScreenPointToRay(Input.mousePosition);
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
        Ray ray = GlobalGameManager.activeCamera.ScreenPointToRay(Input.touches[0].position);
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
}
