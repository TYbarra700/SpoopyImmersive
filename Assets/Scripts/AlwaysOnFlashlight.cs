using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AlwaysOnFlashlight : MonoBehaviour
{
    [SerializeField] private Material flashlight_MAT;
    [SerializeField] private GameObject flashlight_OBJ;
    [SerializeField] private Light spotLight;
    [SerializeField] private GameObject lightCone; // Reference to the light cone's collider GameObject
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound; // Flashlight activation sound

    private InputDevice leftController;
    private InputDevice rightController;

    void Start()
    {
        // Ensure the flashlight starts on
        LightOn();
        AssignControllers();
    }

    void Update()
    {
        // Ensure controllers are assigned
        if (!leftController.isValid || !rightController.isValid)
        {
            AssignControllers();
        }
    }

    private void AssignControllers()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, devices);
        if (devices.Count > 0) leftController = devices[0];

        devices.Clear();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, devices);
        if (devices.Count > 0) rightController = devices[0];
    }

    private void LightOn()
    {
        // Enable flashlight emission
        flashlight_MAT.EnableKeyword("_EMISSION");
        spotLight.enabled = true;

        // Enable light cone collider
        if (lightCone != null)
        {
            lightCone.SetActive(true);
        }

        // Play activation sound
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
