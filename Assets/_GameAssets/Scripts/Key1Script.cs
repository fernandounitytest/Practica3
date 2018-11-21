using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key1Script : MonoBehaviour {
    public Animator animatorPuerta1;
    public Animator animatorPuerta2;
    public AudioClip openDoorSound;
    public AudioSource audioSource;
    public VigilanteScript vigilante;
    public GameObject prefabParticles;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Paniagua")
        {
            Instantiate(prefabParticles, transform.position, transform.rotation);
            animatorPuerta1.SetBool("open", true);
            animatorPuerta2.SetBool("open", true);
            audioSource.PlayOneShot(openDoorSound);
            Destroy(gameObject);
        }
    }
}
