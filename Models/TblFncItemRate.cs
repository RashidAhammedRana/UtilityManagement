using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblFncItemRate
{
    public int Fncrid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Fncid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public DateTime? Date { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? Rate { get; set; }

    public string? Remarks { get; set; }

    public virtual TblFncItem? Fnc { get; set; }
}
