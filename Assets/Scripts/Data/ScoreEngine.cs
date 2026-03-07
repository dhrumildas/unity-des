using UnityEngine;
using MailSorting.Data;

namespace MailSorting.Gameplay
{
    public class ScoreEngine : MonoBehaviour
    {
        public static ScoreEngine Instance;

        [Header("Shift Health Tracker")]
        public int currentDayBP = 0;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public int ProcessAction(MailAction playerAction, MailAction trueAction)
        {
            int points = 0;

            if (trueAction == MailAction.Accept)
            {
                if (playerAction == MailAction.Accept) points = 10;
                else if (playerAction == MailAction.Reply) points = 5;
            }
            else if (trueAction == MailAction.Reply)
            {
                if (playerAction == MailAction.Reply) points = 10;
                else if (playerAction == MailAction.Reject) points = 5;
            }
            else if (trueAction == MailAction.Reject)
            {
                if (playerAction == MailAction.Reject) points = 10;
                else if (playerAction == MailAction.Report) points = 5;
            }
            else if (trueAction == MailAction.Report)
            {
                if (playerAction == MailAction.Report) points = 10;
                else if (playerAction == MailAction.Reject) points = 5;
            }

            currentDayBP += points;
            return points;
        }

        public void ResetDailyScore()
        {
            currentDayBP = 0;
        }
    }
}