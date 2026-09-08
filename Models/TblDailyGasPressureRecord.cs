using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblDailyGasPressureRecord
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Company { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateOnly? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public TimeOnly? Time { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? GpBefore { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? GpIr { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? GpCr { get; set; }
    public string? Remarks { get; set; }

    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
