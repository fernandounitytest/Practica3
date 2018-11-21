using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class YouWinScript : MonoBehaviour {
    static string INSERTCOIN = "insertcoin";
    AudioSource audioSource;
    public Animator cam3Animator;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Paniagua")
        {
            DesactivarVigilantes();
            audioSource.Play();
            cam3Animator.SetBool(INSERTCOIN, true);
        }
    }

    private void DesactivarVigilantes()
    {
        GameObject[] vigilantes = GameObject.FindGameObjectsWithTag("Vigilante");
        foreach (GameObject vigilante in vigilantes)
        {
            vigilante.GetComponent<VigilanteScript>().enabled = false;
            vigilante.GetComponent<Animator>().SetBool("walking", false);
            vigilante.GetComponent<NavMeshAgent>().enabled = false;
        }
    }
}
