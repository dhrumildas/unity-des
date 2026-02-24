namespace MailSorting.Data
{
      public enum MailType
    {
        Letter,
        Package
    }
    public enum MailAction
    {
        Accept,     // Send to Aurora
        Reply,      // Minor issue - sender can try again
        Reject,     // Too many issues - rejected outright
        Report      // Dangerous/policy violation - escalate to safety
    }
    public enum PostageType
    {
        Standard,
        Express,
        Fake
    }
    public enum ContrabandType
    {
        None,
        Drugs,
        Substances,
        NSFW,
        Food,
        Dangerous
    }
    public enum Country
    {
        GBR,
        JPN,
        USA,
        DEU,
        FRA,
        CAN,
        AUS,
        IND
    }
    public enum NarrativeImportance
    {
        Generic,        // Filler mail
        Character,      // Develops a recurring character
        Story,          // Advances the main story
        Critical        // Major plot point
    }
    public enum SubstanceType
    {
        None,
        Chemical,
        Biological,
        Powder,
        Liquid,
        Unknown
    }
    public enum ReplyCategory
    {
        Supportive,     // Fan inspired by Aurora
        Nice,           // General positive fan mail
        Pet,            // Fan's pet likes Aurora's music
        HateMail,       // Negative mail - reply from Paris Scarrott
        Custom          // Uses replyTextOverride instead of a template
    }
    public enum MailSpawnType
    {
        Guaranteed,     // Always spawns on its appearance day
        Random          // Drawn randomly from the pool to fill the shift
    }

    public enum LetterOffenceType
    {
        None,
        Threatening,
        Stalking,
        Inappropriate,
        Other
    }
}