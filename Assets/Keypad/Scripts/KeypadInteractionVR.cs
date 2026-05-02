using UnityEngine;
using UnityEngine.InputSystem; // Required for VR inputs

namespace NavKeypad 
{ 
    public class KeypadInteractionVR : MonoBehaviour
    {
        [Header("VR Input Setup")]
        [Tooltip("Drag your Right Controller object here")]
        public Transform rightControllerTransform; 
        
        [Tooltip("Bind this to the Right Controller Trigger")]
        public InputActionProperty rightTriggerAction; 

        [Header("Raycast Settings")]
        public float rayDistance = 5f;
        public LayerMask interactableLayer = ~0; // Leave as 'Everything' or set to a specific UI/Button layer

        private void Update()
        {
            // Check if the trigger was pulled this exact frame
            if (rightTriggerAction.action.WasPressedThisFrame())
            {
                // Shoot a ray forward from the front of the VR controller
                if (Physics.Raycast(rightControllerTransform.position, rightControllerTransform.forward, out var hit, rayDistance, interactableLayer))
                {
                    // If the ray hits an object with the KeypadButton script, press it!
                    if (hit.collider.TryGetComponent(out KeypadButton keypadButton))
                    {
                        keypadButton.PressButton();
                    }
                }
            }
        }
    }
}