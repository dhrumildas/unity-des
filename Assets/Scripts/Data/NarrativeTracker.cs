using UnityEngine;
using System.Collections.Generic;
using MailSorting.Data;
using System;

namespace MailSorting.Gameplay
{
    public class NarrativeTracker : MonoBehaviour
    {
        public static NarrativeTracker Instance;

        public class CharacterRecord
        {
            public int accepted;
            public int replied;
            public int rejected;
            public int reported;
        }

        public Dictionary<CharacterSender, CharacterRecord> ledger = new Dictionary<CharacterSender, CharacterRecord>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                IntitalizeLedger();
            }
            else
                Destroy(gameObject);
        }

        private void IntitalizeLedger()
        {
            ledger[CharacterSender.Katsuki] = new CharacterRecord();
            ledger[CharacterSender.Florian] = new CharacterRecord();
            ledger[CharacterSender.UnnamedStalker] = new CharacterRecord();
        }

        public void LogAction(CharacterSender sender, MailAction action)
        {
            if (sender == CharacterSender.Generic)
                return;
            if(!ledger.ContainsKey(sender))
            {
                Debug.LogError($"Trying to log action for {sender} but they are not in the ledger");
                return;
            }

            switch (action)
            {
                case MailAction.Accept:
                    ledger[sender].accepted++;
                    break;
                case MailAction.Reject:
                    ledger[sender].rejected++;
                    break;
                case MailAction.Reply:
                    ledger[sender].replied++;
                    break;
                case MailAction.Report:
                    ledger[sender].reported++;
                    break;
            }

            Debug.Log($"[NarrativeTracker] Logged {action} for {sender}. (Total Accepts so far: {ledger[sender].accepted}, Total Replies: {ledger[sender].replied}, Total Rejects: {ledger[sender].rejected}, Total Reports: {ledger[sender].reported})");
        }
    }
}
