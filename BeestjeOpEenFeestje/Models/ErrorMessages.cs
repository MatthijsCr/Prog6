namespace BeestjeOpEenFeestje.Models
{
    public static class ErrorMessages
    {
        public const string RequiredField = "Dit veld is verplicht.";
        public const string InvalidPhoneNumber = "Voer een geldig telefoonnummer in.";
        public const string InvalidPrice = "Voer een geldige prijs in.";
        public const string MaxLengthAddress = "Adres" + MaxLength;
        public const string MaxLengthName = "Naam" + MaxLength;

        private const string MaxLength = " mag maximaal {0} tekens bevatten.";
    }
}
