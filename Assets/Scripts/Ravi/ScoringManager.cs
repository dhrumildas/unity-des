using UnityEngine;
using MailSorting.Data;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int totalScore = 0;
    private int mailsProcessed = 0;
    private int correctDecisions = 0;
    private int incorrectDecisions = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void RegisterDecision(Mail_Items_SO mail, MailAction playerAction)
    {
        mailsProcessed++;

        MailAction correctAction = DetermineCorrectAction(mail);
        int points = CalculatePoints(correctAction, playerAction);
        totalScore += points;

        if (playerAction == correctAction) correctDecisions++;
        else incorrectDecisions++;

        Debug.Log($"[ScoreManager] Player: {playerAction} | Correct: {correctAction} | +{points} pts | Total: {totalScore}");
    }

    private int CalculatePoints(MailAction correctAction, MailAction playerAction)
    {
        if (playerAction == correctAction) return 10;

        switch (correctAction)
        {
            case MailAction.Reply:
                return playerAction == MailAction.Reject ? 5 : 0;
            case MailAction.Reject:
                return playerAction == MailAction.Reply ? 5 : 0;
            case MailAction.Report:
                return playerAction == MailAction.Reject ? 5 : 0;
            case MailAction.Accept:
                return 0;
            default:
                return 0;
        }
    }

    public MailAction DetermineCorrectAction(Mail_Items_SO mail)
    {
        // Report — highest priority
        if (mail.containsContraband || mail.containsSubstance ||
            mail.containsMetal || mail.containsOffence)
            return MailAction.Report;

        // Count violations
        int violations = 0;
        if (!mail.addressedCorrectly) violations++;
        if (!mail.signedCorrectly) violations++;
        if (mail.sentenceCount > 3) violations++;
        if (mail.postageType == PostageType.Fake) violations++;

        // Question = needs reply
        if (mail.containsQuestion) return MailAction.Reply;

        if (violations == 0) return MailAction.Accept;
        if (violations == 1) return MailAction.Reply;
        return MailAction.Reject;
    }

    public int GetTotalScore() => totalScore;
    public int GetCorrect() => correctDecisions;
    public int GetIncorrect() => incorrectDecisions;
    public int GetMailsProcessed() => mailsProcessed;

    public void ResetDay()
    {
        totalScore = 0;
        mailsProcessed = 0;
        correctDecisions = 0;
        incorrectDecisions = 0;
    }
}