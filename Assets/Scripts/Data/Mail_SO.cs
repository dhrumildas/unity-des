using UnityEngine;

namespace MailSorting.Data
{
    [CreateAssetMenu(fileName = "NewMail", menuName = "Mail Sorting/Mail", order = 2)]
    public class Mail_SO : ScriptableObject
    {
        //[Header("Identity")]
        //[Tooltip("Unique identifier e.g. LTR_001, PKG_005")]
        //public string mailID;

        [Tooltip("Letter or Package")]
        public MailType mailType = MailType.Letter;

        //[Tooltip("Which day this mail can appear on")]
        //[Range(1, 7)]
        //public int appearanceDay = 1;

        [Header("Narrative Tracking")]
        [Tooltip("Who sent this? Used for triggering specific endings.")]
        public CharacterSender senderProfile = CharacterSender.Generic;

        [Tooltip("Guaranteed story spawn or random pool filler")]
        public MailSpawnType spawnType = MailSpawnType.Random;

        [Header("Sender Information")]
        [Tooltip("Name of the person who sent this mail")]
        public string senderName;

        [Tooltip("Return address displayed on the envelope")]
        public string senderAddress;

        //[Tooltip("ID linking to a recurring fan profile (empty if one-off sender)")]
        //public string fanProfileID;

        [Header("Letter Content")]
        [TextArea(4, 10)]
        [Tooltip("How the sender addressed AVA(shown at the mail - cover sprite when it is spawned BEFORE opening it")]
        public string companyAddress;

        [Tooltip("How the sender addressed Aurora (displayed text the player reads)")]
        public string howAuroraAddressed;  //this has to be changed like content

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
        [Tooltip("How many questions are in this letter?")]
        [Range(0, 10)]
        public int questionCount = 0;

        //[Header("Letter Content ? Images")]
        //[Tooltip("Image displayed within the letter body (inline alongside text, or full-page if letterContent is empty)")]
        //public Sprite letterContentImage;

        //[Tooltip("Is the letter content encrypted/encoded? (cipher, binary, etc.) Purely informational for narrative.")]
        //public bool isEncrypted = false;

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

        //[Tooltip("Description of what is actually inside the package")]
        //public string itemInsideDescription;

        //[Tooltip("Sprite shown when package is opened or x-rayed")]
        //public Sprite itemInsideSprite;

        //[Tooltip("What the customs form claims is inside")]
        //public string customsFormDescription;

        [Header("Contraband & Danger")]
        [Tooltip("Does this mail contain contraband?")]
        public bool containsContraband = false;

        //[Tooltip("What type of contraband")]
        //public ContrabandType contrabandType = ContrabandType.None;

        [Tooltip("Does this contain metal?")]
        public bool containsMetal = false;

        [Tooltip("Does this contain a suspicious substance?")]
        public bool containsSubstance = false;

        //[Tooltip("What type of substance")]
        //public SubstanceType substanceType = SubstanceType.None;

        [Tooltip("Does the written content violate safety policies?")]
        public bool containsOffence = false;

        //[Tooltip("What type of written offence?")]
        //public LetterOffenceType offenceType = LetterOffenceType.None;


        [Tooltip("Monetary value of gift inside")]
        [Range(0, 10000)]
        public int giftValue = 0;

        [Header("Visual Assets")]
        [Tooltip("Sprite for the envelope front or package main face")]
        public Sprite mailSprite;   /*this *should be the default closed envelope or package sprite that is spawned and later clicked to open the content gameObject
                                     Now the idea is this would cover the face of the cover and I have already made changes in NewDragCheck to change the size of box
                                    collider acc to the sr. This is the face of the mail which could be A) Letter or B) A package, of different widths since we are not
                                    measuring heights of the mail (also need to change for dimensions)*/

        //Note : This face will only contain the address to the AVA company to check if the sender has addressed correctly or not. 

        [Tooltip("Sprite for the opened letter / back of envelope")]
        public Sprite contentSprite;    /*as mentioned the content sprite, this is now the letter itself which would be revealed when the mail is opened (I AM HOPING*/

        [Tooltip("Sprite for the postage stamp area")]
        public Sprite postageStampSprite;

        //[Tooltip("Additional visual elements: stickers, stains, markings")]
        //public Sprite[] visualClues;


        //public string GetDebugInfo()
        //{
        //    //string validation = ValidateIdealAction() ? "VALID" : "MISMATCH ? computed: " + ComputeIdealAction();
        //    string contentMode = string.IsNullOrEmpty(letterContent)
        //        ? (letterContentImage != null ? "IMAGE-ONLY" : "EMPTY")
        //        : (letterContentImage != null ? "TEXT+IMAGE" : "TEXT-ONLY");

        //    return $"{mailType} from {senderName} [{spawnType}]\n" +
        //           $"Addressed: \"{companyAddress}\" ({(addressedCorrectly ? "OK" : "WRONG")})\n" +
        //           $"Signed: \"{signatureName}\" ({(signedCorrectly ? "OK" : "WRONG")})\n" +
        //           $"Sentences: {sentenceCount}, Question: {containsQuestion}\n" +
        //           $"Content Mode: {contentMode}\n" +
        //           $"Contraband: {containsContraband}\n" +
        //           $"Substance: {containsSubstance}\n" +
        //           $"Weight: {weight}g, Dims: {dimensions}cm\n" +
        //           $"Country: {countryOfOrigin}, Postage: {postageType}\n]";
        //    //$"Reply: {replyCategory}\n" +
        //    //$"Ideal Action: {idealAction} [{validation}]\n" +
        //    //$"Broken Rules: [{string.Join(", ", brokenRuleIDs ?? new string[0])}]";
        //}
    }
}