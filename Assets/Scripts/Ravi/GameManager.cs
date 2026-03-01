using UnityEngine;
using System.Collections.Generic;
using MailSorting.Data;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Day Configs — in order")]
    public DayConfig[] allDays;

    // current day
    public int currentDayIndex = 0;
    public DayConfig CurrentDay => allDays[currentDayIndex];

    // character scores (hidden)
    public int characterAScore = 0;
    public int characterBScore = 0;
    public int characterCScore = 0;

    // carried over unsorted random mails
    public List<Mail_Items_SO> carriedOverMail = new List<Mail_Items_SO>();

    // track what spawned this day that is guaranteed
    [HideInInspector] public List<Mail_Items_SO> spawnedGuaranteedToday = new List<Mail_Items_SO>();
    [HideInInspector] public List<Mail_Items_SO> sortedToday = new List<Mail_Items_SO>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void OnGuaranteedSorted(Mail_Items_SO mail, int score)
    {
        sortedToday.Add(mail);

        DayConfig day = CurrentDay;

        // add to correct character score
        if (mail == day.characterAMail) characterAScore += score;
        else if (mail == day.characterBMail) characterBScore += score;
        else if (mail == day.characterCMail) characterCScore += score;

        Debug.Log($"[GM] Character scores — A:{characterAScore} B:{characterBScore} C:{characterCScore}");
    }

    public void OnRandomSorted(Mail_Items_SO mail)
    {
        sortedToday.Add(mail);
    }

    public void OnDayEnd(List<Mail_Items_SO> allSpawnedRandom)
    {
        carriedOverMail.Clear();

        // find unsorted random mails and carry them over
        foreach (var mail in allSpawnedRandom)
        {
            if (!sortedToday.Contains(mail))
            {
                carriedOverMail.Add(mail);
                Debug.Log($"[GM] Carrying over unsorted mail: {mail.mailID}");
            }
        }

        // guaranteed mails not sorted = score 0 (already handled by not calling OnGuaranteedSorted)
        Debug.Log($"[GM] Day {currentDayIndex + 1} ended. {carriedOverMail.Count} mails carried over.");

        // reset daily tracking
        spawnedGuaranteedToday.Clear();
        sortedToday.Clear();
    }

    public void ProceedToNextDay()
    {
        currentDayIndex++;
        // scene loading will go here later
        Debug.Log($"[GM] Moving to Day {currentDayIndex + 1}");
    }
}