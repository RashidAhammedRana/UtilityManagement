using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UtilityManagement.Models;

namespace UtilityManagement.Models;

public partial class TblEquipmentDetail
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
    public virtual ICollection<TblRebReadingInfo> TblRebReadingInfos { get; set; } = new List<TblRebReadingInfo>();
    public virtual ICollection<TblNgGeneratorReadingInfo> TblNgGeneratorReadingInfos { get; set; } = new List<TblNgGeneratorReadingInfo>();
}
