using System;
using System.Collections.Generic;

namespace MippSamplePortal.Models;

public partial class Vendor
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? BusinessName { get; set; }
}
