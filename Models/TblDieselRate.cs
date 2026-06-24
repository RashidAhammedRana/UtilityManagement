using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblDieselRate
{
    public int Drid { get; set; }
    [Required(ErrorMessage = "Date is required")]
    public DateTime? TrDate { get; set; }
    [Required(ErrorMessage = "Rate is required")]
    public double? Rate { get; set; }
    [Required(ErrorMessage = "Status is required")]
    public string? Status { get; set; }

    public string? Remarks { get; set; }
}
