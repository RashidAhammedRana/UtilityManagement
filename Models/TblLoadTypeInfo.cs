using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblLoadTypeInfo
{
    public int Ltid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Laod Type")]
    public string? Ltname { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Status { get; set; }

    public string? Remarks { get; set; }
}
