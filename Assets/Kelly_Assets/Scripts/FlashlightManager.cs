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
    [SerializeField] private AudioClip[] audioClips; // 0 is click sfx; 1 is battery going out; 2 is halfway cue

    [SerializeField] private bool isLightOn = false;
    [SerializeField] private float lightIntensity = 4.06f;
    private float fullIntensity = 4.06f;
    private float remainingBattery = 10.0f; // 10 seconds of battery
    private bool isRecharging = false;
    private float rechargeTime = 9.0f; // 9 seconds recharge time
    private bool canTurnOn = true;
    private bool toggleCoolDown = false;
    private bool wasTriggerPressed = false;

    private float jumpscareTimer = 0f;
    private float jumpscareThreshold = 20f; // Time in seconds before a jumpscare happens
    private bool halfwayCuePlayed = false;

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
        UpdateJumpscareTimer();
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
        // Check if the right controller's trigger is pressed and if it should toggle the flashlight
        if (rightController.isValid &&
            rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool isTriggerPressed))
        {
            if (isTriggerPressed && !wasTriggerPressed)
            {
                // Toggle the flashlight
                if (isLightOn)
                {
                    audioSource.PlayOneShot(audioClips[0]);
                    LightOff();
                }
                else
                {
                    LightOn();
                }

                // Begin cooldown after toggling
                StartCoroutine(ToggleCoolDownRoutine());
            }

            // Update the previous state of the trigger
            wasTriggerPressed = isTriggerPressed;
        }
    }

    private IEnumerator ToggleCoolDownRoutine()
    {
        toggleCoolDown = true;
        yield return new WaitForSeconds(0.5f);
        toggleCoolDown = false;
    }

    private void LightOn()
    {
        if (!isRecharging && canTurnOn)
        {
            audioSource.PlayOneShot(audioClips[0]);
            flashlight_MAT.EnableKeyword("_EMISSION");
            spotLight.enabled = true;
            isLightOn = true;
        }
    }

    private void LightOff()
    {
        flashlight_MAT.DisableKeyword("_EMISSION");
        spotLight.enabled = false;
        isLightOn = false;
    }

    private void UpdateJumpscareTimer()
    {
        if (!isLightOn)
        {
            // Increment the timer only when the light is off
            jumpscareTimer += Time.deltaTime;
            Debug.Log("Timer =" + jumpscareTimer);

            // Play the halfway cue if not already played
            if (!halfwayCuePlayed && jumpscareTimer >= jumpscareThreshold / 2)
            {
                audioSource.PlayOneShot(audioClips[2]); // Halfway cue audio
                halfwayCuePlayed = true;
            }

            // Trigger jumpscare if the timer exceeds the threshold
            if (jumpscareTimer >= jumpscareThreshold)
            {
                TriggerJumpscare();
                jumpscareTimer = 0f; // Reset timer
                halfwayCuePlayed = false; // Reset halfway cue
                jumpscareThreshold += Random.Range(0f,10f);
            }
        }
        else
        {
            // Pause the timer when the light is on
        }
    }

    private void TriggerJumpscare()
    {
        int jumpscareType = Random.Range(1, 4); // Random jumpscare type
        Debug.Log($"Jumpscare triggered! Type: {jumpscareType}");
        OnJumpscare?.Invoke(jumpscareType);
    }
}
