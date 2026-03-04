using UnityEngine;
using UnityEngine.UI;
using MailSorting.Data;
using MailSorting.Gameplay;

public class ActionButtonsController : MonoBehaviour
{
    public static ActionButtonsController Instance;

    [Header("Buttons")]
    public Button acceptButton;
    public Button replyButton;
    public Button rejectButton;
    public Button reportButton;

    [Header("Report Panel")]
    public GameObject reportPanel;
    public Button submitReportButton;
    public Button cancelReportButton;

    // currently active mail
    private Mail_SO activeMail;
    private bool isTutorialMail = false;

    void Awake()
    {
        Instance = this;

        acceptButton.onClick.AddListener(() => OnActionClicked(MailAction.Accept));
        replyButton.onClick.AddListener(() => OnActionClicked(MailAction.Reply));
        rejectButton.onClick.AddListener(() => OnActionClicked(MailAction.Reject));
        reportButton.onClick.AddListener(() => OnActionClicked(MailAction.Report));

        submitReportButton.onClick.AddListener(OnSubmitReport);
        cancelReportButton.onClick.AddListener(OnCancelReport);

        reportPanel.SetActive(false);
    }

    void Update()
    {
        if (activeMail == null) return;

        // hotkeys
        if (Input.GetKeyDown(KeyCode.Alpha1)) OnActionClicked(MailAction.Accept);
        if (Input.GetKeyDown(KeyCode.Alpha2)) OnActionClicked(MailAction.Reply);
        if (Input.GetKeyDown(KeyCode.Alpha3)) OnActionClicked(MailAction.Reject);
        if (Input.GetKeyDown(KeyCode.Alpha4)) OnActionClicked(MailAction.Report);
    }

    // called by UI_DragCheck when mail is picked up
    public void SetActiveMail(Mail_SO mail, bool isTutorial)
    {
        activeMail = mail;
        isTutorialMail = isTutorial;
        Debug.Log($"[Actions] Active mail set: {mail.senderName}");
    }

    void OnActionClicked(MailAction playerAction)
    {
        if (activeMail == null)
        {
            Debug.LogWarning("[Actions] No active mail to sort!");
            return;
        }

        if (playerAction == MailAction.Report)
        {
            reportPanel.SetActive(true);
            return;
        }

        ResolveAction(playerAction);
    }

    void OnSubmitReport()
    {
        reportPanel.SetActive(false);
        ResolveAction(MailAction.Report);
    }

    void OnCancelReport()
    {
        reportPanel.SetActive(false);
    }

    void ResolveAction(MailAction playerAction)
    {
        if (activeMail == null) return;

        // get correct action from RuleValidator
        MailAction correctAction = RuleValidator.Instance.EvaluateMail(activeMail, GameManager.Instance.CurrentDay);

        if (!isTutorialMail)
        {
            // update HUD
            HUDManager.Instance.OnMailSorted(playerAction, correctAction, isTutorialMail);

            // update character scores
            int points = CalculatePoints(playerAction, correctAction);
            GameManager.Instance.OnMailSorted(activeMail, points);

            Debug.Log($"[Actions] Player: {playerAction} | Correct: {correctAction} | Points: {points}");
        }
        else
        {
            Debug.Log("[Actions] Tutorial mail sorted — no score");
        }

        // notify spawner to destroy mail and spawn next
        FindObjectOfType<MailSpawner>()?.OnMailSorted();

        activeMail = null;
        isTutorialMail = false;
    }

    int CalculatePoints(MailAction playerAction, MailAction correctAction)
    {
        if (playerAction == correctAction) return 10;
        if (correctAction == MailAction.Reply && playerAction == MailAction.Accept) return 5;
        if (correctAction == MailAction.Reject && playerAction == MailAction.Reply) return 5;
        if (correctAction == MailAction.Report && playerAction == MailAction.Reject) return 5;
        return 0;
    }

    void OnDestroy()
    {
        acceptButton.onClick.RemoveAllListeners();
        replyButton.onClick.RemoveAllListeners();
        rejectButton.onClick.RemoveAllListeners();
        reportButton.onClick.RemoveAllListeners();
        submitReportButton.onClick.RemoveAllListeners();
        cancelReportButton.onClick.RemoveAllListeners();
    }
}