using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblLoadChartMasterFile
{
    public int Trid { get; set; }

    public DateTime? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name ="Company")]
    public int? Comid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Building")]
    public int? Bldid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Floor")]
    public int? Flid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Load Type")]
    public int? Ltid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Description { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Type { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Country/Origien")]
    public int? Cntid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Brand")]
    public int? Brndid { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Model { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "SI")]
    public string? Si { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Capacity { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Comissioning Date")]
    public DateTime? CmsnDate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Watt { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Qty { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Sub Total Watt")]
    public int? SubTotalWatt { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public int? Volt { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "PF")]
    public double? Pf { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Amp(Single Phase)")]
    public double? AmpSignalPhase { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Amp(Three Phase)")]
    public double? AmpThreePhase { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Sub Total(Kw 400v)")]
    public double? SubTotalKw400v { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Total Load(Kw)")]
    public double? TotalLoadKw { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Standby Load(Kw)")]
    public double? StandbyLoadKw { get; set; }
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Total Load W/O Standby load(Kw)")]
    public double? TotalLoadWithoutStandby { get; set; }

    public string? Remarks { get; set; }

    public virtual TblBrandInfo? Brnd { get; set; }

    public virtual TblCountryInfo? Cnt { get; set; }

    public virtual TblFloorInfo? Fl { get; set; }

    public virtual TblLoadTypeInfo? Lt { get; set; }
    public virtual TblCompanyInfo? Company { get; set; }
    public virtual TblBuildingInfo? Building { get; set; }
}
