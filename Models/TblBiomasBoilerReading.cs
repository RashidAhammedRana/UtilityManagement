using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblBiomasBoilerReading
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateOnly? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Eqid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Company { get; set; }

    public double? JuteCons { get; set; }

    public double? JuteCost { get; set; }

    public double? RhCons { get; set; }

    public double? RhCost { get; set; }

    public double? WwCons { get; set; }

    public double? WwCost { get; set; }

    public double? CartonCons { get; set; }

    public double? CartonCost { get; set; }

    public double? CcCons { get; set; }

    public double? CcCost { get; set; }

    public double? TotalBioCost { get; set; }

    public double? LabourCost { get; set; }

    public double? ElectricityCost { get; set; }

    public double? MaintenanceCost { get; set; }

    public double? OtherCost { get; set; }

    public double? TotalCost { get; set; }

    public double? WaterCons { get; set; }

    public double? SteamGeneration { get; set; }

    public double? CostPerKgSteam { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual TblEquipmentDetail? Eq { get; set; }
}
