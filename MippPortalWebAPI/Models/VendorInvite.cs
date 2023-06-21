using System;
using System.Collections.Generic;

namespace MippPortalWebAPI.Models;

public partial class VendorInvite
{
    public int Id { get; set; }

    public int? ClientId { get; set; }

    public int? VendorId { get; set; }

    public string? InviteSentDate { get; set; }

    public string? JoinedDate { get; set; }

    public string? VendorEmail { get; set; }
}
