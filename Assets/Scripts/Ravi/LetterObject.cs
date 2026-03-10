using UnityEngine;
using TMPro;
using MailSorting.Data;

public class LetterObject : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI salutationText;
    public TextMeshProUGUI bodyText;
    public TextMeshProUGUI signatureText;

    [Header("Size Settings")]
    public Vector2 defaultLetterSize = new Vector2(300f, 400f);

    public void Initialise(Mail_Items_SO data)
    {
        
        // Placeholder white background until sprites are ready
        UnityEngine.UI.Image img = GetComponent<UnityEngine.UI.Image>();
        if (img != null)
        {
            img.color = Color.white;
            if (data.contentSprite != null)
                img.sprite = data.contentSprite;
        }

        // Salutation
        if (salutationText != null)
            salutationText.text = data.addressedCorrectly ? "Dear Aurora," : data.howAuroraAddressed;

        // Body
        if (bodyText != null)
            bodyText.text = data.letterContent;

        // Signature — bottom left
        if (signatureText != null)
            signatureText.text = data.signatureName;
    }
}