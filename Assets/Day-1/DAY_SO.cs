using UnityEngine;
using System.Collections.Generic;
using MailSorting.Data;

[CreateAssetMenu(fileName = "Day_X", menuName = "Mail Sorting/Day SO")]
public class DAY_SO: ScriptableObject
{
    [Header("Day Settings")]
    public int dayNumber = 1;
    public int dailyQuota = 15;
    public int passScoreThreshold = 100;
    public bool isTutorialDay = false;

    [Header("Tutorial (Day 1 only)")]
    public Mail_Items_SO tutorialAccept;
    public Mail_Items_SO tutorialReply;
    public Mail_Items_SO tutorialReject;
    public Mail_Items_SO tutorialReport;

    [Header("Guaranteed — Characters")]
    public Mail_Items_SO characterAMail;
    public Mail_Items_SO characterBMail;
    public Mail_Items_SO characterCMail;

    [Header("Guaranteed Story Mails")]
    [Tooltip("Add the required story mails for this day here.")]
    public List<Mail_Items_SO> guaranteedMails = new List<Mail_Items_SO>();

    [Space(10)]
    [Header("RULE SET")]
    public bool checkContraband = false;
    public bool checkAddress = false;
    public bool checkSignature = false;

    [Space(5)]
    public bool checkSentences = false;
    public int maxSentencesAllowed = 4;

    public bool checkDimensions = false;
    public Vector2 maxDimensions = new Vector2(30f, 20f);

    //public bool checkCountryPolicy = false;
    //public List<Country> bannedCountries = new List<Country>();

    public bool checkWeight = false;
    public float maxWeightLimit = 1000f;

    //public bool checkPostage = false;
    //public PostageType requiredPostage = PostageType.Standard;

    public bool checkGiftValue = false;
    public int maxGiftValue = 50;

    public bool checkQuestion = false;
    public bool ExactlyOneQuestion = false;
}
