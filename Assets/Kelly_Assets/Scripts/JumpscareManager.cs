using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JumpscareManager : MonoBehaviour
{
    [SerializeField] private Transform hands;
    [SerializeField] private Collider jumpscareTriggerZone;
    [SerializeField] private Transform player;
    [SerializeField] private Transform tv; // The TV position
    [SerializeField] private Transform crawlingZombie; // For jumpscare type 2
    [SerializeField] private GameObject move; // The locomotion GameObject
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpscareSound;
    [SerializeField] private Material _material;
    [SerializeField] private GameObject fadeBox;

    [SerializeField] private Vector3 handOffset = new Vector3(0, -0.3f, 1.0f); // Editable offset for hand position
    [SerializeField] private Vector3 handRotationOffset = new Vector3(0, 180, 0); // Editable offset for hand rotation
    
    private Color32 spookyGreen = new Color32(33, 93, 31, 150);
    public bool isJumpscareActive { get; private set; } = false;
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
        crawlingZombie.gameObject.SetActive(false); // Hide crawling zombie initially
        fadeBox.SetActive(false);
    }

    public void TriggerJumpscare(int jumpscareType)
    {
        if (isJumpscareActive) return; // Prevent simultaneous jumpscares
        move.SetActive(false); // Disable player movement during jumpscare

        isJumpscareActive = true;
        if (jumpscareType == 1)
        {
            StartCoroutine(PlayJumpscare1());
        }
        else if (jumpscareType == 2)
        {
            StartCoroutine(PlayJumpscare2());
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (jumpscareTriggerZone != null && other.CompareTag("Player"))
    //    {
    //        if (!isJumpscareActive)
    //        {
    //            TriggerJumpscare(2); // Activate jumpscare type 2 when entering trigger zone
    //            Debug.Log("Jumpscare should appear");
    //        }
            
    //    }

    //    Debug.Log("Collided with" + other.name);
    //}

    private IEnumerator PlayJumpscare1()
    {
        // Temporarily parent the hands to the player's camera (head)
        Transform originalParent = hands.parent;
        hands.SetParent(player, false);

        // Set the hands' local position and rotation using the offsets
        hands.localPosition = handOffset;
        hands.localRotation = Quaternion.Euler(handRotationOffset);

        hands.gameObject.SetActive(true);

        // Play sound
        if (!audioSource.isPlaying) audioSource.PlayOneShot(jumpscareSound);

        yield return new WaitForSeconds(1.5f);

        // Fade to black
        yield return StartCoroutine(FadeEffect());

        // Wait briefly before fading back in
        yield return new WaitForSeconds(1f);

        // Fade back to normal
        hands.gameObject.SetActive(false);

        // Reset hands' parent
        hands.SetParent(originalParent);
        yield return StartCoroutine(FadeToClear());

        fadeBox.SetActive(false);
        move.SetActive(true);
        isJumpscareActive = false;
    }

    private IEnumerator PlayJumpscare2()
    {
        // Position and orient the crawling zombie at the TV facing the player body
        crawlingZombie.position = tv.position;
        crawlingZombie.rotation = Quaternion.LookRotation(player.position - tv.position);
        crawlingZombie.gameObject.SetActive(true);

        if (!audioSource.isPlaying) audioSource.PlayOneShot(jumpscareSound);

        yield return new WaitForSeconds(3f); // Wait for the zombie to crawl

        yield return StartCoroutine(FadeEffect());
        yield return new WaitForSeconds(1f);

        crawlingZombie.gameObject.SetActive(false);
        yield return StartCoroutine(FadeToClear());

        fadeBox.SetActive(false);
        move.SetActive(true); // Re-enable player movement
        isJumpscareActive = false;
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
            //_material.SetColor("_Color", Color.Lerp(spookyGreen, Color.black, elapsedTime / duration));
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
