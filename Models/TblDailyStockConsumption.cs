using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblDailyStockConsumption
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateOnly? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? OsDiesel { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ReceiveDiesel { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ConsDiesel { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CsDiesel { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? OsLps { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ReceiveLpg { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ConsLpg { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CsLpg { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? OsCng { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ReceiveCng { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ConsCng { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CsCng { get; set; }

    public DateTime? CreatedtAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
