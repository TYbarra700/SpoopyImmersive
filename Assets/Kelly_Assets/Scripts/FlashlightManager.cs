using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class FlashlightManager : MonoBehaviour
{
    [SerializeField] private Material flashlight_MAT;
    [SerializeField] private GameObject flashlight_OBJ;
    [SerializeField] private Light spotLight;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips; // 0 is click sfx; 1 is battery going out; 2 is halfway cue

    [SerializeField] private bool isLightOn = false;
    [SerializeField] private float lightIntensity = 4.06f;
    private float remainingBattery = 15.0f; // 10 seconds of battery
    private float batteryLife = 15f;
    private bool isRecharging = false;
    private float rechargeTime = 9.0f; // 9 seconds recharge time
    private bool canTurnOn = true;
    private bool toggleCoolDown = false;
    private bool wasTriggerPressed = false;

    private float jumpscareTimer = 0f;
    private float jumpscareThreshold = 20f; // Time in seconds before a jumpscare happens
    private bool halfwayCuePlayed = false;

    [SerializeField] private XRGrabInteractable grabInteractable;
    [SerializeField] private JumpscareManager jumpscareManager;
    public delegate void JumpscareAction(int jumpscareType);
    public static event JumpscareAction OnJumpscare;
    private InputDevice leftController;
    private InputDevice rightController;
    private bool isHeldByLeftHand = false;
    private bool isHeldByRightHand = false;


    void Start()
    {
        LightOn();
        leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
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
        // Check if the flashlight is held by the left hand and process input
        if (isHeldByLeftHand && leftController.isValid &&
            leftController.TryGetFeatureValue(CommonUsages.triggerButton, out bool isLeftTriggerPressed))
        {
            HandleTriggerPress(isLeftTriggerPressed);
        }
        // Check if the flashlight is held by the right hand and process input
        else if (isHeldByRightHand && rightController.isValid &&
            rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool isRightTriggerPressed))
        {
            HandleTriggerPress(isRightTriggerPressed);
        }
    }

    private void HandleTriggerPress(bool isTriggerPressed)
    {
        if (isTriggerPressed && !wasTriggerPressed && !toggleCoolDown)
        {
            // Toggle light on/off
            if (isLightOn)
            {
                audioSource.PlayOneShot(audioClips[0]);
                LightOff();
            }
            else
            {
                LightOn();
            }

            // Prevent rapid toggling
            StartCoroutine(ToggleCoolDownRoutine());
        }

        // Update the previous state of the trigger
        wasTriggerPressed = isTriggerPressed;
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

            StartCoroutine(FlashlightDies());
        }
    }

    IEnumerator FlashlightDies()
    {
        //float startIntensity = fullIntensity;
        float elapsedTime = 0;

        while (remainingBattery > 0)
        {
            elapsedTime += Time.deltaTime;
            remainingBattery = Mathf.Clamp(batteryLife - elapsedTime, 0, batteryLife); // Decrease battery smoothly over time
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

            //Debug.Log("Fade Light ElapsedTime = " + elapsedTime);
            yield return null;
        }
    }

    IEnumerator RechargeFlashlight()
    {
        isRecharging = true;
        canTurnOn = false;

        yield return new WaitForSeconds(rechargeTime); // Wait for 5 seconds to recharge

        remainingBattery = batteryLife; // Reset battery after recharge
        isRecharging = false;
        canTurnOn = true; // Allow flashlight to turn on again
    }


    private void LightOff()
    {
        flashlight_MAT.DisableKeyword("_EMISSION");
        spotLight.enabled = false;
        isLightOn = false;
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Determine which hand is grabbing the flashlight
        if (args.interactorObject.transform.CompareTag("LeftHand"))
        {
            isHeldByLeftHand = true;
            isHeldByRightHand = false;
        }
        else if (args.interactorObject.transform.CompareTag("RightHand"))
        {
            isHeldByRightHand = true;
            isHeldByLeftHand = false;
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Reset hand states when the flashlight is released
        isHeldByLeftHand = false;
        isHeldByRightHand = false;
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
        //int jumpscareType = Random.Range(1, 4); // Random jumpscare type
        //Debug.Log($"Jumpscare triggered! Type: {jumpscareType}");

        if (jumpscareManager.isJumpscareActive) return;
        int zombieHandsJumpscareType = 1;
        OnJumpscare?.Invoke(zombieHandsJumpscareType);
    }

    private void OnDestroy()
    {
        // Unsubscribe from grab/release events to prevent memory leaks
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }
}
