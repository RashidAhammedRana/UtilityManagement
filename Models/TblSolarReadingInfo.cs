using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblSolarReadingInfo
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateTime? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Eqid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ServiceChargeCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SparePartsCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? GenerationKwh { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? PerUnitGenCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TotalCost { get; set; }

    public string? Remarks { get; set; }

    public virtual TblEquipmentDetail? Eq { get; set; }
}
