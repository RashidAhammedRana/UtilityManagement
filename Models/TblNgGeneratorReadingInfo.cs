using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblNgGeneratorReadingInfo
{
    public int Trid { get; set; }

    public DateTime? Trdate { get; set; }
    [Required(ErrorMessage = "Equipment Name is required")]
    public int? Eqid { get; set; }
    [Required(ErrorMessage = "NG Cons.(m3) is required")]
    public double? NgConsumptionM { get; set; }
    [Required(ErrorMessage = "CNG Cons.(m3) is required")]
    public double? CngConsumption { get; set; }
    [Required(ErrorMessage = "E. Gen.(Kwh) is required")]
    public double? EGeneration { get; set; }
    [Required(ErrorMessage = "Running Hr is required")]
    public double? RunningHr { get; set; }
    [Required(ErrorMessage = "NG Cons./Hr is required")]
    public double? NgConsumptionHr { get; set; }
    [Required(ErrorMessage = "NG Cons./Kwh(m3/Kwh) Name is required")]
    public double? NgConsumptionKwh { get; set; }
    [Required(ErrorMessage = "Kwh/Per Unit NG Cons.(Kwh/m3) is required")]
    public double? KwhUnitNgConsumption { get; set; }
    [Required(ErrorMessage = "Lub Oil Cons. is required")]
    public double? LubOilConsumption { get; set; }
    [Required(ErrorMessage = "NG (Tk/m3) is required")]
    public double? NgTkM { get; set; }
    [Required(ErrorMessage = "Chemical Cost is required")]
    public double? ChemicalCost { get; set; }
    [Required(ErrorMessage = "Lub Oil (Tk/Ltr) is required")]
    public double? LubOilCost { get; set; }
    [Required(ErrorMessage = "CNG(Tk/m3) is required")]
    public double? CngCost { get; set; }
    [Required(ErrorMessage = "Troubleshooting is required")]
    public double? Troubleshooting { get; set; }
    [Required(ErrorMessage = "Service Charge is required")]
    public double? ServiceCharge { get; set; }
    [Required(ErrorMessage = "Repair Charge is required")]
    public double? RepairCharge { get; set; }
    [Required(ErrorMessage = "Total is required")]
    public double? Total { get; set; }
    [Required(ErrorMessage = "Tk/kwh is required")]
    public double? TkKwh { get; set; }

    public string? Remarks { get; set; }

    public virtual TblEquipmentDetail? Eq { get; set; }
}
