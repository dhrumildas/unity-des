using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayEndPanel : MonoBehaviour
{
    public static DayEndPanel Instance;

    [Header("Panel")]
    public GameObject panelRoot;

    [Header("Text Fields")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI quotaText;
    public TextMeshProUGUI resultText;

    [Header("Buttons")]
    public Button continueButton;
    public Button gameOverButton;

    [Header("Settings")]
    public int dailyQuota = 20;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        panelRoot.SetActive(false);
    }

    public void Show()
    {
        // Destroy all active mail objects on the canvas
        foreach (var envelope in FindObjectsByType<EnvelopeObject>(FindObjectsSortMode.None))
        {
            envelope.DestroyLetter();
            Destroy(envelope.gameObject);
        }

        foreach (var package in FindObjectsByType<PackageObject>(FindObjectsSortMode.None))
        {
            package.DestroyContents();
            Destroy(package.gameObject);
        }

        // Destroy any orphaned letter or item objects
        foreach (var letter in FindObjectsByType<LetterObject>(FindObjectsSortMode.None))
            Destroy(letter.gameObject);

        panelRoot.SetActive(true);

        int score = ScoreManager.Instance.GetTotalScore();
        bool passed = score >= dailyQuota;

        scoreText.text = $"Score: {score}";
        quotaText.text = $"Quota: {dailyQuota} — {(passed ? "PASSED" : "FAILED")}";
        resultText.text = passed
            ? "Good work. Aurora's mail is sorted."
            : "You didn't meet today's quota.";

        continueButton.gameObject.SetActive(passed);
        gameOverButton.gameObject.SetActive(!passed);

        if (GameTimer.Instance != null)
            GameTimer.Instance.enabled = false;
    }

    public void OnContinueClicked()
    {
        int score = ScoreManager.Instance.GetTotalScore();
        GameManager.Instance.OnDayComplete(score);
        ScoreManager.Instance.ResetDay();
        GameManager.Instance.LoadNextDay();
    }

    public void OnGameOverClicked()
    {
        GameManager.Instance.TriggerGameOver();
    }
}