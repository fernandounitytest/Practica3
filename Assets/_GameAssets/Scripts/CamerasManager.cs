using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerasManager : MonoBehaviour {

    public static Camera activeCamera;
    public static Camera gameOverCamera;
    private void Start()
    {
        activeCamera = Camera.main;
    }
}
