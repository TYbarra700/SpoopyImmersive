using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JumpscareManager : MonoBehaviour
{
    [SerializeField] private Transform hands;
    [SerializeField] private Transform player;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpscareSound;
    [SerializeField] private Image screenOverlay; // UI Image used as a green filter
    [SerializeField] private CanvasGroup fadeCanvasGroup; // To control camera fade-in and fade-out

    private void OnEnable()
    {
        FlashlightManager.OnJumpscare += TriggerJumpscare;
    }

    private void OnDisable()
    {
        FlashlightManager.OnJumpscare -= TriggerJumpscare;
    }

    private void Start()
    {
        screenOverlay.color = Color.clear;
        fadeCanvasGroup.alpha = 0; // Fully visible screen at start
        hands.gameObject.SetActive(false); // Hide hands initially
    }

    private void TriggerJumpscare(int jumpscareType)
    {
        StartCoroutine(PlayJumpscare());
    }

    private IEnumerator PlayJumpscare()
    {
        // 1. Position hands and enable them
        hands.position = player.position + player.forward * 1.5f; // Adjust distance as needed
        hands.gameObject.SetActive(true);

        // 2. Play creepy sound
        audioSource.PlayOneShot(jumpscareSound);

        // 3. Apply green filter effect
        screenOverlay.color = new Color(0, 1, 0, 0.5f); // Set to semi-transparent green

        // 4. Fade camera to black
        yield return StartCoroutine(FadeToBlack());

        // 5. Wait briefly before fading back in
        yield return new WaitForSeconds(1.5f);

        // 6. Fade camera back to normal
        yield return StartCoroutine(FadeToClear());

        // Reset
        hands.gameObject.SetActive(false);
        screenOverlay.color = Color.clear;
    }

    private IEnumerator FadeToBlack()
    {
        float duration = 0.5f;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = 1;
    }

    private IEnumerator FadeToClear()
    {
        float duration = 0.5f;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = 0;
    }
}
