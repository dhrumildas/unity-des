using System.Collections.Generic;
using MailSorting.Data;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public DAY_SO currentDayConfig;
    public List<MailRules> activeRules = new List<MailRules>();
    public List<MailRules> inactiveRules = new List<MailRules>();
    private int totalScore = 0;
    private int mailsProcessed = 0;
    private int correctDecisions = 0;
    private int incorrectDecisions = 0;

    [Header("Character Scores")]
    public int katsukiScore = 0;
    public int florianScore = 0;
    public int stalkerScore = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void InitializeDayOneRules()
    {
        activeRules.Clear();
        inactiveRules.Clear();

        // 1. The 4 Constant Starting Rules
        activeRules.Add(MailRules.Address);
        activeRules.Add(MailRules.Signature);
        activeRules.Add(MailRules.Contraband);
        activeRules.Add(MailRules.Sentences);

        // 2. The Draft Pool
        inactiveRules.Add(MailRules.Weight);
        inactiveRules.Add(MailRules.Dimension);
        inactiveRules.Add(MailRules.GiftValue);
        inactiveRules.Add(MailRules.Questions);
        inactiveRules.Add(MailRules.Postage);
        inactiveRules.Add(MailRules.Country);

        Debug.Log("[ScoreManager] Day 1 Rules Initialized.");
    }

    public MailRules? DraftNewRule()
    {
        if (inactiveRules.Count == 0) return null; // No more rules to draft!

        int randomIndex = Random.Range(0, inactiveRules.Count);
        MailRules draftedRule = inactiveRules[randomIndex];

        inactiveRules.RemoveAt(randomIndex);
        activeRules.Add(draftedRule);

        Debug.Log($"[ScoreManager] NEW RULE DRAFTED: {draftedRule}");
        return draftedRule;
    }

    public void RegisterDecision(Mail_Items_SO mail, MailAction playerAction)
    {
        mailsProcessed++;

        MailAction correctAction = DetermineCorrectAction(mail);
        int points = CalculatePoints(correctAction, playerAction);
        totalScore += points;

        if (playerAction == correctAction) correctDecisions++;
        else incorrectDecisions++;

        if (mail.senderProfile != CharacterSender.Generic)
        {
            switch (mail.senderProfile)
            {
                case CharacterSender.Katsuki:
                    katsukiScore += points;
                    break;
                case CharacterSender.Florian:
                    florianScore += points;
                    break;
                case CharacterSender.UnnamedStalker:
                    stalkerScore += points;
                    break;
            }
        }

        Debug.Log($"[ScoreManager] Player: {playerAction} | Correct: {correctAction} | +{points} pts | Total: {totalScore}");
    }

    private int CalculatePoints(MailAction correctAction, MailAction playerAction)
    {
        if (playerAction == correctAction) return 10;

        if (correctAction == MailAction.Accept && playerAction == MailAction.Reply) return 5;
        if (correctAction == MailAction.Reply && playerAction == MailAction.Reject) return 5;
        if (correctAction == MailAction.Reject && playerAction == MailAction.Report) return 5;
        if (correctAction == MailAction.Report && playerAction == MailAction.Reject) return 5;

        return 0;
    }

    public MailAction DetermineCorrectAction(Mail_Items_SO mail)
    {
        if (currentDayConfig == null) return MailAction.Accept;

        // Priority: Contraband Check
        if (activeRules.Contains(MailRules.Contraband))
        {
            if (mail.containsContraband || mail.containsSubstance || mail.containsMetal || mail.containsOffence)
                return MailAction.Report;
        }

        int violations = 0;

        // Dynamic Rule Checks
        if (activeRules.Contains(MailRules.Address) && !mail.addressedCorrectly) violations++;
        if (activeRules.Contains(MailRules.Signature) && !mail.signedCorrectly) violations++;
        if (activeRules.Contains(MailRules.Sentences) && mail.sentenceCount > currentDayConfig.maxSentencesAllowed) violations++;
        if (activeRules.Contains(MailRules.Weight) && mail.weight > currentDayConfig.maxWeightLimit) violations++;
        if (activeRules.Contains(MailRules.GiftValue) && mail.giftValue > currentDayConfig.maxGiftValue) violations++;

        if (activeRules.Contains(MailRules.Dimension))
        {
            if (mail.dimensions.x > currentDayConfig.maxDimensions.x || mail.dimensions.y > currentDayConfig.maxDimensions.y)
                violations++;
        }

        if (activeRules.Contains(MailRules.Questions))
        {
            if (currentDayConfig.ExactlyOneQuestion && mail.questionCount != 1) violations++;
            else if (!currentDayConfig.ExactlyOneQuestion && mail.questionCount > 1) violations++;
        }

        // We can add logic for Postage and Country later once you define how those rules work!

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
        //totalScore = 0;
        mailsProcessed = 0;
        correctDecisions = 0;
        incorrectDecisions = 0;
    }
}