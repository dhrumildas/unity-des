using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MailSorting.Data;

public class EnvelopeObject : MonoBehaviour
{
    [Header("References")]
    public Image envelopeImage;
    public TextMeshProUGUI receiverAddressText;
    public Button openButton;
    public GameObject letterObject;

    private Mail_Items_SO mailData;
    private bool isOpened = false;

    private GameObject spawnedLetter;

    public void Initialise(Mail_Items_SO data)
    {
        mailData = data;

        // Set envelope sprite
        if (envelopeImage != null && data.mailSprite != null)
            envelopeImage.sprite = data.mailSprite;

        // Hardcoded receiver address
        if (receiverAddressText != null)
            receiverAddressText.text = "AVA Talent Corporation\nPO BOX 12118\nPrestige Continue\nNew New Eden";

        // *** REMOVED the entire letterObject.transform.SetParent block from here ***

        // Wire open button
        if (openButton != null)
            openButton.onClick.AddListener(OnOpenClicked);
    }

    private void OnOpenClicked()
    {
        if (isOpened) return;
        isOpened = true;

        // letterObject is now your Prefab from the Project window
        if (letterObject != null)
        {
            Canvas rootCanvas = GetComponentInParent<Canvas>();

            // Instantiate the prefab directly onto the root canvas so it moves independently!
            spawnedLetter = Instantiate(letterObject, rootCanvas.transform, false);

            LetterObject letter = spawnedLetter.GetComponent<LetterObject>();
            if (letter != null)
                letter.Initialise(mailData);

            UI_DragCheck letterDrag = spawnedLetter.GetComponent<UI_DragCheck>();
            if (letterDrag == null)
                letterDrag = spawnedLetter.AddComponent<UI_DragCheck>();

            letterDrag.mailData = mailData;
        }

        if (openButton != null)
            openButton.gameObject.SetActive(false);

        if (ActionButtonController.Instance != null)
            ActionButtonController.Instance.OnEnvelopeOpened();
    }

    public void DestroyLetter()
    {
        if (spawnedLetter != null)
            Destroy(spawnedLetter);
    }
}