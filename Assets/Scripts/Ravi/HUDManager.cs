using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("UI References")]
    public TMP_Text dayText;
    public TMP_Text scoreText;

    private int score = 0;
    private int currentDay = 1;
    private int mistakes = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateDisplay();
        GameTimer.Instance.StartTimer();
    }

    public void OnCorrectSort()
    {
        score += 100;
        UpdateDisplay();
    }

    public void OnWrongSort()
    {
        mistakes++;
        Debug.Log($"[HUD] Mistake #{mistakes} recorded silently");
    }

    public int GetScore() => score;
    public int GetMistakes() => mistakes;

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