using UnityEngine;

public class VRButton : MonoBehaviour
{
    [Header("Button Settings")]
    [Tooltip("The number this button represents (e.g., 1, 2, 3, 4, or 5)")]
    public int buttonValue = 1;
    
    [Header("References")]
    public ButtonSequenceManager manager;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // The XR Simple Interactable will call this method when the player clicks it
    public void PressButton()
    {
        // 1. Play the physical animation
        if (animator != null)
        {
            // Assumes you have a trigger parameter named "Push" in your Animator
            animator.SetTrigger("Push"); 
        }

        // 2. Send this button's value to the manager
        if (manager != null)
        {
            manager.ReceiveInput(buttonValue);
        }
        else
        {
            Debug.LogError("VRButton is missing a reference to the Sequence Manager!");
        }
    }
}