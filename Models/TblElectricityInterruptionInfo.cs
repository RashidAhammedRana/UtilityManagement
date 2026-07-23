using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblElectricityInterruptionInfo
{
    public int Eiid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int Comid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int Depid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int Itid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int Rid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateTime? Date { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateTime? PowerOffTime { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateTime? PowerOnTime { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DurationMin { get; set; }
    public string? Remarks { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public virtual TblDepartmentInfo? Department { get; set; }
    public virtual TblInterruptionTypeInfo? InterruptionType { get; set; }
    public virtual TblReasonInfo? Reason { get; set; }
    public virtual TblCompanyInfo? Company { get; set; }
}
