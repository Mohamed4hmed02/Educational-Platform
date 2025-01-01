namespace Educational_Platform.Domain.Models
{
    public class PaymentConfirmationDataModel
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Specialization { get; set; }
        public string? GmailAccount { get; set; }
        public string? WhatsappNumber { get; set; }
        public string? Region { get; set; }
        public string? Governorate { get; set; }
        public string? CountryName { get; set; }
        public bool IsFromEgypt => Region == "Egypt";
    }
}
