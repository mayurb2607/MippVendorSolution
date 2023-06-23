using System;
using System.Collections.Generic;

namespace MippSamplePortal.Models;

public partial class WorkorderComment
{
    public int Id { get; set; }

    public int? WorkorderId { get; set; }

    public string? Email { get; set; }

    public string? Text { get; set; }
}
