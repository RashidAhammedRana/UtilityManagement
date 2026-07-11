using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblBuildingInfo
{
    public int Bldid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Company")]
    public int? Comid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? BldName { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Status { get; set; }

    public string? Remarks { get; set; }

    public virtual TblCompanyInfo? Com { get; set; }

    public virtual ICollection<TblFloorInfo> TblFloorInfo { get; set; } = new List<TblFloorInfo>();
    public virtual ICollection<TblLoadChartMasterFile> TblLoadChartMasterFiles { get; set; } = new List<TblLoadChartMasterFile>();
}
