using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblGenRmsRoom
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateTime? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Eqid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? NgConsumption { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? NgTk { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ToalRh { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TkHr { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? NgConsumptionHr { get; set; }

    public string? Remarks { get; set; }

    public virtual TblEquipmentDetail? Eq { get; set; }
}
