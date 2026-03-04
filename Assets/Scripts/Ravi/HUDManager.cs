using UnityEngine;
using TMPro;
using MailSorting.Data;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("UI References")]
    public TMP_Text dayText;
    public TMP_Text scoreText;

    [Header("Quota Settings")]
    public int passThreshold = 50;

    private int score = 0;
    private int currentDay = 1;
    private int mistakes = 0;
    private int totalMails = 0;
    private int comboCount = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateDisplay();
        GameTimer.Instance.StartTimer();
    }

    public void OnMailSorted(MailAction playerAction, MailAction correctAction, bool isTutorial)
    {
        if (isTutorial) return; // no scoring for tutorial

        totalMails++;

        int points = CalculateScore(playerAction, correctAction);

        if (points == 10)
        {
            comboCount++;
            if (comboCount > 1)
            {
                int comboBonus = (comboCount - 1) * 2;
                points += comboBonus;
                Debug.Log($"[HUD] Combo x{comboCount}! +{comboBonus} bonus");
            }
        }
        else
        {
            comboCount = 0;
            if (points == 0) mistakes++;
        }

        score += points;
        UpdateDisplay();

        Debug.Log($"[HUD] Sorted — Action: {playerAction}, Correct: {correctAction}, Points: {points}, Score: {score}");
    }

    int CalculateScore(MailAction playerAction, MailAction correctAction)
    {
        if (playerAction == correctAction) return 10;
        if (correctAction == MailAction.Reply && playerAction == MailAction.Accept) return 5;
        if (correctAction == MailAction.Reject && playerAction == MailAction.Reply) return 5;
        if (correctAction == MailAction.Report && playerAction == MailAction.Reject) return 5;
        return 0;
    }

    public bool IsPassing() => score >= passThreshold;
    public int GetScore() => score;
    public int GetMistakes() => mistakes;
    public int GetTotalMails() => totalMails;

    public void SetDay(int day)
    {
        currentDay = day;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        dayText.text = $"Day {currentDay}";
        scoreText.text = $"Score: {score}";
    }
}