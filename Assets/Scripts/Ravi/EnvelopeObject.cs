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

        // Hardcoded receiver address — never changes
        if (receiverAddressText != null)
            receiverAddressText.text = "AVA Talent Corporation\nPO BOX 12118\nPrestige Continue\nNew New Eden";

        // Immediately unparent letter to canvas root so it drags independently
        if (letterObject != null)
        {
            Canvas rootCanvas = GetComponentInParent<Canvas>();
            letterObject.transform.SetParent(rootCanvas.transform, true);
            letterObject.SetActive(false); // still hidden until opened

            // Give letter its own UI_DragCheck
            UI_DragCheck letterDrag = letterObject.GetComponent<UI_DragCheck>();
            if (letterDrag == null)
                letterDrag = letterObject.AddComponent<UI_DragCheck>();

            // Assign mail data so scale can read letter weight
            letterDrag.mailData = data;

            //letterDrag.setOriginalParent(rootCanvas.transform);

        }

        // Wire open button
        if (openButton != null)
            openButton.onClick.AddListener(OnOpenClicked);
    }

    private void OnOpenClicked()
    {
        if (isOpened) return;
        isOpened = true;

        if (letterObject != null)
        {
            Canvas rootCanvas = GetComponentInParent<Canvas>();
            letterObject.transform.SetParent(rootCanvas.transform, false);
            letterObject.SetActive(true);
            spawnedLetter = letterObject;

            LetterObject letter = letterObject.GetComponent<LetterObject>();
            if (letter != null)
                letter.Initialise(mailData);

            UI_DragCheck letterDrag = letterObject.GetComponent<UI_DragCheck>();
            if (letterDrag == null)
                letterDrag = letterObject.AddComponent<UI_DragCheck>();
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