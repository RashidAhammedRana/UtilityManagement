using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblRoPlantCostInfo
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateTime? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Eqid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Doshion51Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Doshion51Cost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Doshion52Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Doshion52Cost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DoScale65Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DoScale65Cost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DailyRunningHour { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TotalChemicalCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ServiceCharge { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? MaintenanceCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ManpowerSalary { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? GrandTotalCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? EffFlowRoPlant { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? EffFlowRoSoft { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RoSoftWaterFlow { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? EffFlowRoRejection { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ChemCostEffTreatment { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DailyRoCost { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual TblEquipmentDetail? Eq { get; set; }
}
