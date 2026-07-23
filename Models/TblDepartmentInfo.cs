using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblDepartmentInfo
{
    public int Depid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string DepartmentName { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string Status { get; set; }

    public string? Remarks { get; set; }
}
