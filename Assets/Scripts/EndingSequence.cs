using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingSequence : MonoBehaviour
{
    [SerializeField]
    private FadeEffect fadeEffect; // Reference to the FadeEffect script

    [SerializeField]
    private float delayBeforeFade = 1.0f; // Delay before starting the fade

    [SerializeField]
    private float doorAnimationDuration = 2.0f; // Duration of the door animation

    [SerializeField]
    private Animator animator; // Reference to the Animator for playing animations

    [SerializeField]
    private string animationTriggerName = "PlayAnimation"; // Trigger name for the animation

    [SerializeField]
    private AudioClip voiceClip; // Voice line to play after the animation and fade

    [SerializeField]
    private GameObject locomotionController; // Reference to the object controlling locomotion

    private AudioSource audioSource;

    private void Start()
    {
        // Get the FadeEffect component from the cube
        if (fadeEffect == null)
        {
            Debug.LogWarning("FadeEffect is not assigned.");
        }

        // Get or add an AudioSource for playing sound effects
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Trigger the sequence when the player enters the collider
        if (other.CompareTag("Player"))
        {
            StartCoroutine(PlayEndingSequence());
        }
    }

    private IEnumerator PlayEndingSequence()
    {
        // Disable locomotion to prevent player movement
        if (locomotionController != null)
        {
            locomotionController.SetActive(false);
            Debug.Log("Locomotion controller disabled.");
        }

        // Wait before starting the fade
        yield return new WaitForSeconds(delayBeforeFade);

        // Start the fade-out effect
        if (fadeEffect != null)
        {
            fadeEffect.Fade(true); // Fade out
            Debug.Log("Fade-out triggered.");
        }

        // Trigger the animation
        if (animator != null && !string.IsNullOrEmpty(animationTriggerName))
        {
            animator.SetTrigger(animationTriggerName);
            Debug.Log("Door animation triggered.");
        }

        // Wait for the door animation to finish (adjust duration as needed)
        yield return new WaitForSeconds(doorAnimationDuration);

        // Play the voice line
        if (voiceClip != null)
        {
            audioSource.PlayOneShot(voiceClip);
            Debug.Log("Voice line played.");

            // Wait for the voice clip to finish playing
            yield return new WaitForSeconds(voiceClip.length);
        }

        // Wait 2 seconds after the audio clip finishes
        yield return new WaitForSeconds(2.0f);

        // Load the next scene in the build order
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
            Debug.Log($"Loading next scene: {nextSceneIndex}");
        }
        else
        {
            Debug.LogWarning("No more scenes in build order to load.");
        }
    }
}
