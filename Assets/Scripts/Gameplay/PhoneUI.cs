using UnityEngine;
using TMPro;
using MailSorting.Data;

public class PhoneUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag the TextMeshPro element on your phone screen here.")]
    public TextMeshProUGUI rulesTextDisplay;

    // Start runs once the moment the scene loads
    void Start()
    {
        RefreshPhoneScreen();
    }

    public void RefreshPhoneScreen()
    {
        if (ScoreManager.Instance == null)
        {
            rulesTextDisplay.text = "ERROR: No connection to supervisor.";
            return;
        }

        if (ScoreManager.Instance.activeRules.Count == 0)
        {
            Debug.LogWarning("Phone detected 0 rules! Auto-loading Day 1 rules for testing.");
            ScoreManager.Instance.InitializeDayOneRules();
            ScoreManager.Instance.GenerateRandomLimits();
        }

        // 1. Start with a header
        string screenText = "<b>DAILY DIRECTIVES:</b>\n\n";

        // 2. Loop through every active rule from the ScoreManager
        foreach (MailRules rule in ScoreManager.Instance.activeRules)
        {
            // Fetch the short 5-7 word string you wrote
            string ruleDescription = ScoreManager.Instance.GetRuleDescription(rule);

            // Add a bullet point, the text, and a line break
            screenText += $"• {ruleDescription}\n";
        }

        // 3. Shove the massive string into the actual TextMeshPro component
        rulesTextDisplay.text = screenText;
    }
}