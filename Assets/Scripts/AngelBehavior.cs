using UnityEngine;

public class AngelBehavior : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float activationRange = 10f; // Distance before angel activates
    public float moveSpeed = 1f; // Initial movement speed
    public float speedIncreaseRate = 0.1f; // Rate at which speed increases
    public AudioSource moveSound; // Sound when angel moves
    public AudioSource caughtSound; // Sound when angel catches the player
    public Vector3 resetPosition; // Position to reset the player when caught

    private bool isActivated = false; // Tracks if the angel is within range to activate
    private bool isLit = false; // Tracks if the angel is in the flashlight’s light

    void Update()
    {
        if (!isActivated)
        {
            CheckActivationRange(); // Check if player is close enough to activate the angel
        }

        if (isActivated && !isLit)
        {
            RotateAngelTowardPlayer(); // Rotate the angel toward the player
            MoveTowardPlayer(); // Move the angel toward the player
        }
        else
        {
            StopMovementSound(); // Stop sound if lit or not activated
        }
    }

    private void CheckActivationRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= activationRange)
        {
            isActivated = true;
            Debug.Log("Angel activated! Player is within range.");
        }
    }

    private void MoveTowardPlayer()
    {
        // Move toward the player
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Gradually increase speed
        moveSpeed += speedIncreaseRate * Time.deltaTime;

        // Play movement sound if not already playing
        if (!moveSound.isPlaying)
        {
            moveSound.Play();
        }
    }

    private void StopMovementSound()
    {
        if (moveSound.isPlaying)
        {
            moveSound.Stop();
        }
    }

    private void RotateAngelTowardPlayer()
    {
        if (player != null)
        {
            // Calculate the direction to the player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // Calculate the target rotation and apply a 90-degree correction
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
            targetRotation *= Quaternion.Euler(0, -90, 0); // Adjust for model's orientation

            // Apply the target rotation only to the Y-axis
            transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LightCone")) // Assuming the flashlight cone is tagged as "LightCone"
        {
            Debug.Log("Angel is in the flashlight's light!");
            isLit = true; // Angel is lit
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LightCone"))
        {
            Debug.Log("Angel is out of the flashlight's light!");
            isLit = false; // Angel is no longer lit
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player")) // Assuming the player is tagged as "Player"
        {
            Debug.Log("Angel caught the player!");
            // Play caught sound
            caughtSound.Play();

            // Reset the player's position
            player.position = resetPosition;

            // Reset angel’s speed
            moveSpeed = 1f;
        }
    }
}
