using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblDiselGeneratorReadingInfo
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateTime? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Eqid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DieselConsumptionLtr { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ElectricityGenerationKwh { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RunningHr { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DieselConsumptionHr { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DieselConsumptionKwh { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? LubOilConsumption { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DieselTkLtr { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Chemical { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? LubOilTkLtr { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? OthersConsumable { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Troubleshooting { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ServiceCharge { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RepairCharge { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Total { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TkKwh { get; set; }

    public string? Remarks { get; set; }

    public virtual TblEquipmentDetail? Eq { get; set; }
}
