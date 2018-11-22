using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour {
    public Animator animatorPuerta1;
    public Animator animatorPuerta2;
    public AudioClip openDoorSound;
    public AudioSource audioSource;
    public GameObject prefabParticles;
    private const float DELAY_SET_TARGET = 0.25f;

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
