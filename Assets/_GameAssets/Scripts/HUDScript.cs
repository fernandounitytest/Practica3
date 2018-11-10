using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour {

    [Header("Referencias")]
    public NavMeshAgent playerNMA;//Agente de navegación del Player
    public Text dttText; //Distance To Target Text
    // Update is called once per frame
    void Update () {
        dttText.text = "DTT:" + playerNMA.remainingDistance;
        
		
	}
}
