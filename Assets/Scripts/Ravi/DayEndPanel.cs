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
    public Button nextDayButton;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);

        nextDayButton.onClick.AddListener(OnNextDayClicked);
    }

    public void Show()
    {
        if (panel.activeSelf) return;

        int score = HUDManager.Instance.GetScore();

        congratsText.text = "Great work today!\nAurora's mail is safe for another day.";
        dayScoreText.text = $"Day Score: {score}";

        panel.SetActive(true);
    }

    void OnNextDayClicked()
    {
        // day 2 not implemented yet, just close for now
        panel.SetActive(false);
        Debug.Log("Day 2 not implemented yet!");
    }

    void OnDestroy()
    {
        nextDayButton.onClick.RemoveAllListeners();
    }
}