using UnityEngine;
using TMPro; // Required for TextMeshPro UI

public class RepairCodeManager : MonoBehaviour
{
    [Header("References")]
    public NavKeypad.Keypad targetKeypad; // Drag your Keypad here
    
    [Tooltip("Drag the 3 TextMeshPro screen objects here")]
    public TextMeshProUGUI[] possibleScreens; 

    void Start()
    {
        GenerateNewRepairCode();
    }

    public void GenerateNewRepairCode()
    {
        // 1. Generate a random 4-digit number (between 1000 and 9999)
        int newCode = Random.Range(1000, 10000);
        
        // 2. Send it to the Keypad using the method we just added
        if (targetKeypad != null)
        {
            targetKeypad.SetPasscode(newCode);
        }

        // 3. Clear all screens first (make them display an error/offline message)
        foreach(var screen in possibleScreens)
        {
            screen.text = "SYS_OFFLINE:\nNO DATA";
            screen.color = Color.red;
        }

        // 4. Pick one random screen to display the actual code
        int winningScreenIndex = Random.Range(0, possibleScreens.Length);
        possibleScreens[winningScreenIndex].text = "EMERGENCY OVERRIDE:\n" + newCode.ToString();
        possibleScreens[winningScreenIndex].color = Color.green;
    }
}