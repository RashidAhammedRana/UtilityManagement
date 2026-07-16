using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UtilityManagement.Models;

namespace UtilityManagement.Models;

public partial class TblRebReadingInfo
{
    public int Trid { get; set; }

    public DateTime? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Eqid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ElecGen { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RunHr { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? BdtKwh { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? OtConsumable { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Troubleshoot { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ServiceCharge { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RepairCharge { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Total { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TkKwh { get; set; }
    public string? Remarks { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual TblEquipmentDetail? Eq { get; set; }
}
