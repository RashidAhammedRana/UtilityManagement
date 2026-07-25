using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblSteamConsumptionReadingInfo
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateTime? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DyeingCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DyeingFinCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? WashingCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? GarmentsCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ChillerCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SeamlessDyeingCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SeamlessGarmentsCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? LabCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TotalCons { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
