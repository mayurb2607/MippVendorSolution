namespace MippVendorPortal.Helpers
{
    public class WorkorderRequest
    {
        public int? ClientID { get; set; }
        public int? Id { get; set; }
        public string? Status { get; set; }
        public string? AdditionalComments { get; set; }
        public int? VendorID { get; set; }
    }
}
