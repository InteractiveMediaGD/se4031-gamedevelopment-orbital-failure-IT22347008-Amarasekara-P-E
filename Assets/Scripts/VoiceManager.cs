using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.InputSystem; // Required for reading VR controllers
using System.Collections.Generic;
using System.Linq;

public class VoiceManager : MonoBehaviour
{
    [Header("System References")]
    public OxygenSystem oxygenSystem;

    [Header("VR Input")]
    [Tooltip("Bind this to the Left Controller Trigger")]
    public InputActionProperty pttButton;

    [Header("Audio Feedback")]
    public AudioSource stationSpeaker;
    public AudioClip listeningBeep; // Add a high-pitched 'activation' sound
    public AudioClip successBeep;
    public AudioClip errorBuzz;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> commandActions = new Dictionary<string, System.Action>();

    // PTT State Tracking
    private bool isListening = false;
    private bool commandProcessedThisPress = false;
    private bool wasPttPressedLastFrame = false;

    void Start()
    {
        commandActions.Add("compress atmosphere", CommandCompressAtmosphere);
        commandActions.Add("synchronize shield grid", CommandSyncShields);
        commandActions.Add("reroute life support", CommandReroutePower);

        keywordRecognizer = new KeywordRecognizer(commandActions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += OnSpeechRecognized;
        
        // We leave the recognizer running in the background, 
        // but we only ACT on what it hears if the button is held.
        keywordRecognizer.Start(); 
    }

    void Update()
    {
        // Check if the Left Trigger is currently being squeezed
        bool isPttPressed = pttButton.action.IsPressed();

        // 1. Button just pressed down this frame
        if (isPttPressed && !wasPttPressedLastFrame)
        {
            isListening = true;
            commandProcessedThisPress = false; // Reset the success flag
            PlayFeedback(listeningBeep);
            Debug.Log("COMMS OPEN: Listening...");
        }
        
        // 2. Button just released this frame
        if (!isPttPressed && wasPttPressedLastFrame)
        {
            isListening = false;
            
            // If the player let go and NO valid command was spoken...
            // This is how we fulfill the "Unrecognized Command" requirement!
            if (!commandProcessedThisPress)
            {
                PlayFeedback(errorBuzz);
                Debug.Log("COMMS CLOSED: Command Unrecognized.");
            }
        }

        wasPttPressedLastFrame = isPttPressed;
    }

    private void OnSpeechRecognized(PhraseRecognizedEventArgs speech)
    {
        // Ignore any speech if the player isn't holding the trigger
        if (!isListening) return;

        Debug.Log("Command Detected: " + speech.text);
        
        // Mark as successful so the error buzzer doesn't play when they let go of the trigger
        commandProcessedThisPress = true; 
        
        // Automatically close the comms channel once a successful command is issued
        isListening = false; 

        // Execute the method tied to the spoken phrase
        commandActions[speech.text].Invoke();
    }

    // --- STATION COMMANDS ---

    private void CommandCompressAtmosphere()
    {
        // Now checks if the player is physically inside the trigger box!
        if (oxygenSystem != null && oxygenSystem.isInsideRefillZone)
        {
            Debug.Log("Executing: Compress Atmosphere");
            oxygenSystem.EmergencyRefill();
            PlayFeedback(successBeep);
        }
        else
        {
            Debug.Log("FAILED: Out of Range of Refill Terminals!");
            PlayFeedback(errorBuzz); // Plays error if they use the right command in the wrong place
        }
    }

    private void CommandSyncShields() { PlayFeedback(successBeep); }
    private void CommandReroutePower() { PlayFeedback(successBeep); }

    // --- HELPER METHODS ---

    private void PlayFeedback(AudioClip clip)
    {
        if (stationSpeaker != null && clip != null)
        {
            stationSpeaker.PlayOneShot(clip);
        }
    }

    private void OnApplicationQuit()
    {
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }
    }
}