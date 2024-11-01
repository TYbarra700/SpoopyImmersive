using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAudioController : MonoBehaviour
{
    private AudioSource audioSource;
    private bool isPickedUp = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on this object!");
        }
    }

    public void PickUp()
    {
        isPickedUp = true;
        audioSource.Stop();
    }

    public void Drop()
    {
        isPickedUp = false;
        audioSource.Play();
    }

    void Update()
    {
        if (!isPickedUp && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
