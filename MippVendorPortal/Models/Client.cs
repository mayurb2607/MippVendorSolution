﻿using System;
using System.Collections.Generic;

namespace MippVendorPortal.Models;

public partial class Client
{
    public int Id { get; set; }

    public int? ClientId { get; set; }

    public string? ClientName { get; set; }

    public string? Email { get; set; }
}
