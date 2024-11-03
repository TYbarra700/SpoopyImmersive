using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JumpscareManager : MonoBehaviour
{
    [SerializeField] private Transform hands;
    [SerializeField] private Transform player;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpscareSound;
    [SerializeField] private Material _material;
    [SerializeField] private GameObject fadeBox;

    
    private Color32 spookyGreen = new Color32(33, 93, 31, 150);
    //[SerializeField] private Image screenOverlay; // UI Image used as a green filter
    //[SerializeField] private CanvasGroup fadeCanvasGroup; // To control camera fade-in and fade-out

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
        hands.gameObject.SetActive(false); // Hide hands initially
        fadeBox.SetActive(false);
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
        fadeBox.SetActive(true);
        _material.SetColor("_Color", spookyGreen);
        _material.SetFloat("_Alpha", .6f);

        // allow green effect to stay a lil
        yield return new WaitForSeconds(1.5f);

        // 4. Fade camera to black
        yield return StartCoroutine(FadeEffect());

        // 5. Wait briefly before fading back in
        yield return new WaitForSeconds(1.5f);

        // 6. Fade camera back to normal
        yield return StartCoroutine(FadeToClear());

        // Reset
        hands.gameObject.SetActive(false);
        _material.SetFloat("_Alpha", 0);
        fadeBox.SetActive(false);
    }

    //private IEnumerator FadeToBlack()
    //{
    //    float duration = 0.5f;
    //    float elapsedTime = 0;

    //    while (elapsedTime < duration)
    //    {
    //        screenOverlay.color = Color.Lerp(spookyGreen, Color.black,  elapsedTime / duration);
    //        fadeCanvasGroup.alpha = Mathf.Lerp(0.6f, 1, elapsedTime / duration);
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }
    //    fadeCanvasGroup.alpha = 1;
    //    screenOverlay.color = Color.black;
    //}

    private IEnumerator FadeEffect()
    {
        float duration = 0.5f;
        float elapsedTime = 0;
        float endAlpha = 1;
        float startAlpha = _material.GetFloat("_Alpha");

        while (elapsedTime < duration)
        {
            _material.SetColor("_Color", Color.Lerp(spookyGreen, Color.black, elapsedTime / duration));
            _material.SetFloat("_Alpha", Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _material.SetColor("_Color", Color.black);
        _material.SetFloat("_Alpha", endAlpha);
    }
    

    private IEnumerator FadeToClear()
    {
        float duration = 0.5f;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            _material.SetFloat("_Alpha", Mathf.Lerp(1, 0, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
    }
}
