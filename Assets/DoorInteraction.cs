using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public Animator doorAnimator;
    public AudioSource doorAudioSource; // Reference to the AudioSource component
    private bool isOpen = false;

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            doorAnimator.SetTrigger("OpenDoor");

            // Play the sound when the door opens
            if (doorAudioSource != null)
            {
                doorAudioSource.Play();
            }
            else
            {
                Debug.LogWarning("No AudioSource assigned to doorAudioSource.");
            }
        }
    }
}
