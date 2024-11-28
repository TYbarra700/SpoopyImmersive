using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TV : MonoBehaviour
{
    [SerializeField] private JumpscareManager jumpscareManager;
    [SerializeField] private Collider jumpscareTriggerZone;
    public int countNumJumpscared;

    void Start()
    {
        countNumJumpscared = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (countNumJumpscared < 2 && jumpscareTriggerZone != null && other.CompareTag("Player"))
        {
            jumpscareManager.TriggerJumpscare(2); // Trigger crawling zombie jumpscare
        }

        //Debug.Log("Collided with" + other.name);
    }
}
