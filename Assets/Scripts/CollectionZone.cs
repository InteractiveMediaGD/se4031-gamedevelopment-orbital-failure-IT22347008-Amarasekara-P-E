using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Keep this if your XRI version requires it, or use standard XRI namespaces
using UnityEngine.XR.Interaction.Toolkit.Interactables; 

public class CollectionZone : MonoBehaviour
{
    [Header("Target Item")]
    [Tooltip("Drag the exact GameObject you want to collect here")]
    public GameObject specificCollectible; // Replaced the string Tag with a direct GameObject reference

    [Header("Snap Settings")]
    public Transform snapPoint; 

    [Header("Feedback")]
    public AudioSource audioSource;
    public AudioClip successSound;

    private bool itemCollected = false;

    private void OnTriggerEnter(Collider other)
    {
        // 1. Check if the object entering the box is the EXACT GameObject we assigned
        if (!itemCollected && other.gameObject == specificCollectible)
        {
            itemCollected = true;
            Debug.Log("Target Item Collected!");

            // 2. Force the player to drop it
            XRGrabInteractable grabScript = other.GetComponent<XRGrabInteractable>();
            if (grabScript != null)
            {
                grabScript.enabled = false; 
            }

            // 3. Freeze physics
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; 
            }

            // 4. Snap it to the pedestal
            other.transform.position = snapPoint.position;
            other.transform.rotation = snapPoint.rotation;

            // 5. Play sound
            if (audioSource != null && successSound != null)
            {
                audioSource.PlayOneShot(successSound);
            }
        }
    }
}