using System;
using System.Collections.Generic;

namespace MippPortalWebAPI.Models;

public partial class Tax
{
    public int Id { get; set; }

    public int? ClientId { get; set; }

    public string? TaxRate { get; set; }
}
