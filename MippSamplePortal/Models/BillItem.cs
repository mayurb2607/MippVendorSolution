using System;
using System.Collections.Generic;

namespace MippSamplePortal.Models;

public partial class BillItem
{
    public int Id { get; set; }

    public string? BillId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Quantity { get; set; }

    public string? Price { get; set; }

    public string? Subtotal { get; set; }

    public string? Tax { get; set; }

    public string? Total { get; set; }

    public string? Unit { get; set; }
}
