using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MailSorting.Data;

namespace MailSorting.UI
{
    /// <summary>
    /// Top-level manager that decides which inspection panel to open,
    /// handles the action buttons, report panel, and feedback popup.
    /// Attach to: InspectionManager GameObject.
    /// </summary>
    public class MailInspectionManager : MonoBehaviour
    {
        [Header("Inspection Panels")]
        [SerializeField] private LetterInspectionUI letterInspectionUI;
        [SerializeField] private PackageInspectionUI packageInspectionUI;

        [Header("Action Buttons")]
        [SerializeField] private GameObject actionButtonsPanel;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button replyButton;
        [SerializeField] private Button rejectButton;
        [SerializeField] private Button reportButton;

        [Header("Report Panel")]
        [SerializeField] private GameObject reportPanel;
        [SerializeField] private Toggle contrabandToggle;
        [SerializeField] private TMP_Dropdown contrabandTypeDropdown;
        [SerializeField] private Toggle substanceToggle;
        [SerializeField] private TMP_Dropdown substanceTypeDropdown;
        [SerializeField] private TMP_Dropdown otherOffenceDropdown;
        [SerializeField] private Button submitReportButton;
        [SerializeField] private Button cancelReportButton;

        [Header("Feedback Popup")]
        [SerializeField] private GameObject feedbackPopup;
        [SerializeField] private TMP_Text feedbackText;
        [SerializeField] private TMP_Text scoreChangedText;
        //[SerializeField] private float feedbackDuration = 1.5f;

        // State
        private Mail_Items_SO currentMail;
        private GameObject currentMailObject;
        private bool isInspecting = false;

        // Events — other systems (scoring, spawner) can subscribe to these
        public System.Action<Mail_Items_SO, MailAction, bool> OnMailActioned;

        private void Awake()
        {
            // Action buttons
            acceptButton.onClick.AddListener(() => OnActionClicked(MailAction.Accept));
            replyButton.onClick.AddListener(() => OnActionClicked(MailAction.Reply));
            rejectButton.onClick.AddListener(() => OnActionClicked(MailAction.Reject));
            reportButton.onClick.AddListener(() => OnActionClicked(MailAction.Report));

            // Report panel
            submitReportButton.onClick.AddListener(OnSubmitReport);
            cancelReportButton.onClick.AddListener(OnCancelReport);

            // Toggle listeners for enabling/disabling dropdowns
            contrabandToggle.onValueChanged.AddListener(OnContrabandToggleChanged);
            substanceToggle.onValueChanged.AddListener(OnSubstanceToggleChanged);

            // Start everything hidden
            actionButtonsPanel.SetActive(false);
            reportPanel.SetActive(false);
            //feedbackPopup.SetActive(false);
        }

        private void Update()
        {
            if (!isInspecting) return;

            // Hotkey support
            if (Input.GetKeyDown(KeyCode.Alpha1)) OnActionClicked(MailAction.Accept);
            if (Input.GetKeyDown(KeyCode.Alpha2)) OnActionClicked(MailAction.Reply);
            if (Input.GetKeyDown(KeyCode.Alpha3)) OnActionClicked(MailAction.Reject);
            if (Input.GetKeyDown(KeyCode.Alpha4)) OnActionClicked(MailAction.Report);
        }

        // =====================================================================
        // PUBLIC — Called by the desk/spawner when player selects mail
        // =====================================================================

        /// <summary>
        /// Opens the appropriate inspection panel for the given mail item.
        /// </summary>
        public void InspectMail(Mail_Items_SO mailData, GameObject mailObject)
        {
            if (mailData == null) return;

            currentMail = mailData;
            currentMailObject = mailObject;
            isInspecting = true;

            if (mailData.mailType == MailType.Letter)
            {
                letterInspectionUI.Open(mailData, fromPackage: false, onClose: OnInspectionClosed);
            }
            else if (mailData.mailType == MailType.Package)
            {
                packageInspectionUI.Open(mailData, onClose: OnInspectionClosed);
            }

            // Show action buttons
            actionButtonsPanel.SetActive(true);
        }

        /// <summary>
        /// Returns whether the manager is currently inspecting mail.
        /// </summary>
        public bool IsInspecting()
        {
            return isInspecting;
        }

        // =====================================================================
        // ACTION HANDLING
        // =====================================================================

        private void OnActionClicked(MailAction playerAction)
        {
            if (currentMail == null || !isInspecting) return;

            // Report opens the report panel instead of immediately resolving
            if (playerAction == MailAction.Report)
            {
                OpenReportPanel();
                return;
            }

            ResolveAction(playerAction);
        }

        private void ResolveAction(MailAction playerAction)
        {

            if (currentMail == null) return;
                        
            // check if tutorial mail — no scoring for tutorial
            DragCheck drag = currentMailObject.GetComponent<DragCheck>();
            bool isTutorial = drag != null && drag.isTutorialMail;

            if (!isTutorial)
            {
                // calculate points
                int points = CalculatePoints(playerAction, currentMail.idealAction);

                // update HUD score
                HUDManager.Instance.OnMailSorted(playerAction, currentMail.idealAction);

                // check if this is a character or guaranteed mail
                DayConfig day = GameManager.Instance.CurrentDay;
                bool isCharacterMail = currentMail == day.characterAMail ||
                                       currentMail == day.characterBMail ||
                                       currentMail == day.characterCMail ||
                                       currentMail == day.generalGuaranteedMail;

                if (isCharacterMail)
                    GameManager.Instance.OnGuaranteedSorted(currentMail, points);
                else
                    GameManager.Instance.OnRandomSorted(currentMail);
            }
            else
            {
                Debug.Log("[Inspector] Tutorial mail sorted — no score");
            }

            // notify subscribers and spawner
            OnMailActioned?.Invoke(currentMail, playerAction, playerAction == currentMail.idealAction);
            FindObjectOfType<MailSpawner>()?.OnMailSorted();

            // destroy the physical letter
            if (currentMailObject != null)
                Destroy(currentMailObject);

            currentMailObject = null;
            currentMail = null;
            isInspecting = false;

            // close inspection panels
            CloseAllPanels();
        }

        int CalculatePoints(MailAction playerAction, MailAction correctAction)
        {
            if (playerAction == correctAction) return 10;
            if (correctAction == MailAction.Reply && playerAction == MailAction.Accept) return 5;
            if (correctAction == MailAction.Reject && playerAction == MailAction.Reply) return 5;
            if (correctAction == MailAction.Report && playerAction == MailAction.Reject) return 5;
            return 0;
        }

        // =====================================================================
        // REPORT PANEL
        // =====================================================================

        private void OpenReportPanel()
        {
            // Reset toggles and dropdowns
            contrabandToggle.isOn = false;
            contrabandTypeDropdown.value = 0;
            contrabandTypeDropdown.interactable = false;

            substanceToggle.isOn = false;
            substanceTypeDropdown.value = 0;
            substanceTypeDropdown.interactable = false;

            otherOffenceDropdown.value = 0;

            reportPanel.SetActive(true);
        }

        private void OnContrabandToggleChanged(bool isOn)
        {
            contrabandTypeDropdown.interactable = isOn;
            if (!isOn) contrabandTypeDropdown.value = 0;
        }

        private void OnSubstanceToggleChanged(bool isOn)
        {
            substanceTypeDropdown.interactable = isOn;
            if (!isOn) substanceTypeDropdown.value = 0;
        }

        private void OnSubmitReport()
        {
            reportPanel.SetActive(false);

            // For prototype: simple scoring — if idealAction is Report, it's correct
            ResolveAction(MailAction.Report);
        }

        private void OnCancelReport()
        {
            reportPanel.SetActive(false);
            // Return to inspection — do nothing else
        }

        // =====================================================================
        // FEEDBACK
        // =====================================================================

        //private void ShowFeedback(string message, int scoreChange)
        //{
        //    feedbackText.text = message;

        //    if (scoreChange >= 0)
        //    {
        //        scoreChangedText.text = "+" + scoreChange + " BP";
        //        scoreChangedText.color = Color.green;
        //    }
        //    else
        //    {
        //        scoreChangedText.text = scoreChange + " BP";
        //        scoreChangedText.color = Color.red;
        //    }

        //    feedbackPopup.SetActive(true);

        //    // Auto-dismiss after duration
        //    CancelInvoke(nameof(HideFeedback));
        //    Invoke(nameof(HideFeedback), feedbackDuration);
        //}

        //private void HideFeedback()
        //{
        //    feedbackPopup.SetActive(false);
        //}

        // =====================================================================
        // PANEL MANAGEMENT
        // =====================================================================

        private void OnInspectionClosed()
        {
            // Called when the player closes an inspection panel via X button
            // without taking an action — hide action buttons
            actionButtonsPanel.SetActive(false);
            isInspecting = false;
            currentMail = null;
        }

        private void CloseAllPanels()
        {
            letterInspectionUI.gameObject.SetActive(false);
            packageInspectionUI.gameObject.SetActive(false);
            actionButtonsPanel.SetActive(false);
            reportPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            acceptButton.onClick.RemoveAllListeners();
            replyButton.onClick.RemoveAllListeners();
            rejectButton.onClick.RemoveAllListeners();
            reportButton.onClick.RemoveAllListeners();
            submitReportButton.onClick.RemoveAllListeners();
            cancelReportButton.onClick.RemoveAllListeners();
            contrabandToggle.onValueChanged.RemoveAllListeners();
            substanceToggle.onValueChanged.RemoveAllListeners();
        }
    }
}
