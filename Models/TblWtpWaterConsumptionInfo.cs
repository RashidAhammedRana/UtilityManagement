using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblWtpWaterConsumptionInfo
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateTime? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Eqid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RawConsGarments { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RawConsDyeing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RawConsDyeingFin { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RawConsPrinting { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RawConsUtilityArea { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RawConsWashing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RawConsSeamlessDyeing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RawConsLab { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RawConsGardening { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RawConsWashroomOthers { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RawConsKnittingArea { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RawConsOthersArea { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RawConsBackWash { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RawConsTotal { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SoftConsDyeing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SoftConsDyeingFin { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SoftConsWashing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SoftConsSeamlessDyeing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SoftConsLab { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SoftConsOthersArea { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SoftConsTotal { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RoConsDyeing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RoConsDyeingFin { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RoConsWashing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RoConsSeamlessDyeing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RoConsLab { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RoConsOthersArea { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RoConsTotal { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? HotConsDyeing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? HotConsDyeingFin { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? HotConsWashing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? HotConsSeamlessDyeing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? HotConsLab { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? HotConsOthersArea { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? HotWaterReturn { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? HotConsTotal { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public virtual TblEquipmentDetail? Eq { get; set; }
}
