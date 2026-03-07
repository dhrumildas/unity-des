using UnityEngine;
using UnityEngine.UI;
using MailSorting.Data;
using MailSorting.Gameplay;

// NO namespace wrapper — sits in global scope so everyone can see it
public class ActionButtonController : MonoBehaviour
{
    public static ActionButtonController Instance;

    [Header("Buttons")]
    public Button acceptButton;
    public Button replyButton;
    public Button rejectButton;
    public Button reportButton;

    
    private Mail_Items_SO currentMail;
    private bool mailIsOpen = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        
    }

    void Start()
    {
        acceptButton.onClick.AddListener(() => OnAction(MailAction.Accept));
        replyButton.onClick.AddListener(() => OnAction(MailAction.Reply));
        rejectButton.onClick.AddListener(() => OnAction(MailAction.Reject));
        reportButton.onClick.AddListener(() => OnAction(MailAction.Report));

        SetButtonsInteractable(false);
    }

    public void SetCurrentMail(Mail_Items_SO mail)
    {
        currentMail = mail;
        mailIsOpen = false;
        SetButtonsInteractable(false);
    }

    public void OnEnvelopeOpened()
    {
        mailIsOpen = true;
        SetButtonsInteractable(true);
    }

    private void OnAction(MailAction action)
    {
        if (currentMail == null)
        {
            Debug.LogWarning("[ActionButtonController] No active mail.");
            return;
        }

        if (!mailIsOpen)
        {
            Debug.LogWarning("[ActionButtonController] Open the envelope first.");
            return;
        }

        //Narrative Tracking
        if (NarrativeTracker.Instance != null)
            NarrativeTracker.Instance.LogAction(currentMail.senderProfile, action);
        
        //scoring
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.RegisterDecision(currentMail, action);

        Debug.Log($"[ActionButtonController] {action} on [{currentMail.mailID}]");

        SetButtonsInteractable(false);
        currentMail = null;
        mailIsOpen = false;

        FindFirstObjectByType<MailSorting.Gameplay.MailSpawner>()?.ProcessNextMail();
    }

    private void SetButtonsInteractable(bool state)
    {
        acceptButton.interactable = state;
        replyButton.interactable = state;
        rejectButton.interactable = state;
        reportButton.interactable = state;
    }
}