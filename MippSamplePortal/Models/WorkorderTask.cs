using System;
using System.Collections.Generic;

namespace MippSamplePortal.Models;

public partial class WorkorderTask
{
    public int Id { get; set; }

    public int? WorkorderId { get; set; }

    public string? DescriptionOfWorkToComplete { get; set; }

    public string? ExpectedHours { get; set; }

    public string? AdditionalComment { get; set; }

    public bool? IsDelete { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifideBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
