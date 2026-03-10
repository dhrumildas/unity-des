using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MailSorting.Data;
using MailSorting.Gameplay;

public enum TutorialState
{
    IntroDialogue,
    AcceptTask,
    ReplyTask,
    RejectTask,
    ReportTask,
    OutroDialogue
}

public class Tutorial : MonoBehaviour
{
    [Header("State Tracking")]
    public TutorialState currentState = TutorialState.IntroDialogue;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI agentText;
    public GameObject spacebarPrompt; // "Press [SPACE] to continue"

    [Header("Hardcoded Mails")]
    public Mail_Items_SO acceptMail;
    public Mail_Items_SO replyMail;
    public Mail_Items_SO rejectMail;
    public Mail_Items_SO reportMail;

    [Header("Spawning Setup")]
    public Transform letterSpawnPoint;
    public GameObject letterPrefab;

    // Internal tracking
    private GameObject currentSpawnedMail;
    private int dialogueIndex = 0;
    private string[] currentDialogueArray;
    private bool waitingForPlayerAction = false;

    // --- HARDCODED DIALOGUE ---
    private string[] introLines = {
        "Welcome to A.V.A. Sorting Facility.",
        "I am your supervising Agent. I will walk you through the basics.",
        "Your job is to process mail based on daily directives.",
        "Let's start simple. The rule today is: All mail must have a valid signature.",
        "I'm sending you a perfectly fine letter. Approve it."
    };
    private string[] failLines = {
        "Incorrect. Read the directives carefully.",
        "Let's try that again. Here is a fresh copy."
    };

    void Start()
    {
        // Kick off the tutorial
        StartDialogue(introLines);
    }

    void Update()
    {
        // Handle Dialogue Progression
        if (!waitingForPlayerAction && dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceDialogue();
        }
    }

    // --- DIALOGUE SYSTEM ---

    private void StartDialogue(string[] lines)
    {
        currentDialogueArray = lines;
        dialogueIndex = 0;
        dialoguePanel.SetActive(true);
        spacebarPrompt.SetActive(true);
        agentText.text = currentDialogueArray[dialogueIndex];
    }

    private void AdvanceDialogue()
    {
        dialogueIndex++;

        if (dialogueIndex < currentDialogueArray.Length)
        {
            agentText.text = currentDialogueArray[dialogueIndex];
        }
        else
        {
            // Dialogue finished! Trigger the next action
            dialoguePanel.SetActive(false);
            TriggerNextPhase();
        }
    }

    private void TriggerNextPhase()
    {
        waitingForPlayerAction = true;

        switch (currentState)
        {
            case TutorialState.IntroDialogue:
                currentState = TutorialState.AcceptTask;
                SpawnTutorialMail(acceptMail);
                break;
            case TutorialState.AcceptTask:
                currentState = TutorialState.ReplyTask;
                SpawnTutorialMail(replyMail);
                break;
            case TutorialState.ReplyTask:
                currentState = TutorialState.RejectTask;
                SpawnTutorialMail(rejectMail);
                break;
            case TutorialState.RejectTask:
                currentState = TutorialState.ReportTask;
                SpawnTutorialMail(reportMail);
                break;
            case TutorialState.ReportTask:
                currentState = TutorialState.OutroDialogue;
                waitingForPlayerAction = false;
                StartDialogue(new string[] { "Good. You understand the basics.", "Clock in when you are ready to begin your real shift." });
                break;
            case TutorialState.OutroDialogue:
                // TUTORIAL COMPLETE!
                Debug.Log("Tutorial Finished! Time to load the Clock-In Scene.");
                // UnityEngine.SceneManagement.SceneManager.LoadScene("ClockInScene");
                break;
        }
    }

    // --- MAIL HANDLING ---

    private void SpawnTutorialMail(Mail_Items_SO mailData)
    {
        if (currentSpawnedMail != null) Destroy(currentSpawnedMail);

        currentSpawnedMail = Instantiate(letterPrefab, letterSpawnPoint.position, Quaternion.identity, letterSpawnPoint);

        // Setup the physical envelope
        EnvelopeObject envelope = currentSpawnedMail.GetComponent<EnvelopeObject>();
        if (envelope != null) envelope.Initialise(mailData);

        UI_DragCheck drag = currentSpawnedMail.GetComponent<UI_DragCheck>();
        if (drag != null) drag.mailData = mailData;

        // Tell the Agent UI to give them a hint
        dialoguePanel.SetActive(true);
        spacebarPrompt.SetActive(false); // Hide spacebar prompt since they need to click a button now

        switch (currentState)
        {
            case TutorialState.AcceptTask: agentText.text = "Click ACCEPT."; break;
            case TutorialState.ReplyTask: agentText.text = "This one is missing a signature. Click REPLY."; break;
            case TutorialState.RejectTask: agentText.text = "This is too heavy. Click REJECT."; break;
            case TutorialState.ReportTask: agentText.text = "This contains contraband! Click REPORT immediately."; break;
        }
    }

    // --- BUTTON INTERCEPT ---
    // Your UI Buttons will call these methods instead of ActionButtonController!

    public void ClickAccept() => CheckPlayerAction(MailAction.Accept);
    public void ClickReply() => CheckPlayerAction(MailAction.Reply);
    public void ClickReject() => CheckPlayerAction(MailAction.Reject);
    public void ClickReport() => CheckPlayerAction(MailAction.Report);

    private void CheckPlayerAction(MailAction playerAction)
    {
        if (!waitingForPlayerAction) return;

        MailAction expectedAction = MailAction.Accept; // Default

        // Determine what the right answer was supposed to be
        if (currentState == TutorialState.AcceptTask) expectedAction = MailAction.Accept;
        if (currentState == TutorialState.ReplyTask) expectedAction = MailAction.Reply;
        if (currentState == TutorialState.RejectTask) expectedAction = MailAction.Reject;
        if (currentState == TutorialState.ReportTask) expectedAction = MailAction.Report;

        if (playerAction == expectedAction)
        {
            // SUCCESS
            if (currentSpawnedMail != null) Destroy(currentSpawnedMail);
            waitingForPlayerAction = false;
            StartDialogue(new string[] { "Correct." });
            // Hitting spacebar after this will trigger the next phase!
        }
        else
        {
            // FAIL
            if (currentSpawnedMail != null) Destroy(currentSpawnedMail);
            waitingForPlayerAction = false;

            // Go back one state so when the dialogue finishes, it repeats the same task!
            if (currentState == TutorialState.AcceptTask) currentState = TutorialState.IntroDialogue;
            else if (currentState == TutorialState.ReplyTask) currentState = TutorialState.AcceptTask;
            else if (currentState == TutorialState.RejectTask) currentState = TutorialState.ReplyTask;
            else if (currentState == TutorialState.ReportTask) currentState = TutorialState.RejectTask;

            StartDialogue(failLines);
        }
    }
}
