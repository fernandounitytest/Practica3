using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasoRoom1ToRoom2Script : MonoBehaviour {
    int room = 1;
    public Camera c1;
    public Camera c2;
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name=="Paniagua" && room == 1)
        {
            c1.gameObject.SetActive(false);
            c2.gameObject.SetActive(true);
            CamerasManager.activeCamera = c2;
            room = 2;
        }
    }

}
