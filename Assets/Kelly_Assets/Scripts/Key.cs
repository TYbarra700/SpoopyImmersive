using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Key : MonoBehaviour
{
    //[SerializeField] private ParticleSystem pfx;              // The particle effect attached to the key
    [SerializeField] private XRGrabInteractable grabInteractable; // The XR Grab Interactable component
    [SerializeField] private Light keyLight;                  // The light attached to the key
    [SerializeField] private float glowSpeed = 2f;            // Speed of the light fading in and out
    [SerializeField] private float minIntensity = 0.5f;       // Minimum intensity of the light
    [SerializeField] private float maxIntensity = 2f;         // Maximum intensity of the light

    private bool isGrabbed = false; // Flag to check if the key has been grabbed

    void Start()
    {
        // Subscribe to grab events
        grabInteractable.selectEntered.AddListener(OnKeyGrabbed);

        // Start the light fading coroutine
        StartCoroutine(GlowEffect());
    }

    private void OnKeyGrabbed(SelectEnterEventArgs args)
    {
        // Stop the light and particle effect when the key is grabbed
        isGrabbed = true;
        keyLight.enabled = false;
        //if (pfx != null)
        //{
        //    pfx.Stop();
        //}
    }

    private IEnumerator GlowEffect()
    {
        while (!isGrabbed)
        {
            // Gradually change the light's intensity between min and max
            float intensity = Mathf.PingPong(Time.time * glowSpeed, maxIntensity - minIntensity) + minIntensity;
            if (keyLight != null)
            {
                keyLight.intensity = intensity;
            }

            yield return null; // Wait for the next frame
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from grab events to avoid memory leaks
        grabInteractable.selectEntered.RemoveListener(OnKeyGrabbed);
    }
}
