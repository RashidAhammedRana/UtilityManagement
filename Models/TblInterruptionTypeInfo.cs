using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblInterruptionTypeInfo
{
    public int Itid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? InterruptionTypeName { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Status { get; set; }

    public string? Remarks { get; set; }

    public virtual ICollection<TblReasonInfo> TblReasonInfos { get; set; } = new List<TblReasonInfo>();
}
