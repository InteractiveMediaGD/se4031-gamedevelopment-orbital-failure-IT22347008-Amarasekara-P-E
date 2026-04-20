using UnityEngine;

public class DoorZone : MonoBehaviour
{
    public VoiceManager voiceManager;
    public BayDoor controlledDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            // Tell the VoiceManager THIS is the door we are looking at
            voiceManager.activeDoor = controlledDoor;
            Debug.Log("Entered Door Access Zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            // Only clear the active door if we are leaving the zone for THIS specific door
            if (voiceManager.activeDoor == controlledDoor) 
            {
                voiceManager.activeDoor = null;
            }
            Debug.Log("Exited Door Access Zone");
        }
    }
}