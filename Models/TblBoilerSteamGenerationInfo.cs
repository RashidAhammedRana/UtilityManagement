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
    [Required(ErrorMessage = "This field is required")]
    public double? GasPressure { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? HeaderSteamPressure { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Boiler1SteamGeneration { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Boiler2SteamGeneration { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Boiler3SteamGeneration { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? EgbBoilerSteamGeneration { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? TotalGeneration { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? B1UsageFuel { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? B2UsageFuel { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? B3UsageFuel { get; set; }
    public string? Remarks { get; set; }

    public string? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
