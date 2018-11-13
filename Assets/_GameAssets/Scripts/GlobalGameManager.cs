using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameManager : MonoBehaviour {

    public static Camera activeCamera;
    private void Start()
    {
        activeCamera = Camera.main;
    }
}
