using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblSectionWiseMonthlyUtilityCost
{
    public int Trid { get; set; }

    public DateOnly? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Comid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Depid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Year { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Month { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ElectricityCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SteamCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? EtpCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? WtpCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? AcCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TotalCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Production { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? PerUnitProductionCost { get; set; }

    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public virtual TblCompanyInfo? Com { get; set; }
    public virtual TblDepartmentInfo? Dep { get; set; }
}
