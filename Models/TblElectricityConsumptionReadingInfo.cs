using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblElectricityConsumptionReadingInfo
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateTime? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Company { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DyeingCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DyeingFinCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TestingLabCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? WashingCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? GmntsFinCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CuttingCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? AllUtilityCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? PrintingHeatsealCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? EmbroideryCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? KnittingCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SeamlessKnitCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SeamlessDyeCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SeamlessGmntCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? AllOfficeCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? OthersAreaCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TotalCons { get; set; }

    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
