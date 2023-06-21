using System;
using System.Collections.Generic;

namespace MippPortalWebAPI.Models;

public partial class BillItemTax
{
    public int Id { get; set; }

    public int? BillItemId { get; set; }

    public int? TaxId { get; set; }
}
