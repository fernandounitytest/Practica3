using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Vigilante : MonoBehaviour {
    public Transform target;
    NavMeshAgent agente;
	
	void Start () {
        agente = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        agente.destination = target.position;
    }
}
