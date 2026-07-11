using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblCountryInfo
{
    public int Cntid { get; set; }
    [Display(Name ="Country Code")]
    public string? CntCode { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Country Name")]
    public string? CntName { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Status { get; set; }

    public string? Remarks { get; set; }
    public virtual ICollection<TblLoadChartMasterFile> TblLoadChartMasterFiles { get; set; } = new List<TblLoadChartMasterFile>();
}
