using UnityEngine;
using MailSorting.Data;

namespace MailSorting.Gameplay
{
    public class RuleVaildator : MonoBehaviour
    {
        public static RuleVaildator Instance;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public MailAction EvaluateMail(Mail_Items_SO mail, DayConfig_SO currentDay)
        {
            if (currentDay.checkContraband)
            {
                if (mail.containsContraband || mail.containsSubstance || mail.containsOffence)
                {
                    return MailAction.Report;
                }
            }
            int violations = 0;
            if (currentDay.checkAddress && !mail.addressedCorrectly) violations++;
            if (currentDay.checkSignature && !mail.addressedCorrectly)violations++;
            if (currentDay.checkSentences && mail.sentenceCount > currentDay.maxSentencesAllowed) violations++;

            //next rule added : dimensions
            if(currentDay.checkDimensions)
            {
                if(mail.dimensions.x > currentDay.maxDimensions.x || mail.dimensions.y > currentDay.maxDimensions.y)
                    violations++;
            }

            //next rule added : weight
            if(currentDay.checkWeight && mail.weight > currentDay.maxWeightLimit)
                violations++;

            //next rule added : gift value
            if( currentDay.checkGiftValue && mail.giftValue > currentDay.maxGiftValue)
                violations++;

            //next rule addded : question
            if (currentDay.checkQuestion)
            {
                if (mail.containsQuestion && mail.questionCount > 1)
                    violations++;
            }

            if (violations == 0) return MailAction.Accept;
            if (violations == 1) return MailAction.Reply;

            return MailAction.Reject;
        }
    }
}
