using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblReasonInfo
{
    public int Rid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int Itid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string ReasonName { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string Status { get; set; }

    public string? Remarks { get; set; }

    public virtual TblInterruptionTypeInfo? It { get; set; }
    public virtual ICollection<TblElectricityInterruptionInfo> TblElectricityInterruptionInfo { get; set; } = new List<TblElectricityInterruptionInfo>();

}
