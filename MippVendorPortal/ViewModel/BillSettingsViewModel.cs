using MippVendorPortal.Models;
using System.ComponentModel.DataAnnotations;

namespace MippVendorPortal.ViewModel
{
    public class BillSettingsViewModel
    {

        public BillSettingsViewModel billSettingsViewModel { get; set; }
        public Bill bill { get; set; }
        public ClientSettings settings { get; set; }
        public BillSettingsViewModel()
        {
            //this.bill = bill;
            //this.settings = settings;

            //_bill = (IEnumerable<Bill>?)bill;
            //_settings = (IEnumerable<Settings>?)settings;
            //this.billSettingsViewModel = billSettingsViewModels;
        }

        [Key]
        public int? ClientId { get; set; }

        public class Bill
        {
            public int Id { get; set; }

            public string? BillId { get; set; }

            public string? Name { get; set; }

            public string? Description { get; set; }

            public string? Quantity { get; set; }

            public int? Unit { get; set; }

            public string? Price { get; set; }

            public string? Subtotal { get; set; }

            public string? Total { get; set; }
        }
        public class ClientSettings
        {
            public int Id { get; set; }

            public string? BusinessName { get; set; }

            public string? CareOf { get; set; }

            public string? Phone { get; set; }

            public string? Email { get; set; }

            public string? Address { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string Province { get; set; }
            public string Zip { get; set; }
            public string BillDate { get; set; }

            public string DueDate { get; set; }

            public int? ClientId { get; set; }

            public int? VendorId { get; set; }

            public static explicit operator ClientSettings(Setting? v)
            {
                throw new NotImplementedException();
            }
        }
    }

}
