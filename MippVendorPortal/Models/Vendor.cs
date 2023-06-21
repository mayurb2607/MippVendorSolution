using System;
using System.Collections.Generic;

namespace MippVendorPortal.Models;

public partial class Vendor
{
    public int Id { get; set; }

    public string? VendorEmail { get; set; }

    public string? VendorCompany { get; set; }

    public string? VendorPhone { get; set; }

    public int? RootVendorId { get; set; }
}
