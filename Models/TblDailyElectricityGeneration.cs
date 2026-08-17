using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblDailyElectricityGeneration
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateOnly? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Reb { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Gg1 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Gg2 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Gg3 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Gg4 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Dg1 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Dg2 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Dg3 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Dg4 { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Solar { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Total { get; set; }

    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
