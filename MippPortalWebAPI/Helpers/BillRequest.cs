namespace MippPortalWebAPI.Helpers
{
    public class BillRequest
    {
        public int? Id { get; set; }
        public int BillId { get; set; }
        public int ClientID { get; set; }
        public int VendorID { get; set; }
    }
}
