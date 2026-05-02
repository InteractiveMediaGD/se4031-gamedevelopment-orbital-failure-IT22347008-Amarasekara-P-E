using UnityEngine;
using TMPro; 
using UnityEngine.UI; 
using UnityEngine.SceneManagement; // Required for reloading the scene
using System.Collections;          // Required for Coroutines

public class OxygenSystem : MonoBehaviour
{
    [Header("Oxygen Settings")]
    public float maxOxygen = 100f;
    public float currentOxygen;
    public float depletionRate = 1f; 
    public bool isBreached = false;
    public bool isInsideRefillZone = false;

    [Header("UI References")]
    public TextMeshProUGUI oxygenText;
    public Slider oxygenSlider;   
    public Image sliderFillImage; 

    [Header("Game Over Settings")]
    [Tooltip("The Canvas Group attached to your Black Death Screen")]
    public CanvasGroup deathScreenCanvas; 
    [Tooltip("Drag your XR Locomotion System here to disable walking")]
    public GameObject locomotionSystem; 
    public AudioSource audioSource;
    public AudioClip failureSound;
    public float fadeSpeed = 0.5f; // How fast the screen fades to black
    public float timeBeforeRestart = 3f; // How long to wait in the dark before reloading

    private bool isDead = false;

    void Start()
    {
        currentOxygen = maxOxygen;

        if (oxygenSlider != null)
        {
            oxygenSlider.maxValue = maxOxygen;
            oxygenSlider.value = currentOxygen;
        }

        // Ensure the death screen is invisible at the start
        if (deathScreenCanvas != null)
        {
            deathScreenCanvas.alpha = 0f;
            deathScreenCanvas.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // If the player is already dead, stop draining oxygen and ignore the rest of the update
        if (isDead) return;

        float currentDrain = isBreached ? depletionRate * 5f : depletionRate;
        currentOxygen -= currentDrain * Time.deltaTime;
        currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);

        UpdateUI();

        if (currentOxygen <= 0)
        {
            StartCoroutine(GameOverSequence());
        }
    }

    void UpdateUI()
    {
        if (oxygenText != null)
        {
            oxygenText.text = "O2: " + Mathf.RoundToInt(currentOxygen).ToString() + "%";
            oxygenText.color = (currentOxygen < 20f) ? Color.red : Color.white;
        }

        if (oxygenSlider != null)
        {
            oxygenSlider.value = currentOxygen; 
        }

        if (sliderFillImage != null)
        {
            sliderFillImage.color = (currentOxygen < 20f) ? Color.red : Color.cyan;
        }
    }

    public void EmergencyRefill()
    {
        if (isDead) return; // Don't allow refills if they are already dying
        currentOxygen = maxOxygen;
        isBreached = false; 
    }

    private IEnumerator GameOverSequence()
    {
        isDead = true;

        // 1. Play the failure sound
        if (audioSource != null && failureSound != null)
        {
            audioSource.PlayOneShot(failureSound);
        }

        // 2. Freeze the player by turning off their ability to move
        if (locomotionSystem != null)
        {
            locomotionSystem.SetActive(false);
        }

        // 3. Fade the screen to black
        if (deathScreenCanvas != null)
        {
            deathScreenCanvas.gameObject.SetActive(true);
            
            // Gradually increase the alpha from 0 to 1
            while (deathScreenCanvas.alpha < 1f)
            {
                deathScreenCanvas.alpha += fadeSpeed * Time.deltaTime;
                yield return null; // Wait for the next frame
            }
        }

        // 4. Wait in the dark for a few seconds
        yield return new WaitForSeconds(timeBeforeRestart);

        // 5. Reload the current scene!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}