using System;
using System.Collections.Generic;

namespace MippSamplePortal.Models;

public partial class WorkorderWorkDescription
{
    public int Id { get; set; }

    public int? WorkorderId { get; set; }

    public string? DescriptionOfWorkCompletedMaterialsUsed { get; set; }

    public string? HoursSpent { get; set; }
}
