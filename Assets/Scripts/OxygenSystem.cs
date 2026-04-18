using UnityEngine;
using TMPro; 

public class OxygenSystem : MonoBehaviour
{
    [Header("Oxygen Settings")]
    public float maxOxygen = 100f;
    public float currentOxygen;
    public float depletionRate = 1f; // How much oxygen is lost per second
    public bool isBreached = false;
    public bool isInsideRefillZone = false;

    [Header("UI References")]
    public TextMeshProUGUI oxygenText;

    void Start()
    {
        currentOxygen = maxOxygen;
    }

    void Update()
    {
        // If there's a hull breach, drain 5 times faster
        float currentDrain = isBreached ? depletionRate * 5f : depletionRate;
        
        currentOxygen -= currentDrain * Time.deltaTime;
        
        // Clamp the value so it doesn't drop below 0
        currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);

        UpdateUI();

        if (currentOxygen <= 0)
        {
            Debug.Log("Out of Oxygen! Game Over.");
            // We will add the actual Game Over logic here later
        }
    }

    void UpdateUI()
    {
        if (oxygenText != null)
        {
            // Displays oxygen as a whole number percentage
            oxygenText.text = "O2: " + Mathf.RoundToInt(currentOxygen).ToString() + "%";
            
            // Turn text red if below 20%
            if (currentOxygen < 20f)
                oxygenText.color = Color.red;
            else
                oxygenText.color = Color.white;
        }
    }

    // A method to test refilling the oxygen
    public void EmergencyRefill()
    {
        currentOxygen = maxOxygen;
        isBreached = false; // Fixes the breach
    }
}