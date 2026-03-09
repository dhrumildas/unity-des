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

    [Header("Letter")]
    public GameObject letterPrefab;

    private Mail_Items_SO mailData;
    private bool isOpened = false;
    private GameObject spawnedLetter;

    public void Initialise(Mail_Items_SO data)
    {
        mailData = data;

        if (envelopeImage != null && data.mailSprite != null)
            envelopeImage.sprite = data.mailSprite;

        if (envelopeImage != null)
            envelopeImage.color = new Color(0.95f, 0.85f, 0.7f);

        if (receiverAddressText != null)
            receiverAddressText.text = "AVA Talent Corporation\nPO BOX 12118\nPrestige Continue\nNew New Eden";

        if (openButton != null)
            openButton.onClick.AddListener(OnOpenClicked);
    }

    private void OnOpenClicked()
    {
        if (isOpened) return;
        isOpened = true;

        // Spawn letter prefab directly on canvas
        Canvas rootCanvas = GetComponentInParent<Canvas>();
        while (rootCanvas.transform.parent != null &&
           rootCanvas.transform.parent.GetComponent<Canvas>() != null)
            rootCanvas = rootCanvas.transform.parent.GetComponent<Canvas>();
        spawnedLetter = Instantiate(letterPrefab, rootCanvas.transform);

        // Position it next to envelope
        RectTransform envelopeRt = GetComponent<RectTransform>();
        RectTransform letterRt = spawnedLetter.GetComponent<RectTransform>();
        letterRt.anchoredPosition = envelopeRt.anchoredPosition + new Vector2(20f, 20f);

        // Assign mail data to drag
        UI_DragCheck letterDrag = spawnedLetter.GetComponent<UI_DragCheck>();
        if (letterDrag != null)
            letterDrag.mailData = mailData;

        // Initialise letter content
        LetterObject letter = spawnedLetter.GetComponent<LetterObject>();
        if (letter != null)
            letter.Initialise(mailData);

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