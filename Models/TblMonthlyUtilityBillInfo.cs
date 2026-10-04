using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblMonthlyUtilityBillInfo
{
    public int Trid { get; set; }
    public DateOnly? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Comid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Year { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Month { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RebTotalKwh { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RebBill { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TitasCaptiveUse { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TitasCaptiveBill { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TitasIndustrialUse { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TitasIndustrialBill { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DieselTotalIssue { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DieselTotalBill { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CngTotalIssue { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CngTotalBill { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? LpgTotalIssue { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? LpgTotalBill { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? BiomassTotalIssue { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? BiomassTotalBill { get; set; }
    public double? TotalAmount { get; set; }

    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public virtual TblCompanyInfo? Com { get; set; }
}
