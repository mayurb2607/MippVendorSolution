using System;
using System.Collections.Generic;

namespace MippPortalWebAPI.Models;

public partial class ClientStatus
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public string? Status { get; set; }
}
