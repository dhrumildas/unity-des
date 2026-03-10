using UnityEngine;
using TMPro;
using MailSorting.Data;
using MailSorting.Gameplay;

public class Tutorial : MonoBehaviour
{
    [Header("State Tracking")]
    public int stage = 0;
    private bool waitingForButton = false;
    private string buttonPressed = "";

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI agentText;
    public GameObject spacebarPrompt;

    [Header("Hardcoded Mails")]
    public Mail_Items_SO acceptMail;
    public Mail_Items_SO replyMail;
    public Mail_Items_SO rejectMail;
    public Mail_Items_SO reportMail;

    [Header("Spawning Setup")]
    public Transform letterSpawnPoint;
    public GameObject letterPrefab;

    private GameObject currentSpawnedMail;

    void Start()
    {
        dialoguePanel.SetActive(true);
        LoadStage(0);
    }

    void Update()
    {
        if (!waitingForButton && Input.GetKeyDown(KeyCode.Space))
        {
            int nextStage = stage + 1;

            // --- REWIND LOGIC ---
            // If they are on a Fail or Acceptable screen, pressing space sends them back to the question!
            if (stage == 13 || stage == 14) nextStage = 12; // Rewind to Accept Test
            else if (stage == 16 || stage == 17) nextStage = 15; // Rewind to Reply Test
            else if (stage == 19 || stage == 20) nextStage = 18; // Rewind to Reject Test
            else if (stage == 22 || stage == 23) nextStage = 21; // Rewind to Report Test

            LoadStage(nextStage);
        }
    }

    public void LoadStage(int newStage)
    {
        stage = newStage;
        waitingForButton = false;
        spacebarPrompt.SetActive(true);

        switch (stage)
        {
            case 0:
                agentText.text = "Welcome to Fan Mail Associate Service, yadda yadda yadda...";
                break;
            case 1:
                agentText.text = "You're here to organise and sort mail and escalate it to the correct department.";
                break;
            case 2:
                agentText.text = "I'm assuming you've done this before, anyway.";
                break;
            case 3:
                agentText.text = "While you passed our checks, you still have a seven day probation to pass before we can sign you off. I'm putting a lot of trust in you for this.";
                break;
            case 4:
                agentText.text = "So, seems here that I have to walk you through how to do your job...";
                break;
            case 5:
                agentText.text = "In front of you is a letter, take a look at what it says.";
                SpawnTutorialMail(acceptMail);
                break;
            case 6:
                agentText.text = "Now if you pay attention to the rules on the right, you can compare these to the letter to see if it passes.";
                break;
            case 7:
                agentText.text = "To your left, there are buttons on the side of your desk. You can press the number keys 1, 2, 3 or 4 to quickly select these.";
                break;
            case 8:
                agentText.text = "If all the rules are followed, the mail can be ACCEPTED.";
                break;
            case 9:
                agentText.text = "If one rule was broken, the mail can be sent to another department to be REPLIED to.";
                break;
            case 10:
                agentText.text = "If more than one rule is broken, the mail should be REJECTED.";
                break;
            case 11:
                agentText.text = "If anything in the mail is suspicious or dangerous, you should REPORT it.";
                break;
            case 12:
                agentText.text = "Okay, got all that? Go ahead and make a decision for the letter you have there.";
                if (currentSpawnedMail == null) SpawnTutorialMail(acceptMail);
                WaitForButton();
                break;

            // --- ACCEPT LETTER GRADING ---
            case 13: // Accept Fail
                agentText.text = $"Really? You thought that was {buttonPressed}? Try again, here.";
                break;
            case 14: // Accept Acceptable
                agentText.text = "Okay, I see how you would think that, but that was actually an ACCEPT. Here's another letter, try again.";
                break;
            case 15: // Accept Correct
                agentText.text = "Good, now how would you process this one?";
                SpawnTutorialMail(replyMail);
                WaitForButton();
                break;

            // --- REPLY LETTER GRADING ---
            case 16: // Reply Fail
                agentText.text = $"Really? You thought that was {buttonPressed}? Try again, here.";
                break;
            case 17: // Reply Acceptable
                agentText.text = "Okay, I see how you would think that, but that was actually a REPLY. Here's another letter, try again.";
                break;
            case 18: // Reply Correct
                agentText.text = "That's correct, you're getting the hang of this. Here's another letter.";
                SpawnTutorialMail(rejectMail);
                WaitForButton();
                break;

            // --- REJECT LETTER GRADING ---
            case 19: // Reject Fail
                agentText.text = $"Really? You thought that was {buttonPressed}? Try again, here.";
                break;
            case 20: // Reject Acceptable
                agentText.text = "Okay, I see how you would think that, but that was actually a REJECT. Here's another letter, try again.";
                break;
            case 21: // Reject Correct
                agentText.text = "You're on a roll, okay, one last letter. Take a look at this.";
                SpawnTutorialMail(reportMail);
                WaitForButton();
                break;

            // --- REPORT LETTER GRADING ---
            case 22: // Report Fail
                agentText.text = $"Really? You thought that was {buttonPressed}? Try again, here.";
                break;
            case 23: // Report Acceptable
                agentText.text = "Okay, I see how you would think that, but that was actually a REPORT. Here's another letter, try again.";
                break;
            case 24: // Report Correct (Proceeds to Outro)
                agentText.text = "Yeah, sorry you had to see that one, but you're gonna have to get used to it if you're gonna work here.";
                break;

            // --- OUTRO ---
            case 25:
                agentText.text = "Good. Well I'm too busy to answer questions, so you'll just have to get on with it. Tomorrow we have a bigger shipment of mail, so be prepared.";
                break;
            case 26:
                Debug.Log("Tutorial Complete! Transition to Clock In Scene!");
                // UnityEngine.SceneManagement.SceneManager.LoadScene("ClockInScene");
                break;

            case -1:
                agentText.text = "I don't have time for this. George, you need to find me another mail sorter.";
                waitingForButton = true; // Lock the game
                spacebarPrompt.SetActive(false);
                break;
        }

        //// Logic to "Rewind" if the player hit spacebar after failing a letter
        //if (stage == 13 || stage == 14) stage = 11; // Rewinds to "Make a decision..." (Accept)
        //if (stage == 16 || stage == 17) stage = 14; // Rewinds to waiting for Reply
        //if (stage == 19 || stage == 20) stage = 17; // Rewinds to waiting for Reject
        //if (stage == 22 || stage == 23) stage = 20; // Rewinds to waiting for Report
    }

    private void WaitForButton()
    {
        waitingForButton = true;
        spacebarPrompt.SetActive(false);
    }

    private void SpawnTutorialMail(Mail_Items_SO mailData)
    {
        if (currentSpawnedMail != null) Destroy(currentSpawnedMail);

        currentSpawnedMail = Instantiate(letterPrefab, letterSpawnPoint.position, Quaternion.identity, letterSpawnPoint);

        // 1. Initialize Envelope (if your prefab uses one)
        EnvelopeObject envelope = currentSpawnedMail.GetComponent<EnvelopeObject>();
        if (envelope != null) envelope.Initialise(mailData);

        // 2. INITIALIZE THE LETTER (This is the fix!)
        // It checks the main object first, and if it's not there, it checks the children.
        LetterObject letter = currentSpawnedMail.GetComponent<LetterObject>();
        if (letter == null) letter = currentSpawnedMail.GetComponentInChildren<LetterObject>(true);
        if (letter != null) letter.Initialise(mailData);

        // 3. Setup Dragging
        UI_DragCheck drag = currentSpawnedMail.GetComponent<UI_DragCheck>();
        if (drag != null) drag.mailData = mailData;
    }

    // --- BUTTON INTERCEPT METHODS ---
    public void ClickAccept() => CheckAnswer(MailAction.Accept);
    public void ClickReply() => CheckAnswer(MailAction.Reply);
    public void ClickReject() => CheckAnswer(MailAction.Reject);
    public void ClickReport() => CheckAnswer(MailAction.Report);

    private void CheckAnswer(MailAction playerAction)
    {
        if (!waitingForButton) return;

        buttonPressed = playerAction.ToString().ToUpper();
        if (currentSpawnedMail != null) Destroy(currentSpawnedMail);

        // Grade the current state based on what letter they are on
        if (stage == 12) GradeTask(playerAction, MailAction.Accept, MailAction.Reply, 15, 14, 13);
        else if (stage == 15) GradeTask(playerAction, MailAction.Reply, MailAction.Reject, 18, 17, 16);
        else if (stage == 18) GradeTask(playerAction, MailAction.Reject, MailAction.Report, 21, 20, 19);
        else if (stage == 21) GradeTask(playerAction, MailAction.Report, MailAction.Reject, 24, 23, 22);
    }

    private void GradeTask(MailAction player, MailAction expected, MailAction acceptable, int correctStage, int acceptableStage, int failStage)
    {
        if (player == expected) LoadStage(correctStage);
        else if (player == acceptable) LoadStage(acceptableStage);
        else LoadStage(failStage);
    }
}