using MailSorting.Data;
using TMPro;
using UnityEngine;

public class BulletinBoard : MonoBehaviour
{
    [Header("Room State")]
    public GameObject instructionPanel;

    public GameObject bulletinBoardContainer;

    [Header("Poster Spawning")]
    public GameObject posterPrefab;

    public Transform posterGridParent;

    [Header("Upcoming Shift")]
    public DAY_SO upcomingShiftConfig;

    void Start()
    {
        SetupRoom();
    }

    private void SetupRoom()
    {

        int isTutorialFinished = PlayerPrefs.GetInt("TutorialFinished", 0);

        //isTutorialFinished = 1;
        if (isTutorialFinished == 0)
        {

            instructionPanel.SetActive(true);
            bulletinBoardContainer.SetActive(false);

        }
        else
        {

            instructionPanel.SetActive(false);

            bulletinBoardContainer.SetActive(true);

            PopulateBulletinBoard();
        }
    }

    private void PopulateBulletinBoard()
    {
        if (ScoreManager.Instance == null) return;

        ScoreManager.Instance.currentDayConfig = upcomingShiftConfig;
        ScoreManager.Instance.GenerateRandomLimits();

        MailRules? newlyDraftedRule = null;

        if (ScoreManager.Instance.activeRules.Count == 0)
        {
            ScoreManager.Instance.InitializeDayOneRules();
        }
        else
        {
            newlyDraftedRule = ScoreManager.Instance.DraftNewRule();
        }

        foreach (Transform child in posterGridParent)
        {
            Destroy(child.gameObject);
        }

        foreach (MailRules rule in ScoreManager.Instance.activeRules)
        {

            GameObject newPoster = Instantiate(posterPrefab, posterGridParent);

            TextMeshProUGUI posterText = newPoster.GetComponentInChildren<TextMeshProUGUI>();
            string ruleDescription = ScoreManager.Instance.GetRuleDescription(rule);

            if (newlyDraftedRule.HasValue && rule == newlyDraftedRule.Value)
            {
                posterText.text = $"<color=#AA0000><b>[NEW]\n{ruleDescription}</b></color>";
            }
            else
            {
                posterText.text = ruleDescription;
            }
        }
    }
}
