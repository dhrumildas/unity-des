using UnityEngine;

namespace MailSorting.Data
{
    [CreateAssetMenu(fileName = "NewMailItem", menuName = "Mail Sorting/Mail Item", order = 1)]
    public class Mail_Items_SO : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Unique identifier e.g. LTR_001, PKG_005")]
        public string mailID;

        [Tooltip("Letter or Package")]
        public MailType mailType = MailType.Letter;

        [Tooltip("Which day this mail can appear on")]
        [Range(1, 7)]
        public int appearanceDay = 1;

        [Tooltip("Narrative importance of this mail")]
        public NarrativeImportance importance = NarrativeImportance.Generic;

        [Tooltip("Guaranteed story spawn or random pool filler")]
        public MailSpawnType spawnType = MailSpawnType.Random;

        [Header("Sender Information")]
        [Tooltip("Name of the person who sent this mail")]
        public string senderName;

        [Tooltip("Return address displayed on the envelope")]
        public string senderAddress;

        [Tooltip("ID linking to a recurring fan profile (empty if one-off sender)")]
        public string fanProfileID;

        [Header("Letter Content")]
        [Tooltip("How the sender addressed Aurora (displayed text the player reads)")]
        public string addressedName;

        [Tooltip("Does the addressing pass the rule check?")]
        public bool addressedCorrectly = true;

        [Tooltip("How the sender signed the letter (displayed text)")]
        public string signatureName;

        [Tooltip("Does the signature pass the rule check?")]
        public bool signedCorrectly = true;

        [Tooltip("The body text of the letter. Leave empty if the letter is image-only.")]
        [TextArea(5, 15)]
        public string letterContent;

        [Tooltip("Number of sentences in the letter (pre-counted)")]
        [Range(0, 20)]
        public int sentenceCount = 3;

        [Tooltip("Does the letter contain a question?")]
        public bool containsQuestion = false;

        [Header("Letter Content — Images")]
        [Tooltip("Image displayed within the letter body (inline alongside text, or full-page if letterContent is empty)")]
        public Sprite letterContentImage;

        [Tooltip("Is the letter content encrypted/encoded? (cipher, binary, etc.) Purely informational for narrative.")]
        public bool isEncrypted = false;

        [Header("Envelope / Exterior")]
        [Tooltip("Country of origin (3-letter code)")]
        public Country countryOfOrigin = Country.GBR;

        [Tooltip("Type of postage on the mail")]
        public PostageType postageType = PostageType.Standard;

        [Header("Physical Properties")]
        [Tooltip("Weight in grams")]
        [Range(0f, 5000f)]
        public float weight = 50f;

        [Tooltip("Dimensions in cm (width x height)")]
        public Vector2 dimensions = new Vector2(21f, 15f);

        [Header("Package-Specific (ignore for letters)")]
        [Tooltip("Sprites for the 4 sides of the package: Front, Right, Back, Left")]
        public Sprite[] packageSideSprites = new Sprite[4];

        [Tooltip("Description of what is actually inside the package")]
        public string itemInsideDescription;

        [Tooltip("Sprite shown when package is opened or x-rayed")]
        public Sprite itemInsideSprite;

        [Tooltip("What the customs form claims is inside")]
        public string customsFormDescription;

        [Header("Contraband & Danger")]
        [Tooltip("Does this mail contain contraband?")]
        public bool containsContraband = false;

        [Tooltip("What type of contraband")]
        public ContrabandType contrabandType = ContrabandType.None;

        [Tooltip("Does this contain metal?")]
        public bool containsMetal = false;

        [Tooltip("Does this contain a suspicious substance?")]
        public bool containsSubstance = false;

        [Tooltip("What type of substance")]
        public SubstanceType substanceType = SubstanceType.None;

        [Header("Letter-Specific Offences")]
        [Tooltip("Does the written content violate safety policies?")]
        public bool containsOffence = false;

        [Tooltip("What type of written offence?")]
        public LetterOffenceType offenceType = LetterOffenceType.None;


        [Tooltip("Monetary value of gift inside")]
        [Range(0, 10000)]
        public int giftValue = 0;

        [Header("Visual Assets")]
        [Tooltip("Sprite for the envelope front or package main face")]
        public Sprite mailSprite;

        [Tooltip("Sprite for the opened letter / back of envelope")]
        public Sprite contentSprite;

        [Tooltip("Sprite for the postage stamp area")]
        public Sprite postageStampSprite;

        [Tooltip("Additional visual elements: stickers, stains, markings")]
        public Sprite[] visualClues;

        [Header("Reply System")]
        [Tooltip("Which pre-written Aurora reply template to use")]
        public ReplyCategory replyCategory = ReplyCategory.Nice;

        [Tooltip("Custom reply text — only used when replyCategory is set to Custom")]
        [TextArea(2, 5)]
        public string replyTextOverride;


        [Header("Rule Evaluation")]
        [Tooltip("The correct action for this mail item — set manually")]
        public MailAction idealAction = MailAction.Accept;

        [Tooltip("List of rule IDs this mail violates e.g. ADDR_WRONG, SIGN_WRONG, SENTENCE_OVER, CONTRABAND, SUBSTANCE, SUSPICIOUS_OFFENCE")]
        public string[] brokenRuleIDs;

        public bool ValidateIdealAction()
        {
            return idealAction == ComputeIdealAction();
        }

        public MailAction ComputeIdealAction()
        {
            if (containsContraband) return MailAction.Report;
            if (containsSubstance) return MailAction.Report;
            if (containsOffence) return MailAction.Report;

            int violations = 0;

            if (!addressedCorrectly) violations++;
            if (!signedCorrectly) violations++;
            if (sentenceCount > 4) violations++;

            if (violations == 0)
                return MailAction.Accept;
            else if (violations == 1)
                return MailAction.Reply;
            else
                return MailAction.Reject;
        }

        public string GetReplyText()
        {
            if (replyCategory == ReplyCategory.Custom && !string.IsNullOrEmpty(replyTextOverride))
                return replyTextOverride;

            // For non-custom categories, the ReplyManager/UI system handles template lookup
            return null;
        }

        public string GetDebugInfo()
        {
            string validation = ValidateIdealAction() ? "VALID" : "MISMATCH — computed: " + ComputeIdealAction();
            string contentMode = string.IsNullOrEmpty(letterContent)
                ? (letterContentImage != null ? "IMAGE-ONLY" : "EMPTY")
                : (letterContentImage != null ? "TEXT+IMAGE" : "TEXT-ONLY");

            return $"[{mailID}] {mailType} from {senderName} [{spawnType}]\n" +
                   $"Addressed: \"{addressedName}\" ({(addressedCorrectly ? "OK" : "WRONG")})\n" +
                   $"Signed: \"{signatureName}\" ({(signedCorrectly ? "OK" : "WRONG")})\n" +
                   $"Sentences: {sentenceCount}, Question: {containsQuestion}\n" +
                   $"Content Mode: {contentMode}, Encrypted: {isEncrypted}\n" +
                   $"Contraband: {containsContraband} ({contrabandType})\n" +
                   $"Substance: {containsSubstance} ({substanceType})\n" +
                   $"Weight: {weight}g, Dims: {dimensions}cm\n" +
                   $"Country: {countryOfOrigin}, Postage: {postageType}\n" +
                   $"Reply: {replyCategory}\n" +
                   $"Ideal Action: {idealAction} [{validation}]\n" +
                   $"Broken Rules: [{string.Join(", ", brokenRuleIDs ?? new string[0])}]";
        }
    }
}