using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblDailyFuelConsumption
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateOnly? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Company { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? NgGenerator { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? NgBoiler { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? NgDfma { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CngGenerator { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CngBoiler { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CngDfma { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DieselGenerator { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DieselBoiler { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DieselFl { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? LpgBoiler { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? NgTotal { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CngTotal { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DieselTotal { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? LpgTotal { get; set; }

    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
