using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblWtpWaterConsumptionInfo
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateTime? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Eqid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ConsumptionB1 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ConsumptionB7 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ConsumptionB11 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ConsumptionB12 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ConsumptionConstruction { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? BackWashWater { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SurplusRawWater { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TotalConsumptionRawWater { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ConsumptionD1 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ConsumptionD2 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ConsumptionSlitting { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ConsumptionFinishing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Washing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ConsumptionChiller { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ConsumptionGenerator { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ConsumptionSteam { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TotalConsumptionSoftWater { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ConDyeingHotWaterOut { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ConDyeingHotWaterIn { get; set; }
    public double? Opt01 { get; set; }

    public double? Opto2 { get; set; }

    public double? Opt03 { get; set; }

    public string? Remarks { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual TblEquipmentDetail? Eq { get; set; }
}
