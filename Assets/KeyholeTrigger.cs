using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyholeTrigger : MonoBehaviour
{
    public DoorInteraction door;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            door.OpenDoor();
            Destroy(other.gameObject);
        }
    }
}
