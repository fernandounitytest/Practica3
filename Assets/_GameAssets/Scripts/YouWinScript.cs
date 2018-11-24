using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class YouWinScript : MonoBehaviour {
    static string INSERTCOIN = "insertcoin";
    AudioSource audioSource;
    public Animator cam3Animator;
    public Text instructions;
    private int timeToReload = 20;
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
            InvokeRepeating("CountDown", 5, 1);
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

    private void CountDown()
    {
        timeToReload--;
        if (timeToReload==0)
        {
            SceneManager.LoadScene(0);
        }
        instructions.enabled = true;
        instructions.text = timeToReload.ToString();
    }
}
