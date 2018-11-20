using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        audioSource.Play();
        cam3Animator.SetBool(INSERTCOIN, true);
    }
}
