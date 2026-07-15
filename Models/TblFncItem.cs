using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UtilityManagement.Models;

namespace UtilityManagement.Models;

public partial class TblFncItem
{
    public int Fncid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? ItemName { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Uom { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Status { get; set; }

    public virtual ICollection<TblFncItemRate> TblFncItemRates { get; set; } = new List<TblFncItemRate>();
}
