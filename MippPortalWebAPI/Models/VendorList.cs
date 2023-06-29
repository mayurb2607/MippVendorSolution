using System;
using System.Collections.Generic;

namespace MippPortalWebAPI.Models;

public partial class VendorList
{
    public int Id { get; set; }

    public string? VendorName { get; set; }

    public string? VendorEmail { get; set; }

    public int? ClientId { get; set; }

    public string? Location { get; set; }

    public string? BusinessName { get; set; }

    public string? VendorPhone { get; set; }
}
