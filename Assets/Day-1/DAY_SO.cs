using UnityEngine;
using System.Collections.Generic;
using MailSorting.Data;

[CreateAssetMenu(fileName = "Day_X", menuName = "Mail Sorting/Day SO")]
public class DAY_SO : ScriptableObject
{
    [Header("Day Settings")]
    public int dayNumber = 1;
    public int dailyQuota = 15;
    public int passScoreThreshold = 100;
    public bool isTutorialDay = false;


    //[Header("Guaranteed — Characters")]
    //public Mail_Items_SO characterAMail;
    //public Mail_Items_SO characterBMail;
    //public Mail_Items_SO characterCMail;

    [Header("Guaranteed Story Mails")]
    public List<Mail_Items_SO> guaranteedMails = new List<Mail_Items_SO>();

    //[Space(10)]
    //[Header("RULE PARAMETERS (Used if rule is active)")]
    //public int maxSentencesAllowed = 4;
    //public Vector2 maxDimensions = new Vector2(30f, 20f);
    //public float maxWeightLimit = 1000f;
    //public int maxGiftValue = 50;
    public bool ExactlyOneQuestion = false;
}