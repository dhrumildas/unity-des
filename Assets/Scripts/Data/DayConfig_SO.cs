using UnityEngine;
using System.Collections.Generic;

namespace MailSorting.Data
{
    [CreateAssetMenu(fileName = "DayConfig_Day_X", menuName = "Mail Sorting/Day Config SO", order = 3)]

    public class DayConfig_SO : ScriptableObject
    {
        [Header("Day Settings")]
        [Range(1, 7)]
        public int dayNumber = 1;

        public int dailyMailQuota = 15;

        public int passScoreThreshold = 100;

        [Header("Guaranteed Pool")]
        public List<MailPoolEntry> guaranteedPool;
        [System.Serializable]
        public class MailPoolEntry
        {
            public Mail_Items_SO mailData;
        }

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
}
