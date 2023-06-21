using System;
using System.Collections.Generic;

namespace MippVendorPortal.Models;

public partial class Setting
{
    public int Id { get; set; }

    public int? VendorId { get; set; }

    public string? BusinessName { get; set; }

    public string? CareOf { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? City { get; set; }

    public string? Province { get; set; }

    public string? Zip { get; set; }

    public string? BillDate { get; set; }

    public string? DueDate { get; set; }
}
