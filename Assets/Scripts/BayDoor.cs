using UnityEngine;
using System.Collections;

public class BayDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public float openHeight = 3.5f; // How high the door goes
    public float moveSpeed = 3f;    // How fast it moves

    [Header("Audio")]
    public AudioClip mechanicalSound;
    private AudioSource audioSource;

    private Vector3 closedPosition;
    private bool isOpen = false;
    private Coroutine moveCoroutine;

    void Start()
    {
        // Remember where the door started so it knows where to close
        closedPosition = transform.position;

        // Automatically add an AudioSource and make it 3D spatial
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f; 
        audioSource.playOnAwake = false;
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            PlaySound();
            
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            // Calculate the target position (current start position + height on the Y axis)
            Vector3 targetPos = closedPosition + (Vector3.up * openHeight);
            moveCoroutine = StartCoroutine(MoveToPosition(targetPos));
        }
    }

    public void CloseDoor()
    {
        if (isOpen)
        {
            isOpen = false;
            PlaySound();
            
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveToPosition(closedPosition));
        }
    }

    private void PlaySound()
    {
        if (mechanicalSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(mechanicalSound);
        }
    }

    // A Coroutine smoothly interpolates the position over time
    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
            yield return null; // Wait until next frame
        }
        
        // Snap perfectly to the target at the end
        transform.position = targetPosition; 
    }
}