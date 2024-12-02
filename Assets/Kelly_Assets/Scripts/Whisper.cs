using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whisper : MonoBehaviour
{
    public AudioSource whisperAudioSource; // Reference to the AudioSource component

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            if (whisperAudioSource != null && !whisperAudioSource.isPlaying)
            {
                whisperAudioSource.Play();
            }
        }
    }
}
