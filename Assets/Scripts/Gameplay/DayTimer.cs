using UnityEngine;
using TMPro;

namespace MailSorting.Gameplay
{
    public class DayTimer : MonoBehaviour
    {
        [Tooltip("Total time for the shift in seconds (Day 1 is 120)")]
        public float shiftDurationSeconds = 120f;
        public TMP_Text timerText;

        private float timeRemaining;
        private bool isTimerRunning = false;

        void Start()
        {
            timeRemaining = shiftDurationSeconds;
            isTimerRunning = true;
        }

        void Update()
        {
            if (!isTimerRunning) return;

            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isTimerRunning = false;
                Debug.Log("Day is over! Time to show the grade screen.");
                // We will hook up the end-of-day grade screen trigger here later
            }

            UpdateTimerUI();
        }

        void UpdateTimerUI()
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60);
                int seconds = Mathf.FloorToInt(timeRemaining % 60);

                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

                // Add some tension: turn text red in the last 15 seconds
                if (timeRemaining <= 15f)
                {
                    timerText.color = Color.red;
                }
            }
        }
    }
}