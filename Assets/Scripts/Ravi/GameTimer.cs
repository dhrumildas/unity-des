using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [Header("Time Settings")]
    public float realDayDuration = 180f; // 3 mins real time
    public int gameStartHour = 10;       // 10:00 AM
    public int gameEndHour = 18;         // 6:00 PM

    [Header("UI")]
    public TMP_Text clockText;

    private float elapsed = 0f;
    private bool running = false;
    public bool IsDayOver => elapsed >= realDayDuration;

    void Awake()
    {
        Instance = this;
    }

    public void StartTimer()
    {
        elapsed = 0f;
        running = true;
    }

    void Update()
    {
        if (!running || IsDayOver) return;

        elapsed += Time.deltaTime;

        if (IsDayOver)
        {
            running = false;
            DayEndPanel.Instance.Show();
        }

        UpdateClockDisplay();
    }

    void UpdateClockDisplay()
    {
        // map elapsed real time to game hours
        float t = elapsed / realDayDuration;
        float totalGameMinutes = t * (gameEndHour - gameStartHour) * 60f;

        int hours = gameStartHour + Mathf.FloorToInt(totalGameMinutes / 60f);
        int minutes = Mathf.FloorToInt(totalGameMinutes % 60f);

        string period = hours >= 12 ? "PM" : "AM";
        int displayHour = hours > 12 ? hours - 12 : hours;

        clockText.text = $"{displayHour:00}:{minutes:00} {period}";
    }
}