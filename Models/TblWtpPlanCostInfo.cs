using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblWtpPlanCostInfo
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateTime? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Eqid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DeepPump1 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DeepPump2 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DeepPump3 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DeepPump4 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TotalDrawing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Softner1 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Softner2 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Softner3 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Softner4 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SoftnerGeneration { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? NaclConsumption { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? NaclCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Maintenance { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Kwh { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TotalCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TkMcSoftWater { get; set; }

    public double? Opt01 { get; set; }

    public double? Opt2 { get; set; }

    public double? Opt3 { get; set; }

    public double? Remarks { get; set; }

    public string? CraetedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual TblEquipmentDetail? Eq { get; set; }
}
