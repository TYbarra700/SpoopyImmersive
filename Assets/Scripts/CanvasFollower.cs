using UnityEngine;

public class CanvasFollower : MonoBehaviour
{
    public Canvas tutorialCanvas;
    public float distanceFromPlayer = 1.0f; // Distance in front of the player
    public float yFixedHeight = 1.5f; // Fixed height for the canvas
    public float recenterThresholdAngle = 45f; // Angle threshold to recenter the canvas

    private Vector3 initialPositionOffset;

    void Start()
    {
        // Initial position offset, keeping the canvas at a fixed height
        initialPositionOffset = new Vector3(0, yFixedHeight, distanceFromPlayer);
        PositionCanvas();
    }

    void Update()
    {
        PositionCanvas();

        // Check if player turns head enough to recenter
        if (Vector3.Angle(Camera.main.transform.forward, tutorialCanvas.transform.position - Camera.main.transform.position) > recenterThresholdAngle)
        {
            PositionCanvas();
        }
    }

    void PositionCanvas()
    {
        // Calculate new position in front of player, keeping y fixed
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * distanceFromPlayer;
        targetPosition.y = Camera.main.transform.position.y + yFixedHeight;

        // Smooth movement and rotation to keep the canvas facing the player
        tutorialCanvas.transform.position = Vector3.Lerp(tutorialCanvas.transform.position, targetPosition, Time.deltaTime * 5f);
        tutorialCanvas.transform.rotation = Quaternion.LookRotation(tutorialCanvas.transform.position - Camera.main.transform.position);
    }
}
