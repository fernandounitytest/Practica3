using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key1Script : MonoBehaviour {
    public Animator animatorPuerta1;
    public Animator animatorPuerta2;
    public GameObject prefabParticles;
    public AudioClip openDoorSound;
    public AudioSource audioSource;
    public VigilanteScript vigilante;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Paniagua")
        {
            Instantiate(prefabParticles, transform.position, transform.rotation);
            animatorPuerta1.SetBool("open", true);
            animatorPuerta2.SetBool("open", true);
            SetEnemyTarget();
            audioSource.PlayOneShot(openDoorSound);
            Destroy(gameObject);
        }
    }

    private void SetEnemyTarget()
    {
        vigilante.SetExternalTarget(this.transform.position);
    }

    

}
