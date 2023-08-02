namespace MippPortalWebAPI.Models
{
    public class SendEmailViewModel
    {
        public int? ClientID { get; set; }
        public int? VendorID { get; set; }
        public string? ToEmail { get; set; }
        public string? FromEmail { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public List<string>? Cc { get; set; }
    }
}
