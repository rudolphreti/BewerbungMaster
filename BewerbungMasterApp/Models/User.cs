namespace BewerbungMasterApp.Models
{
    public class User
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LinkedIn { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public ApplicationText ApplicationText { get; set; } = new ApplicationText();
    }

    public class ApplicationText
    {
        public string Introduction { get; set; } = string.Empty;
        public string PositionIntro { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string Skills { get; set; } = string.Empty;
        public string InternshipOffer { get; set; } = string.Empty;
        public string Closing { get; set; } = string.Empty;
    }
}
