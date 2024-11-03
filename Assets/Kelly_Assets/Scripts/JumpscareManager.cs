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
        // Position hands a set distance in front of the player's camera, aligning with the camera's orientation
        Vector3 handPositionOffset = player.forward + player.up; // Adjust forward and height offset as needed
        hands.position = player.position + handPositionOffset;

        // Rotate hands to face the player by matching camera's rotation
        hands.rotation = Quaternion.LookRotation(hands.position - player.position);
        //hands.Rotate(0, 180, 0); // Flip rotation 180 degrees to ensure palms face player

        

        hands.gameObject.SetActive(true);

        // 2. Play creepy sound
        if(!audioSource.isPlaying) audioSource.PlayOneShot(jumpscareSound);

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
        hands.gameObject.SetActive(false);
        yield return StartCoroutine(FadeToClear());

        // Reset
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
