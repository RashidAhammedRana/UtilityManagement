using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblLuboilRate
{
    public int Loid { get; set; }
    [Required(ErrorMessage = "Date is required")]
    public DateTime? TrDate { get; set; }
    [Required(ErrorMessage = "UOM is required")]
    public string? Uom { get; set; }
    [Required(ErrorMessage = "Rate is required")]
    public double? Rate { get; set; }
    [Required(ErrorMessage = "Status is required")]
    public string? Status { get; set; }

    public string? Remarks { get; set; }
}
