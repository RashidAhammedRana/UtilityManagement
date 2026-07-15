using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblEtpPlanCostInfo
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Eqid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateTime? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? UreaConsumption { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? UreaCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DecoloringConsumption { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DecoloringCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DapConsumption { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DapCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? PolymerConsumption { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? PolymerCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? MolassasesConsumption { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? MolassasesCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? AntifoamConsumption { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? AntifoamCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? H2so4Consumption { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? H2so4Cost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? BiocleanConsumption { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? BiocleanCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TotalChemicalCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? MaintenanceCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ManpowerSalary { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? MiscellaneousCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? GrandTotalCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DailyEffluentFlow { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? EffluentFlowThrough { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ChemicalCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DailyEtpCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DyeingProduction { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? EtpChemCostPerKgDyeing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? EflluentTreatmentPerKgDyeing { get; set; }

    public string? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual TblEquipmentDetail? Eq { get; set; }
}
