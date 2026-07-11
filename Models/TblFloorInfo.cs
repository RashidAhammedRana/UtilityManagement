using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblFloorInfo
{
    public int Flid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Comid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Bldid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Flname { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Status { get; set; }

    public string? Remarks { get; set; }

    public virtual TblBuildingInfo? Building { get; set; }

    public virtual TblCompanyInfo? Company { get; set; }
    public virtual ICollection<TblLoadChartMasterFile> TblLoadChartMasterFiles { get; set; } = new List<TblLoadChartMasterFile>();
}
