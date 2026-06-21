using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblEquipmentDetails
{
    public int Eqid { get; set; }

    [Required(ErrorMessage = "Equipment Name is required")]
    public string? EquipmentName { get; set; }

    [Required(ErrorMessage = "Capacity is required")]
    public string? Capacity { get; set; }

    [Required(ErrorMessage = "Brand is required")]
    public string? Brand { get; set; }

    [Required(ErrorMessage = "Model is required")]
    public string? Model { get; set; }

    [Required(ErrorMessage = "SL No is required")]
    public string? Slno { get; set; }

    [Required(ErrorMessage = "Current Location is required")]
    public string? CurrentLocation { get; set; }
}
