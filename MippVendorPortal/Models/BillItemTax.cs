using System;
using System.Collections.Generic;

namespace MippVendorPortal.Models;

public partial class BillItemTax
{
    public int Id { get; set; }

    public int? BillItemId { get; set; }

    public int? TaxId { get; set; }
}
