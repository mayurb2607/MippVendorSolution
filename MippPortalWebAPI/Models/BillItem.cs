using System;
using System.Collections.Generic;

namespace MippPortalWebAPI.Models;

public partial class BillItem
{
    public int Id { get; set; }

    public int? ProductAndServiceId { get; set; }

    public int? BillId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Quantity { get; set; }

    public string? Unit { get; set; }

    public decimal? Price { get; set; }

    public decimal? Subtotal { get; set; }

    public int? TaxId1 { get; set; }

    public decimal? Tax1 { get; set; }

    public int? TaxId2 { get; set; }

    public decimal? Tax2 { get; set; }

    public decimal? Total { get; set; }
}
