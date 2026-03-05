using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [Header("Time Settings")]
    public float realDayDuration = 180f;
    public int gameStartHour = 10;
    public int gameEndHour = 18;

    [Header("UI")]
    public TMP_Text clockText;

    private float elapsed = 0f;
    private bool running = false;

    public bool IsDayOver => elapsed >= realDayDuration;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartTimer();
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
            if (DayEndPanel.Instance != null)
                DayEndPanel.Instance.Show();
        }
        UpdateClockDisplay();
    }

    void UpdateClockDisplay()
    {
        float t = elapsed / realDayDuration;
        float totalGameMinutes = t * (gameEndHour - gameStartHour) * 60f;
        int hours = gameStartHour + Mathf.FloorToInt(totalGameMinutes / 60f);
        int minutes = Mathf.FloorToInt(totalGameMinutes % 60f);
        string period = hours >= 12 ? "PM" : "AM";
        int displayHour = hours > 12 ? hours - 12 : hours;
        clockText.text = $"{displayHour:00}:{minutes:00} {period}";
    }
}