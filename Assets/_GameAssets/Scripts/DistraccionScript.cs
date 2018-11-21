using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistraccionScript : MonoBehaviour
{

    public int timeToDestroy = 3;
    public int radioDistraccion = 10;
    public GameObject prefabParticles;

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(prefabParticles, transform.position, transform.rotation);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radioDistraccion);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.CompareTag("Vigilante"))
            {
                hitColliders[i].gameObject.GetComponent<VigilanteScript>().SetExternalTarget(this.transform.position);
            }
        }
        Destroy(this.gameObject);
    }
}
