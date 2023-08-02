namespace MippVendorPortal.Models
{
    public class AcceptInvitationLoginModel
    {
        public int ClientId { get; set; }
        public int VendorId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
