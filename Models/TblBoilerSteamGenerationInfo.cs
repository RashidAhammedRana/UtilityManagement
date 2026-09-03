using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblBoilerSteamGenerationInfo
{
    public int Trid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateOnly? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public TimeOnly? Time { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Company { get; set; }
    public double? GasPressure { get; set; }
    public double? HeaderSteamPressure { get; set; }
    public double? Boiler1SteamGeneration { get; set; }
    public double? Boiler2SteamGeneration { get; set; }
    public double? Boiler3SteamGeneration { get; set; }
    public double? EgbBoilerSteamGeneration { get; set; }
    public double? TotalGeneration { get; set; }
    public string? B1UsageFuel { get; set; }
    public string? B2UsageFuel { get; set; }
    public string? B3UsageFuel { get; set; }
    public string? Remarks { get; set; }

    public string? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
