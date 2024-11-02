using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MovementAudio : MonoBehaviour
{
    public ContinuousMoveProviderBase moveProvider;
    public AudioSource movementAudioSource;
    public float audioInterval = 1f; // Interval in seconds between audio clips

    private float timer = 0f;
    private bool isMoving = false;

    void Update()
    {
        if (moveProvider == null || movementAudioSource == null)
            return;

        // Check if the player is moving by checking the character controller's velocity
        var characterController = moveProvider.GetComponent<CharacterController>();

        if (characterController != null && characterController.velocity.magnitude > 0.1f)
        {
            if (!isMoving)
            {
                isMoving = true;
                timer = audioInterval; // Reset timer when movement starts
            }

            // Play sound at intervals
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                movementAudioSource.Play();
                timer = audioInterval;
            }
        }
        else
        {
            if (isMoving)
            {
                // Stop playing audio after the last clip finishes
                isMoving = false;
            }
        }
    }
}
