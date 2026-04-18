using UnityEngine;

public class RefillZone : MonoBehaviour
{
    public OxygenSystem oxygenSystem;

    // Triggered when the XR Rig enters the cube
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            oxygenSystem.isInsideRefillZone = true;
            Debug.Log("Entered Refill Zone");
        }
    }

    // Triggered when the XR Rig leaves the cube
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            oxygenSystem.isInsideRefillZone = false;
            Debug.Log("Exited Refill Zone");
        }
    }
}