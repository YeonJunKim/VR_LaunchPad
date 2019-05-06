using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    AudioSource audioSource;


    private void Awake()
    {
        if (AudioManager.instance == null)
        {
            AudioManager.instance = this;
        }

        audioSource = transform.GetComponent<AudioSource>();
    }


    public void PlayAudio()
    {
        CancelInvoke("StopAudio");
        audioSource.time = 0;
        audioSource.Play();
    }

    public void PlayAudioAtPoint(float time)
    {
        CancelInvoke("StopAudio");
        audioSource.time = time;
        audioSource.Play();
    }

    public void SetStopTimer(float time)
    {
        CancelInvoke("StopAudio");
        Invoke("StopAudio", time);
    }

    public void StopAudio()
    {
        audioSource.Stop();
        audioSource.time = 0;
    }


    public float GetCurrentPlayTime()
    {
        return audioSource.time;
    }


    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }

    public void SetAudioClip(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
    }

    public AudioClip GetAudioClip()
    {
        return audioSource.clip;
    }


    public void AudioFastForward(bool forward)
    {
        if(audioSource.isPlaying)
        {
            float time = audioSource.time;

            if (forward)
                time += 10;
            else
                time -= 10;


            if (time < 0)
                time = 0;
            else if (time > audioSource.clip.length)
            {
                time = audioSource.clip.length - 0.1f;
            }

            audioSource.time = time;
        }

        // when you reached the end, and the audio Stopped
        // you can go backwords, and play the audio automatically
        else
        {
            if (!forward)
            {
                audioSource.time = audioSource.clip.length - 10;

                audioSource.Play();
            }
        }
    }
}
