using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MailSorting.Data;

namespace MailSorting.UI
{
    /// <summary>
    /// Controls the Letter Inspection Panel.
    /// Populates all fields from a Mail_Items_SO and handles front/back flipping.
    /// Attach to: LetterInspectionPanel GameObject.
    /// </summary>
    public class LetterInspectionUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button flipButton;

        [Header("Views")]
        [SerializeField] private GameObject envelopeFrontView;
        [SerializeField] private GameObject letterBackView;
        [SerializeField] private GameObject encryptedOverlay;

        [Header("Envelope Front — Sender")]
        [SerializeField] private TMP_Text senderAddressLabel;
        [SerializeField] private TMP_Text senderAddressText;
        [SerializeField] private TMP_Text addressLabel;
        [SerializeField] private TMP_Text addressNameText;

        [Header("Envelope Front — Postage")]
        [SerializeField] private Image postageStampImage;
        [SerializeField] private TMP_Text countryOfOriginText;

        [Header("Envelope Front — Visual Clues")]
        [SerializeField] private Image[] visualClueImages;

        [Header("Letter Back — Content")]
        [SerializeField] private TMP_Text letterContentText;
        [SerializeField] private Image letterContentImage;

        [Header("Letter Back — Signature")]
        [SerializeField] private TMP_Text signatureLabel;
        [SerializeField] private TMP_Text signatureText;

        // State
        private bool showingFront = true;
        private Mail_Items_SO currentMail;
        private System.Action onCloseCallback;

        private void Awake()
        {
            closeButton.onClick.AddListener(OnCloseClicked);
            flipButton.onClick.AddListener(OnFlipClicked);
        }

        /// <summary>
        /// Opens the letter panel and populates it with mail data.
        /// </summary>
        /// <param name="mailData">The mail ScriptableObject to display</param>
        /// <param name="fromPackage">True if opened via View Enclosed Letter from package panel</param>
        /// <param name="onClose">Callback when closed (used for returning to package view)</param>
        public void Open(Mail_Items_SO mailData, bool fromPackage = false, System.Action onClose = null)
        {
            currentMail = mailData;
            onCloseCallback = onClose;

            PopulateEnvelopeFront();
            PopulateLetterBack();
            SetEncryptedOverlay();

            // Always start showing the front
            showingFront = true;
            envelopeFrontView.SetActive(true);
            letterBackView.SetActive(false);

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Closes the letter panel.
        /// </summary>
        public void Close()
        {
            gameObject.SetActive(false);
            currentMail = null;

            onCloseCallback?.Invoke();
            onCloseCallback = null;
        }

        /// <summary>
        /// Returns the currently displayed mail item.
        /// </summary>
        public Mail_Items_SO GetCurrentMail()
        {
            return currentMail;
        }

        // =====================================================================
        // POPULATE
        // =====================================================================

        private void PopulateEnvelopeFront()
        {
            if (currentMail == null) return;

            // Sender info
            senderAddressText.text = string.IsNullOrEmpty(currentMail.senderAddress)
                ? "No return address"
                : currentMail.senderAddress;

            addressNameText.text = string.IsNullOrEmpty(currentMail.addressedName)
                ? "(no addressee)"
                : currentMail.addressedName;

            // Postage
            countryOfOriginText.text = currentMail.countryOfOrigin.ToString();

            // Postage stamp sprite
            if (postageStampImage != null)
            {
                if (currentMail.postageStampSprite != null)
                {
                    postageStampImage.sprite = currentMail.postageStampSprite;
                    postageStampImage.enabled = true;
                }
                else
                {
                    postageStampImage.enabled = false;
                }
            }

            // Visual clues
            if (visualClueImages != null)
            {
                for (int i = 0; i < visualClueImages.Length; i++)
                {
                    if (currentMail.visualClues != null &&
                        i < currentMail.visualClues.Length &&
                        currentMail.visualClues[i] != null)
                    {
                        visualClueImages[i].sprite = currentMail.visualClues[i];
                        visualClueImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        visualClueImages[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        private void PopulateLetterBack()
        {
            if (currentMail == null) return;

            bool hasText = !string.IsNullOrEmpty(currentMail.letterContent);
            bool hasImage = currentMail.letterContentImage != null;

            // Letter content text
            if (hasText)
            {
                letterContentText.text = currentMail.letterContent;
                letterContentText.gameObject.SetActive(true);
            }
            else
            {
                letterContentText.text = "";
                letterContentText.gameObject.SetActive(false);
            }

            // Letter content image
            if (letterContentImage != null)
            {
                if (hasImage)
                {
                    letterContentImage.sprite = currentMail.letterContentImage;
                    letterContentImage.gameObject.SetActive(true);
                }
                else
                {
                    letterContentImage.gameObject.SetActive(false);
                }
            }

            // Signature
            if (string.IsNullOrEmpty(currentMail.signatureName))
            {
                signatureText.text = "(unsigned)";
                signatureText.fontStyle = FontStyles.Italic;
                signatureText.color = Color.red;
            }
            else
            {
                signatureText.text = currentMail.signatureName;
                signatureText.fontStyle = FontStyles.Normal;
                signatureText.color = Color.black;
            }
        }

        private void SetEncryptedOverlay()
        {
            if (encryptedOverlay != null)
            {
                encryptedOverlay.SetActive(currentMail != null && currentMail.isEncrypted);
            }
        }

        // =====================================================================
        // BUTTON HANDLERS
        // =====================================================================

        private void OnFlipClicked()
        {
            showingFront = !showingFront;
            envelopeFrontView.SetActive(showingFront);
            letterBackView.SetActive(!showingFront);
        }

        private void OnCloseClicked()
        {
            Close();
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(OnCloseClicked);
            flipButton.onClick.RemoveListener(OnFlipClicked);
        }
    }
}