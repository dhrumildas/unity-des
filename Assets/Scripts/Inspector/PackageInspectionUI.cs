using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MailSorting.Data;

namespace MailSorting.UI
{
    /// <summary>
    /// Controls the Package Inspection Panel.
    /// Handles 4-side navigation, opening the package, customs form, and viewing the enclosed letter.
    /// Attach to: PackageInspectionPanel GameObject.
    /// </summary>
    public class PackageInspectionUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button prevSideButton;
        [SerializeField] private Button nextSideButton;
        [SerializeField] private Button openPackageButton;
        [SerializeField] private Button viewLetterButton;

        [Header("Package View")]
        [SerializeField] private Image packageSideImage;
        [SerializeField] private TMP_Text sideIndicatorText;

        [Header("Item Inside")]
        [SerializeField] private GameObject itemInsidePanel;
        [SerializeField] private Image itemImage;

        [Header("Info Area — Envelope Info")]
        [SerializeField] private TMP_Text senderAddressLabel;
        [SerializeField] private TMP_Text senderAddressText;
        [SerializeField] private TMP_Text addressedLabel;
        [SerializeField] private TMP_Text addressedNameText;
        [SerializeField] private TMP_Text countryOfOriginText;
        [SerializeField] private TMP_Text stampTypeText;
        [SerializeField] private Image postageStampImage;

        [Header("Info Area — Customs Form")]
        [SerializeField] private TMP_Text customsLabel;
        [SerializeField] private TMP_Text customsDescriptionText;

        [Header("References")]
        [SerializeField] private LetterInspectionUI letterInspectionUI;

        // State
        private Mail_Items_SO currentMail;
        private int currentSideIndex = 0;
        private bool packageOpened = false;
        private System.Action onCloseCallback;

        private static readonly string[] sideNames = { "Front", "Right", "Back", "Left" };

        private void Awake()
        {
            closeButton.onClick.AddListener(OnCloseClicked);
            prevSideButton.onClick.AddListener(OnPrevSide);
            nextSideButton.onClick.AddListener(OnNextSide);
            openPackageButton.onClick.AddListener(OnOpenPackage);
            viewLetterButton.onClick.AddListener(OnViewLetter);
        }

        /// <summary>
        /// Opens the package panel and populates it with mail data.
        /// </summary>
        public void Open(Mail_Items_SO mailData, System.Action onClose = null)
        {
            currentMail = mailData;
            onCloseCallback = onClose;
            currentSideIndex = 0;
            packageOpened = false;

            PopulatePackageView();
            PopulateInfoArea();

            // Reset item inside state
            itemInsidePanel.SetActive(false);
            openPackageButton.gameObject.SetActive(true);

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Closes the package panel.
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

        private void PopulatePackageView()
        {
            UpdateSideImage();
            UpdateSideIndicator();
        }

        private void PopulateInfoArea()
        {
            if (currentMail == null) return;

            // Sender info
            senderAddressText.text = string.IsNullOrEmpty(currentMail.senderAddress)
                ? "No return address"
                : currentMail.senderAddress;

            addressedNameText.text = string.IsNullOrEmpty(currentMail.addressedName)
                ? "(no addressee)"
                : currentMail.addressedName;

            // Postage
            countryOfOriginText.text = currentMail.countryOfOrigin.ToString();
            stampTypeText.text = currentMail.postageType.ToString().ToUpper();

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

            // Customs form
            customsDescriptionText.text = string.IsNullOrEmpty(currentMail.customsFormDescription)
                ? "No customs declaration"
                : currentMail.customsFormDescription;
        }

        private void UpdateSideImage()
        {
            if (currentMail == null) return;

            if (currentMail.packageSideSprites != null &&
                currentSideIndex < currentMail.packageSideSprites.Length &&
                currentMail.packageSideSprites[currentSideIndex] != null)
            {
                packageSideImage.sprite = currentMail.packageSideSprites[currentSideIndex];
                packageSideImage.enabled = true;
                packageSideImage.color = Color.white;
            }
            else
            {
                // No sprite — show a placeholder colour
                packageSideImage.sprite = null;
                packageSideImage.enabled = true;
                packageSideImage.color = new Color(0.76f, 0.6f, 0.42f); // cardboard brown
            }
        }

        private void UpdateSideIndicator()
        {
            string sideName = (currentSideIndex >= 0 && currentSideIndex < sideNames.Length)
                ? sideNames[currentSideIndex]
                : "Unknown";

            sideIndicatorText.text = sideName + " (" + (currentSideIndex + 1) + "/4)";
        }

        // =====================================================================
        // BUTTON HANDLERS
        // =====================================================================

        private void OnPrevSide()
        {
            currentSideIndex--;
            if (currentSideIndex < 0) currentSideIndex = 3;
            UpdateSideImage();
            UpdateSideIndicator();
        }

        private void OnNextSide()
        {
            currentSideIndex++;
            if (currentSideIndex > 3) currentSideIndex = 0;
            UpdateSideImage();
            UpdateSideIndicator();
        }

        private void OnOpenPackage()
        {
            if (packageOpened) return;

            packageOpened = true;

            // Hide the open button permanently for this mail item
            openPackageButton.gameObject.SetActive(false);

            // Show item inside
            itemInsidePanel.SetActive(true);

            if (currentMail.itemInsideSprite != null)
            {
                itemImage.sprite = currentMail.itemInsideSprite;
                itemImage.enabled = true;
            }
            else
            {
                itemImage.enabled = false;
            }

        }

        private void OnViewLetter()
        {
            if (currentMail == null || letterInspectionUI == null) return;

            // Hide this package panel temporarily
            gameObject.SetActive(false);

            // Open the letter panel with this package's letter data
            // When letter is closed, return to this package panel
            letterInspectionUI.Open(currentMail, fromPackage: true, onClose: () =>
            {
                gameObject.SetActive(true);
            });
        }

        private void OnCloseClicked()
        {
            Close();
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(OnCloseClicked);
            prevSideButton.onClick.RemoveListener(OnPrevSide);
            nextSideButton.onClick.RemoveListener(OnNextSide);
            openPackageButton.onClick.RemoveListener(OnOpenPackage);
            viewLetterButton.onClick.RemoveListener(OnViewLetter);
        }
    }
}