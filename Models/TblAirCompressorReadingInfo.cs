using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblAirCompressorReadingInfo
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateTime? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Eqid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? RunningHour { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? PowerConsumption { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ElectricityRate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ElectricityCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? ServiceMaintenanceCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TotalCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? AirFlow { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? AirProduced { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CostPerM3Air { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public virtual TblEquipmentDetail? Equipments { get; set; }
}
