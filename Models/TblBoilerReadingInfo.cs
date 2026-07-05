using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblBoilerReadingInfo
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateTime? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Eqid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DslConLtr { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DslRh { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DslLtrHr { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? NgConM { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? NgRh { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? NgMHr { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CngConM { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CngRh { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CngMHr { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? LpgConKg { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? LpgRh { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? LpgKgHr { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? WaterInletLtr { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? StaemGenKg { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TotalRh { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SteamKgHr { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? NgTkM { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DslTkLtr { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CngTkM { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? LpgTkLtr { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ChemicalTk { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? OtherConsumable { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Maintenance { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Troubleshooting { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ServiceCharge { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Kwh { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Total { get; set; }

    [Required(ErrorMessage = "This field is required")]
    public double? TkKgSteamGenCost { get; set; }

    public string? Remarks { get; set; }

    public virtual TblEquipmentDetail? Eq { get; set; }
}
