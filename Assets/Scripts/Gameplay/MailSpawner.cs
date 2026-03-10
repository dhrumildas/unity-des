using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MailSorting.Data;

namespace MailSorting.Gameplay
{
    public class MailSpawner : MonoBehaviour
    {
        [Header("Day Config")]
        public DAY_SO dayConfig;

        [Header("Prefabs")]
        public GameObject letterPrefab;
        public GameObject packagePrefab;

        [Header("Spawn Points")]
        public Transform letterSpawnPoint;
        public Transform packageSpawnPoint;

        private List<Mail_Items_SO> mainQueue = new List<Mail_Items_SO>();
        private GameObject currentMailObject;
        private bool waitingForSort = false;

        void Start()
        {
            BuildQueue();
            StartCoroutine(SpawnLoop());

            if (GameTimer.Instance != null)
                GameTimer.Instance.StartTimer();
        }

        //private void BuildQueue()
        //{
        //    mainQueue.Clear();

        //    if (dayConfig == null)
        //    {
        //        Debug.LogError("[MailSpawner] No DayConfig assigned!");
        //        return;
        //    }

        //    Debug.Log($"[MailSpawner] randomPool entries in DayConfig: {dayConfig.randomPool.Count}");

        //    // Build weighted random pool
        //    List<Mail_Items_SO> randomPool = new List<Mail_Items_SO>();
        //    foreach (var entry in dayConfig.randomPool)
        //    {
        //        Debug.Log($"Entry: {entry?.mailData?.mailID} | weight: {entry?.weight}");
        //        if (entry.mailData == null) continue;
        //        int w = entry.weight <= 0 ? 1 : entry.weight;
        //        for (int i = 0; i < w; i++)
        //            randomPool.Add(entry.mailData);
        //    }

        //    Debug.Log($"[MailSpawner] Random pool built: {randomPool.Count} entries");

        //    // Shuffle random pool
        //    for (int i = randomPool.Count - 1; i > 0; i--)
        //    {
        //        int j = Random.Range(0, i + 1);
        //        (randomPool[i], randomPool[j]) = (randomPool[j], randomPool[i]);
        //    }

        //    // Guaranteed mails
        //    List<Mail_Items_SO> guaranteed = new List<Mail_Items_SO>();
        //    if (dayConfig.characterAMail != null) guaranteed.Add(dayConfig.characterAMail);
        //    if (dayConfig.characterBMail != null) guaranteed.Add(dayConfig.characterBMail);
        //    if (dayConfig.characterCMail != null) guaranteed.Add(dayConfig.characterCMail);
        //    if (dayConfig.generalGuaranteedMail != null) guaranteed.Add(dayConfig.generalGuaranteedMail);

        //    // Interleave guaranteed every 3rd slot
        //    int gIdx = 0;
        //    for (int i = 0; i < randomPool.Count; i++)
        //    {
        //        if (i % 3 == 0 && gIdx < guaranteed.Count)
        //            mainQueue.Add(guaranteed[gIdx++]);
        //        mainQueue.Add(randomPool[i]);
        //    }
        //    while (gIdx < guaranteed.Count)
        //        mainQueue.Add(guaranteed[gIdx++]);

        //    Debug.Log($"[MailSpawner] Day {dayConfig.dayNumber} — {mainQueue.Count} mails queued.");
        //}

        private void BuildQueue()
        {
            mainQueue.Clear();

            if(dayConfig == null)
            {
                Debug.Log("No day config assigned");
                return;
            }

            int quota = dayConfig.dailyQuota;
            List<Mail_Items_SO> guaranteed = new List<Mail_Items_SO>(dayConfig.guaranteedMails);
            int guaranteedCount = guaranteed.Count;

            if(guaranteedCount == 0)
            {
                for (int i = 0; i < quota; i++)
                {
                    mainQueue.Add(MailDatabase.Instance.GetRandomMail());
                }
                return;
            }



            int mailsPerChunk = quota / guaranteedCount;
            int remainder = quota % guaranteedCount; // it wont always divide evenly
            for (int i =0; i < guaranteedCount;  i++)
            {
                List<Mail_Items_SO> currentBracket = new List<Mail_Items_SO>();
                currentBracket.Add(guaranteed[i]);
                int bracketRandomCount = mailsPerChunk - 1;   //1 for the guaranteed

                //check if this is the last bracket
                if(i == guaranteedCount - 1)
                {
                    bracketRandomCount += remainder;
                }

                for(int r = 0; r <bracketRandomCount; r++)
                {
                    Mail_Items_SO randomMail = MailDatabase.Instance.GetRandomMail();
                    if(randomMail != null)
                    {
                        currentBracket.Add(randomMail);
                        //Note : This doesn't remove it from the Master Pool. Prolly an even would do it
                    }
                }

                for (int j = currentBracket.Count - 1; j >= 0; j--)
                {
                    int k = Random.Range(0, j + 1);
                    (currentBracket[j], currentBracket[k]) = (currentBracket[k],  currentBracket[j]);
                }
                mainQueue.AddRange(currentBracket);
            }
            Debug.Log($"[MailSpawner] Day {dayConfig.dayNumber} Queue Built. Total: {mainQueue.Count}. Chunks: {guaranteedCount}");
        }

        private IEnumerator SpawnLoop()
        {
            // Tutorial first on Day 1
            if (dayConfig.isTutorialDay)
            {
                List<Mail_Items_SO> tutorial = new List<Mail_Items_SO>();
                if (dayConfig.tutorialAccept != null) tutorial.Add(dayConfig.tutorialAccept);
                if (dayConfig.tutorialReply != null) tutorial.Add(dayConfig.tutorialReply);
                if (dayConfig.tutorialReject != null) tutorial.Add(dayConfig.tutorialReject);
                if (dayConfig.tutorialReport != null) tutorial.Add(dayConfig.tutorialReport);

                foreach (var mail in tutorial)
                {
                    Debug.Log($"[MailSpawner] Tutorial: {mail.mailID}");
                    waitingForSort = true;
                    SpawnMail(mail);
                    yield return new WaitUntil(() => !waitingForSort);
                }
                Debug.Log("[MailSpawner] Tutorial complete.");
            }

            // Main queue
            Debug.Log($"[MailSpawner] Main queue count: {mainQueue.Count}");
            foreach (var mail in mainQueue)
            {
                Debug.Log($"[MailSpawner] Spawning: {mail.mailID}");
                waitingForSort = true;
                SpawnMail(mail);
                Debug.Log($"[MailSpawner] Waiting for sort...");
                yield return new WaitUntil(() => !waitingForSort);
                Debug.Log($"[MailSpawner] Sorted! Moving to next.");
            }

            Debug.Log("[MailSpawner] All mail delivered for today.");
        }

        private void SpawnMail(Mail_Items_SO data)
        {
            bool isPackage = data.mailType == MailType.Package;
            Transform spawnPoint = isPackage ? packageSpawnPoint : letterSpawnPoint;
            GameObject prefab = isPackage ? packagePrefab : letterPrefab;

            if (spawnPoint == null || prefab == null)
            {
                Debug.LogWarning($"[MailSpawner] Missing spawn point or prefab for {data.mailID} — skipping.");
                waitingForSort = false;
                return;
            }

            currentMailObject = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            currentMailObject.transform.SetParent(letterSpawnPoint, false);

            // Pass data to drag component
            UI_DragCheck drag = currentMailObject.GetComponent<UI_DragCheck>();
            if (drag != null)
                drag.mailData = data;

            // Set envelope sprite
            Image img = currentMailObject.GetComponent<Image>();
            if (img != null && data.mailSprite != null)
                img.sprite = data.mailSprite;

            // Initialise envelope
            EnvelopeObject envelope = currentMailObject.GetComponent<EnvelopeObject>();
            if (envelope != null)
                envelope.Initialise(data);

            // Tell action buttons
            if (ActionButtonController.Instance != null)
                ActionButtonController.Instance.SetCurrentMail(data);
        }

        public void ProcessNextMail()
        {
            if (currentMailObject != null)
            {
                EnvelopeObject envelope = currentMailObject.GetComponent<EnvelopeObject>();
                if (envelope != null)
                    envelope.DestroyLetter();

                Destroy(currentMailObject);
            }

            waitingForSort = false;
        }
    }
}