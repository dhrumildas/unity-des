using UnityEngine;
using UnityEngine.SceneManagement;
using MailSorting.Data;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Day Settings")]
    public int currentDay = 1;
    public int totalDays = 7;

    [Header("Day Configs")]
    public DayConfig[] dayConfigs; // assign all 7 days in Inspector

    [Header("Ending Thresholds")]
    public int passingScorePerDay = 50;

    // Tracks cumulative score across all days
    private int cumulativeScore = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public DayConfig GetCurrentDayConfig()
    {
        if (dayConfigs == null || dayConfigs.Length == 0) return null;
        int idx = currentDay - 1;
        if (idx < 0 || idx >= dayConfigs.Length) return null;
        return dayConfigs[idx];
    }

    public void OnDayComplete(int dayScore)
    {
        cumulativeScore += dayScore;
        bool passed = dayScore >= passingScorePerDay;

        Debug.Log($"[GameManager] Day {currentDay} complete. Score: {dayScore}. Passed: {passed}");

        if (!passed)
        {
            TriggerGameOver();
            return;
        }

        if (currentDay >= totalDays)
        {
            TriggerEnding();
            return;
        }

        currentDay++;
    }

    public void LoadNextDay()
    {
        // Reload same scene — MailSpawner will pick up new DayConfig
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TriggerGameOver()
    {
        Debug.Log("[GameManager] Game Over!");
        // TODO: load game over scene
    }

    public void TriggerEnding()
    {
        Debug.Log($"[GameManager] All 7 days complete! Cumulative score: {cumulativeScore}");
        // TODO: check NarrativeTracker records to determine which ending
        // NarrativeTracker.Instance.GetRecord(CharacterSender.Katsuki) etc.
    }

    public int GetCumulativeScore() => cumulativeScore;
    public int GetCurrentDay() => currentDay;
}