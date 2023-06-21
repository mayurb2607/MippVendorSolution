using System;
using System.Collections.Generic;

namespace MippVendorPortal.Models;

public partial class VendorClient
{
    public int Id { get; set; }

    public int VendorId { get; set; }

    public int ClientId { get; set; }
}
