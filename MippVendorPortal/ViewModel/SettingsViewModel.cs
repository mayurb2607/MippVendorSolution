namespace MippVendorPortal.ViewModel
{
    public class SettingsViewModel
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Id { get; set; }
        public string BusinessName { get; set; }

        public string CareOf { get; set; }
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string Province { get; set; }

        public string Zip { get; set; }

        public string BillDate { get; set; }

        public string? DueDate { get; set; }
    }
}
