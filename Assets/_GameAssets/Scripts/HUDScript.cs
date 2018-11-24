using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour {
    public Text instruction;
    public string[] mensajes = {
        "Haz click con el ratón para andar",
        "Mantén pulsada la tecla R para correr",
        "Pulsa la tecla V para lanzar señuelo" };
    int i = 0;
    private void Start()
    {
        Invoke("MuestraMensaje", 1);
    }
    private void MuestraMensaje()
    {
        if (i < mensajes.Length)
        {
            instruction.text = mensajes[i];
            i++;
            Invoke("MuestraMensaje", 5);
        }
        else
        {
            instruction.enabled = false;
        }
    }
}
