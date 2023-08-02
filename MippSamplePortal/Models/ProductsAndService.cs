using System;
using System.Collections.Generic;

namespace MippSamplePortal.Models;

public partial class ProductsAndService
{
    public int Id { get; set; }

    public int? ClientId { get; set; }

    public string? ItemName { get; set; }

    public string? Description { get; set; }

    public string? Unit { get; set; }

    public decimal? Price { get; set; }

    public int? TaxId2 { get; set; }

    public int? TaxId1 { get; set; }

    public bool? IsDelete { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifideBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
