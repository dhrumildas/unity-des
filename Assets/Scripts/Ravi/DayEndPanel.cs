using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DayEndPanel : MonoBehaviour
{
    public static DayEndPanel Instance;

    [Header("UI References")]
    public GameObject panel;
    public TMP_Text congratsText;
    public TMP_Text dayScoreText;
    public TMP_Text quotaText;
    public Button nextDayButton;
    public Button gameOverButton;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        nextDayButton.onClick.AddListener(OnNextDayClicked);
        gameOverButton.onClick.AddListener(OnGameOverClicked);
    }

    public void Show()
    {
        if (panel.activeSelf) return;

        int score = HUDManager.Instance.GetScore();
        bool passing = HUDManager.Instance.IsPassing();

        congratsText.text = passing
            ? "Great work today!\nAurora's mail is safe."
            : "Not good enough today.\nAurora's safety was compromised.";

        dayScoreText.text = $"Day Score: {score}";
        quotaText.text = passing ? "PASS" : "FAIL";

        // show correct button
        nextDayButton.gameObject.SetActive(passing);
        gameOverButton.gameObject.SetActive(!passing);

        panel.SetActive(true);
    }

    void OnNextDayClicked()
    {
        panel.SetActive(false);
        Debug.Log("Day 2 not implemented yet!");
    }

    void OnGameOverClicked()
    {
        Debug.Log("Game Over screen not implemented yet!");
    }

    void OnDestroy()
    {
        nextDayButton.onClick.RemoveAllListeners();
        gameOverButton.onClick.RemoveAllListeners();
    }
}