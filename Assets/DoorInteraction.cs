using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class DoorInteraction : MonoBehaviour
{
    public Animator doorAnimator;
    private bool isOpen = false;

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            doorAnimator.SetTrigger("OpenDoor");
        }
    }
}
