using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TV : MonoBehaviour
{
    [SerializeField] private JumpscareManager jumpscareManager;
    [SerializeField] private Collider jumpscareTriggerZone;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (jumpscareTriggerZone != null && other.CompareTag("Player"))
        {
            jumpscareManager.TriggerJumpscare(2); // Trigger crawling zombie jumpscare
            Debug.Log("Crawling zombie jumpscare triggered.");
        }

        Debug.Log("Collided with" + other.name);
    }
}
