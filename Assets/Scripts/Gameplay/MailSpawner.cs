using System.Collections.Generic;
using UnityEngine;
using MailSorting.Data;
using MailSorting.UI;

namespace MailSorting.Gameplay
{
    public class MailSpawner : MonoBehaviour
    {
        [Header("Mail Pools (Day 1)")]
        [Tooltip("Guaranteed story letters (e.g., Katsuki, Florian)")]
        public Mail_Items_SO[] guaranteedMail;
        [Tooltip("Generic filler mail (randomly shuffled)")]
        public Mail_Items_SO[] randomPool;

        [Header("Scene References")]
        public Transform letterSpawnPoint;
        public Transform packageSpawnPoint;
        public GameObject draggableMailPrefab;
        public MailInspectionManager inspectionManager;

        private Queue<Mail_Items_SO> mailQueue = new Queue<Mail_Items_SO>();
        private GameObject currentMailObject;

        void Start()
        {
            BuildQueue();

            // Listen for when the player makes a decision so we know to spawn the next one
            if (inspectionManager != null)
            {
                inspectionManager.OnMailActioned += HandleMailActioned;
            }

            SpawnNextMail();
        }

        private void BuildQueue()
        {
            // 1. Shuffle the random pool into a list
            List<Mail_Items_SO> randomList = new List<Mail_Items_SO>(randomPool);
            for (int i = 0; i < randomList.Count; i++)
            {
                Mail_Items_SO temp = randomList[i];
                int randomIndex = Random.Range(i, randomList.Count);
                randomList[i] = randomList[randomIndex];
                randomList[randomIndex] = temp;
            }

            // 2. Interleave the guaranteed story mail so they don't all spawn back-to-back
            int guaranteedIndex = 0;
            for (int i = 0; i < randomList.Count; i++)
            {
                // Inject a story letter every 3rd piece of mail
                if (i % 3 == 0 && guaranteedIndex < guaranteedMail.Length)
                {
                    mailQueue.Enqueue(guaranteedMail[guaranteedIndex]);
                    guaranteedIndex++;
                }
                mailQueue.Enqueue(randomList[i]);
            }

            // 3. Add any leftover guaranteed mail to the end just in case
            while (guaranteedIndex < guaranteedMail.Length)
            {
                mailQueue.Enqueue(guaranteedMail[guaranteedIndex]);
                guaranteedIndex++;
            }
        }

        private void SpawnNextMail()
        {
            if (mailQueue.Count == 0)
            {
                Debug.Log("Shift over! The mail queue is empty.");
                return;
            }

            Mail_Items_SO nextMail = mailQueue.Dequeue();

            // Letters spawn on the right, packages on the left
            Transform spawnPos = (nextMail.mailType == MailType.Letter) ? letterSpawnPoint : packageSpawnPoint;

            currentMailObject = Instantiate(draggableMailPrefab, spawnPos.position, Quaternion.identity);

            // Pass the SO data to the drag script so the inspection zone can read it
            DragCheck dragComponent = currentMailObject.GetComponent<DragCheck>();
            if (dragComponent != null)
            {
                dragComponent.mailData = nextMail;
            }

            // Set the correct visual sprite on the desk
            SpriteRenderer sr = currentMailObject.GetComponent<SpriteRenderer>();
            if (sr != null && nextMail.mailSprite != null)
            {
                sr.sprite = nextMail.mailSprite;
            }
        }

        private void HandleMailActioned(Mail_Items_SO mail, MailAction action, bool isCorrect)
        {
            // The player made a choice! Destroy the mail on the desk.
            if (currentMailObject != null)
            {
                Destroy(currentMailObject);
            }

            // Wait 1 second before spawning the next one so the player has time to read the feedback popup
            Invoke(nameof(SpawnNextMail), 1.0f);
        }

        private void OnDestroy()
        {
            // Always un-subscribe from events to prevent memory leaks
            if (inspectionManager != null)
            {
                inspectionManager.OnMailActioned -= HandleMailActioned;
            }
        }
    }
}