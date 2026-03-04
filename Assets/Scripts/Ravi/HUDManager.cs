using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("UI References")]
    public TMP_Text dayText;
    public TMP_Text scoreText;

    [Header("Quota Settings")]
    public int passThreshold = 50; // Changable in Inspector

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

    public void OnMailSorted(MailSorting.Data.MailAction playerAction, MailSorting.Data.MailAction correctAction)
    {
        totalMails++;

        int points = CalculateScore(playerAction, correctAction);

        if (points == 10)
        {
            comboCount++;
            if (comboCount > 1)
            {
                int comboBonus = (comboCount - 1) * 2; // +2 per consecutive correct
                points += comboBonus;
                Debug.Log($"[HUD] Combo x{comboCount}! +{comboBonus} bonus");
            }
        }
        else
        {
            comboCount = 0; // reset combo on partial or wrong
            if (points == 0) mistakes++;
        }

        score += points;
        UpdateDisplay();

        Debug.Log($"[HUD] Sorted — Action: {playerAction}, Correct: {correctAction}, Points: {points}, Score: {score}");
    }

    int CalculateScore(MailSorting.Data.MailAction playerAction, MailSorting.Data.MailAction correctAction)
    {
        // correct sort
        if (playerAction == correctAction) return 10;

        // partial sorts
        if (correctAction == MailSorting.Data.MailAction.Reply && playerAction == MailSorting.Data.MailAction.Accept) return 5;
        if (correctAction == MailSorting.Data.MailAction.Reject && playerAction == MailSorting.Data.MailAction.Reply) return 5;
        if (correctAction == MailSorting.Data.MailAction.Report && playerAction == MailSorting.Data.MailAction.Reject) return 5;

        // wrong sort
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