using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickSound : MonoBehaviour {

    AudioSource audio;


    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Stick")
        {
            audio.Stop();
            audio.PlayOneShot(audio.clip);
        }
    }
}
