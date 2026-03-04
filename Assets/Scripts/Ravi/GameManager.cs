using UnityEngine;
using System.Collections.Generic;
using MailSorting.Data;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Day Configs — in order")]
    public DayConfig_SO[] allDays;

    public int currentDayIndex = 0;
    public DayConfig_SO CurrentDay => allDays[currentDayIndex];

    [Header("Global Random Pool")]
    public List<Mail_SO> globalRandomPool;

    // character scores (hidden, keyed by CharacterSender enum)
    public Dictionary<CharacterSender, int> characterScores = new Dictionary<CharacterSender, int>();

    // carried over unsorted random mails
    public List<Mail_SO> carriedOverMail = new List<Mail_SO>();

    [HideInInspector] public List<Mail_SO> sortedToday = new List<Mail_SO>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void OnMailSorted(Mail_SO mail, int score)
    {
        sortedToday.Add(mail);

        if (mail.senderProfile != CharacterSender.Generic)
        {
            if (!characterScores.ContainsKey(mail.senderProfile))
                characterScores[mail.senderProfile] = 0;

            characterScores[mail.senderProfile] += score;
            Debug.Log($"[GM] {mail.senderProfile} score: {characterScores[mail.senderProfile]}");
        }
    }

    public void OnDayEnd(List<Mail_SO> allSpawnedRandom)
    {
        carriedOverMail.Clear();

        Dictionary<Mail_SO, int> sortedCounts = new Dictionary<Mail_SO, int>();
        foreach (var mail in sortedToday)
        {
            if (sortedCounts.ContainsKey(mail)) sortedCounts[mail]++;
            else sortedCounts[mail] = 1;
        }

        Dictionary<Mail_SO, int> spawnedCounts = new Dictionary<Mail_SO, int>();
        foreach (var mail in allSpawnedRandom)
        {
            if (spawnedCounts.ContainsKey(mail)) spawnedCounts[mail]++;
            else spawnedCounts[mail] = 1;
        }

        foreach (var kvp in spawnedCounts)
        {
            int sortedCount = sortedCounts.ContainsKey(kvp.Key) ? sortedCounts[kvp.Key] : 0;
            int unsorted = kvp.Value - sortedCount;
            for (int i = 0; i < unsorted; i++)
            {
                carriedOverMail.Add(kvp.Key);
                Debug.Log($"[GM] Carrying over: {kvp.Key.senderName}");
            }
        }

        Debug.Log($"[GM] Day {currentDayIndex + 1} ended. {carriedOverMail.Count} mails carried over.");
        sortedToday.Clear();
    }

    public void ProceedToNextDay()
    {
        currentDayIndex++;
        Debug.Log($"[GM] Moving to Day {currentDayIndex + 1}");
    }
}