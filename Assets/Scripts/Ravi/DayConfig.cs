using UnityEngine;
using System.Collections.Generic;
using MailSorting.Data;

[CreateAssetMenu(fileName = "DayConfig", menuName = "Mail Sorting/Day Config")]
public class DayConfig : ScriptableObject
{
    [Header("Day Settings")]
    public int dayNumber = 1;
    public float dayDuration = 180f;

    [Header("Tutorial (Day 1 only)")]
    public Mail_Items_SO tutorialAccept;
    public Mail_Items_SO tutorialReply;
    public Mail_Items_SO tutorialReject;
    public Mail_Items_SO tutorialReport;

    [Header("Guaranteed — Characters")]
    public Mail_Items_SO characterAMail;
    public Mail_Items_SO characterBMail;
    public Mail_Items_SO characterCMail;

    [Header("Guaranteed — General")]
    public Mail_Items_SO generalGuaranteedMail;

    [Header("Random Pool")]
    public List<MailPoolEntry> randomPool;
}

[System.Serializable]
public class MailPoolEntry
{
    public Mail_Items_SO mailData;
    [Range(1, 10)]
    public int weight = 5;
}