using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ButtonSequenceManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public int sequenceLength = 5;
    
    [Header("UI & Feedback")]
    public TextMeshProUGUI sequenceDisplay;
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip successChime;
    public AudioClip errorBuzz;
    public AudioClip powerUpSound;

    [Header("Lighting System")]
    public GameObject[] normalLights;
    public GameObject[] emergencyLights;

    private List<int> targetSequence = new List<int>();
    private List<int> currentInput = new List<int>();
    private bool isPoweredOn = false;

    void Start()
    {
        SetPowerState(false); // Start in emergency mode
        GenerateRandomSequence();
    }

    public void GenerateRandomSequence()
    {
        targetSequence.Clear();
        string displayString = "SYSTEM OVERRIDE CODE:\n";

        for (int i = 0; i < sequenceLength; i++)
        {
            // Generates a random number from 1 to 5
            int randomNum = Random.Range(1, 6); 
            targetSequence.Add(randomNum);
            displayString += randomNum.ToString() + " ";
        }

        if (sequenceDisplay != null)
        {
            sequenceDisplay.text = displayString;
            sequenceDisplay.color = Color.yellow;
        }
        
        Debug.Log("Target Sequence Generated: " + displayString);
    }

    // Called by the individual VR Buttons
    public void ReceiveInput(int buttonValue)
    {
        if (isPoweredOn) return; // Don't do anything if puzzle is already solved

        PlaySound(buttonClickSound);
        currentInput.Add(buttonValue);

        // Update UI to show how many buttons have been pressed
        sequenceDisplay.text = $"ENTERING CODE...\n[{currentInput.Count} / {sequenceLength}]";
        sequenceDisplay.color = Color.white;

        // Check if the player has entered all 5 digits
        if (currentInput.Count >= sequenceLength)
        {
            CheckSequence();
        }
    }

    private void CheckSequence()
    {
        bool isCorrect = true;

        // Compare the player's input to the target sequence
        for (int i = 0; i < sequenceLength; i++)
        {
            if (currentInput[i] != targetSequence[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            // SUCCESS!
            isPoweredOn = true;
            PlaySound(successChime);
            RestorePower();
        }
        else
        {
            // FAILURE! Reset the system.
            PlaySound(errorBuzz);
            ResetPuzzle();
        }
    }

    private void ResetPuzzle()
    {
        currentInput.Clear();
        
        // Regenerate the code on failure to make it harder, or comment 
        // this out to let them try the same code again!
        GenerateRandomSequence(); 
    }

    private void RestorePower()
    {
        PlaySound(powerUpSound);
        if (sequenceDisplay != null)
        {
            sequenceDisplay.text = "POWER RESTORED.\nSYSTEMS NOMINAL.";
            sequenceDisplay.color = Color.green;
        }
        SetPowerState(true);
    }

    private void SetPowerState(bool isPowered)
    {
        foreach (var light in normalLights) light.SetActive(isPowered);
        foreach (var light in emergencyLights) light.SetActive(!isPowered);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null) 
        {
            audioSource.PlayOneShot(clip);
        }
    }
}