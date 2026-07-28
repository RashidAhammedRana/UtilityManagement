using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblChillerReadingInfo
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Eqid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateTime? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SteamCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SteamRate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SteamCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ChC114Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ChC114Cost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ChC2100bCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ChC2100bCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ChC2150Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ChC2150Cost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ChC317Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ChC317Cost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ChC615Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ChC615Cost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ServiceCharge { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? MaintenanceCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SparePartsCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TotalCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TotalCoolingWater { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CoolingCost { get; set; }

    public string? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual TblEquipmentDetail? Equipments { get; set; }
}
