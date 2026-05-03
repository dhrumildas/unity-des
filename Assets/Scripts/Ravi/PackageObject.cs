using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MailSorting.Data;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class PackageObject : MonoBehaviour
{
    [Header("References")]
    public Button openButton;

    [Header("Text")]
    public TextMeshProUGUI receiverAddressText;

    [Header("Prefabs")]
    public GameObject letterPrefab;
    public GameObject itemPrefab;

    private Mail_Items_SO mailData;
    private bool isOpened = false;
    private GameObject spawnedLetter;
    private GameObject spawnedItem;

    public void Initialise(Mail_Items_SO data)
    {
        mailData = data;

        // Receiver address — same as envelope, always fixed
        if (receiverAddressText != null)
            receiverAddressText.text = "AVA Talent Corporation\nPO BOX 12118\nPrestige Continue\nNew New Eden";

        // Sprite stays as default on prefab — no override from SO

        if (openButton != null)
            openButton.onClick.AddListener(OnOpenClicked);
    }

    private void OnOpenClicked()
    {
        if (isOpened) return;
        isOpened = true;

        // Get root canvas
        Canvas rootCanvas = null;
        Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var c in allCanvases)
            if (c.isRootCanvas) { rootCanvas = c; break; }

        if (rootCanvas == null) return;

        RectTransform packageRt = GetComponent<RectTransform>();

        // Spawn letter
        if (letterPrefab != null)
        {
            spawnedLetter = Instantiate(letterPrefab, rootCanvas.transform);
            RectTransform letterRt = spawnedLetter.GetComponent<RectTransform>();
            letterRt.anchoredPosition = packageRt.anchoredPosition + new Vector2(-20f, 30f);

            UI_DragCheck letterDrag = spawnedLetter.GetComponent<UI_DragCheck>();
            if (letterDrag != null)
                letterDrag.mailData = mailData;

            LetterObject letter = spawnedLetter.GetComponent<LetterObject>();
            if (letter != null)
                letter.Initialise(mailData);
        }

        // Spawn item — default sprite on prefab, override only if SO has one
        if (itemPrefab != null)
        {
            spawnedItem = Instantiate(itemPrefab, rootCanvas.transform);
            RectTransform itemRt = spawnedItem.GetComponent<RectTransform>();
            itemRt.anchoredPosition = packageRt.anchoredPosition + new Vector2(20f, -30f);

            Image itemImg = spawnedItem.GetComponent<Image>();
            if (itemImg != null && mailData.itemInsideSprite != null)
                itemImg.sprite = mailData.itemInsideSprite;

            UI_DragCheck itemDrag = spawnedItem.GetComponent<UI_DragCheck>();
            if (itemDrag != null)
                itemDrag.mailData = mailData;
        }

        if (openButton != null)
            openButton.gameObject.SetActive(false);

        // Unlock action buttons
        if (ActionButtonController.Instance != null)
            ActionButtonController.Instance.OnEnvelopeOpened();
    }

    public void DestroyContents()
    {
        if (spawnedLetter != null) Destroy(spawnedLetter);
        if (spawnedItem != null) Destroy(spawnedItem);
    }
}
