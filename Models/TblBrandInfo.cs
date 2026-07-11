using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblBrandInfo
{
    public int Brndid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name ="Brand")]
    public string? BrndName { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Status { get; set; }

    public string? Remarks { get; set; }
}
