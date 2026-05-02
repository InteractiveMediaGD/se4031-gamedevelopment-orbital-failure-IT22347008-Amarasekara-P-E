using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR; // REQUIRED for standard XR haptics
using System.Collections;

public class VRWristMenuToggle : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject wristPanel;
    public float animationDuration = 0.25f;
    
    [Header("Animation Curves")]
    [Tooltip("Curve for opening. A value going slightly above 1 creates a bouncy pop-up.")]
    public AnimationCurve popInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    [Tooltip("Curve for closing. Keep this between 0 and 1 for a smooth shrink down.")]
    public AnimationCurve popOutCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Input Settings")]
    [Tooltip("Assign the Left Controller Trigger action here.")]
    public InputActionProperty leftTriggerAction;

    [Header("Haptic Settings")]
    public bool enableHaptics = true;
    [Range(0f, 1f)] 
    [Tooltip("How strong the vibration is (0.0 to 1.0).")]
    public float hapticIntensity = 0.3f; 
    [Tooltip("How long the vibration lasts in seconds.")]
    public float hapticDuration = 0.1f;

    private bool isMenuOpen = false;
    private Vector3 originalScale;
    private Coroutine currentAnimation;

    void Awake()
    {
        if (wristPanel != null)
        {
            // Store the target size, then shrink it to 0 and disable it for the start
            originalScale = wristPanel.transform.localScale;
            wristPanel.transform.localScale = Vector3.zero;
            wristPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Listen for the trigger press to toggle the menu
        if (leftTriggerAction.action != null && leftTriggerAction.action.WasPressedThisFrame())
        {
            ToggleWristMenu();
        }
    }

    public void ToggleWristMenu()
    {
        if (wristPanel == null) return;

        isMenuOpen = !isMenuOpen;

        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        if (isMenuOpen)
        {
            wristPanel.SetActive(true);
            currentAnimation = StartCoroutine(AnimateScale(originalScale, popInCurve, false));
        }
        else
        {
            currentAnimation = StartCoroutine(AnimateScale(Vector3.zero, popOutCurve, true));
        }
    }

    private IEnumerator AnimateScale(Vector3 targetScale, AnimationCurve activeCurve, bool disableOnComplete)
    {
        Vector3 startingScale = wristPanel.transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float timeRatio = elapsedTime / animationDuration;
            
            float curveValue = activeCurve.Evaluate(timeRatio);
            wristPanel.transform.localScale = Vector3.LerpUnclamped(startingScale, targetScale, curveValue);
            
            yield return null;
        }

        // Snap exactly to target
        wristPanel.transform.localScale = targetScale;

        if (disableOnComplete)
        {
            wristPanel.SetActive(false);
        }

        // --- NEW: Trigger the haptic bump when the animation finishes ---
        if (enableHaptics)
        {
            TriggerHapticFeedback();
        }
    }

    private void TriggerHapticFeedback()
    {
        // Target the left hand specifically
        UnityEngine.XR.InputDevice leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        
        // Safety check to ensure the controller is connected and valid
        if (leftHandDevice.isValid)
        {
            // Channel 0 is the primary haptic motor
            leftHandDevice.SendHapticImpulse(0, hapticIntensity, hapticDuration);
        }
    }

    public void ForceClose()
    {
        isMenuOpen = false;
        
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        if (wristPanel != null)
        {
            wristPanel.transform.localScale = Vector3.zero;
            wristPanel.SetActive(false);
        }
    }
}