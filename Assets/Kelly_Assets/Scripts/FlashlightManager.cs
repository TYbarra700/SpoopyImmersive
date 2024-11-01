using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FlashlightManager : MonoBehaviour
{
    [SerializeField] private Material flashlight_MAT;
    [SerializeField] private GameObject flashlight_OBJ;
    [SerializeField] private Light spotLight;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips; // 0 is click sfx; 1 is battery going out

    [SerializeField] private bool isLightOn = false;
    [SerializeField] private float lightIntensity = 4.06f;
    private float fullIntensity = 4.06f;
    private float remainingBattery = 10.0f; // 6 seconds of battery
    private bool isRecharging = false;
    private float rechargeTime = 9.0f; // 5 seconds recharge time
    private bool canTurnOn = true;

    private float jumpscareChance = 0.0f;
    private bool hasJumpscareOccurred = false;
    public delegate void JumpscareAction(int jumpscareType);
    public static event JumpscareAction OnJumpscare;
    private InputDevice leftController;
    private InputDevice rightController;

    void Start()
    {
        LightOff();
        leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    void Update()
    {
        // Ensure controllers are assigned
        if (!leftController.isValid || !rightController.isValid)
        {
            AssignControllers();
        }

        UpdateLightState();
        CheckJumpscare();
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

    private void UpdateLightState()
    {
        // Check if "l" or the main trigger is pressed on the right controller
        if (Input.GetKeyDown(KeyCode.L) || (rightController.isValid && rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool isTriggerPressed) && isTriggerPressed))
        {
            if (isLightOn)
            {
                LightOff();
                audioSource.PlayOneShot(audioClips[0]);
            }
            else
            {
                LightOn();
            }

            // turn on trigger to turn on / off flashlight
        }


        // temporary jumpscare test
        if (Input.GetKeyDown(KeyCode.J))
        {
            jumpscareChance = 100f;
        }

        // Check if the A button is pressed on the left controller
        if (leftController.isValid && leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool isYPressed) && isYPressed)
        {
            jumpscareChance = 100f;
        }
    }

    private void LightOn()
    {
        if (!isRecharging && canTurnOn)
        {
            // Play click audio
            audioSource.PlayOneShot(audioClips[0]);
            flashlight_MAT.EnableKeyword("_EMISSION");
            spotLight.enabled = true;
            isLightOn = true;

            StartCoroutine(FlashlightDies());
            UpdateJumpscareChance();
        }
    }

    private void LightOff()
    {
        flashlight_MAT.DisableKeyword("_EMISSION");
        spotLight.enabled = false;
        isLightOn = false;
        UpdateJumpscareChance();
    }

    IEnumerator FlashlightDies()
    {
        //float startIntensity = fullIntensity;
        float elapsedTime = 0;

        while (remainingBattery > 0)
        {
            elapsedTime += Time.deltaTime;
            remainingBattery = Mathf.Clamp(10.0f - elapsedTime, 0, 10.0f); // Decrease battery smoothly over time
            //spotLight.intensity = Mathf.Lerp(startIntensity, 0, elapsedTime / 10.0f); // Dim over 6 seconds

            if (!isLightOn)
            {
                // Save remaining battery when the light is turned off manually
                yield break;
            }


            if (remainingBattery <= 0)
            {
                // Battery depleted
                audioSource.PlayOneShot(audioClips[1]); // Play battery depletion sound
                LightOff();
                StartCoroutine(RechargeFlashlight());
                yield break;
            }

            Debug.Log("Fade Light ElapsedTime = " + elapsedTime);
            yield return null;
        }
    }

    IEnumerator RechargeFlashlight()
    {
        isRecharging = true;
        canTurnOn = false;

        yield return new WaitForSeconds(rechargeTime); // Wait for 5 seconds to recharge

        remainingBattery = 10.0f; // Reset battery after recharge
        isRecharging = false;
        canTurnOn = true; // Allow flashlight to turn on again
    }

    private void UpdateJumpscareChance()
    {
        if (isLightOn)
        {
            jumpscareChance = Random.Range(50f, 100f); // 50-100% chance when flashlight is on
        }
        else
        {
            jumpscareChance = Random.Range(0f, 40f); // 0-40% chance when flashlight is off
        }

        // Increase the chance slightly for each use
        jumpscareChance += isLightOn ? Random.Range(0.1f, 1.0f) : Random.Range(0f, 0.5f);

        Debug.Log($"JumpscareChance = {jumpscareChance}");
    }

    void CheckJumpscare()
    {
        // Determine if a jumpscare happens
        if (jumpscareChance >= 85f)
        {
            // Simulate different types of jumpscares
            int jumpscareType = Random.Range(1, 4); // Jumpscare types 1, 2, 3
            Debug.Log($"Jumpscare occurred! Type: {jumpscareType}, Chance: {jumpscareChance}%");

            // call event on jumpscare manager to do jumpscare
            //if (OnJumpscare != null) OnJumpscare(jumpscareType);
            OnJumpscare?.Invoke(jumpscareType);

            // Reset the chance after a jumpscare occurs
            jumpscareChance = 0f;
        }
    }
}
