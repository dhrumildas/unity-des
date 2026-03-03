using UnityEngine;
using System.Collections.Generic;
using MailSorting.Data;

[CreateAssetMenu(fileName = "DayConfig", menuName = "Mail Sorting/Day Config")]
public class DayConfig : ScriptableObject
{
    [Header("Day Settings")]
    public int dayNumber = 1;
    public int totalMailCount = 10;
    public float spawnInterval = 20f;
    public float dayDuration = 200f;

    [Header("Guaranteed Mail")]
    [Tooltip("These always spawn first, in this exact order")]
    public List<Mail_Items_SO> guaranteedMail;

    [Header("Random Pool")]
    [Tooltip("These fill the remaining slots randomly")]
    public List<MailPoolEntry> randomPool;
}

[System.Serializable]
public class MailPoolEntry
{
    public Mail_Items_SO mailData;
    [Range(1, 10)]
    public int weight = 5;
}