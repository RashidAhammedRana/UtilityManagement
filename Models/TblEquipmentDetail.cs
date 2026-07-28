using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UtilityManagement.Models;

namespace UtilityManagement.Models;

public partial class TblEquipmentDetail
{
    public int Eqid { get; set; }

    [Required(ErrorMessage = "Equipment Name is required")]
    public string? EquipmentName { get; set; }

    [Required(ErrorMessage = "Capacity is required")]
    public string? Capacity { get; set; }

    [Required(ErrorMessage = "Brand is required")]
    public string? Brand { get; set; }

    [Required(ErrorMessage = "Model is required")]
    public string? Model { get; set; }

    [Required(ErrorMessage = "SL No is required")]
    public string? Slno { get; set; }
    [Required(ErrorMessage = "Date is required")]
    public DateTime? InstallDate { get; set; }

    [Required(ErrorMessage = "Current Location is required")]
    public string? CurrentLocation { get; set; }
    [Required(ErrorMessage = "Status is required")]
    public string? Status { get; set; }
    public string? Remarks { get; set; }

    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public virtual ICollection<TblRebReadingInfo> TblRebReadingInfos { get; set; } = new List<TblRebReadingInfo>();
    public virtual ICollection<TblNgGeneratorReadingInfo> TblNgGeneratorReadingInfos { get; set; } = new List<TblNgGeneratorReadingInfo>();
    public virtual ICollection<TblDiselGeneratorReadingInfo> TblDiselGeneratorReadingInfos { get; set; } = new List<TblDiselGeneratorReadingInfo>();
    public virtual ICollection<TblSolarReadingInfo> TblSolarReadingInfos { get; set; } = new List<TblSolarReadingInfo>();
    public virtual ICollection<TblBoilerRmsRoom> TblBoilerRmsRooms { get; set; } = new List<TblBoilerRmsRoom>();
    public virtual ICollection<TblGenRmsRoom> TblGenRmsRooms { get; set; } = new List<TblGenRmsRoom>();
    public virtual ICollection<TblBoilerReadingInfo> TblBoilerReadingInfo { get; set; } = new List<TblBoilerReadingInfo>();
    public virtual ICollection<TblWtpPlanCostInfo> TblWtpPlanCostInfos { get; set; } = new List<TblWtpPlanCostInfo>();
    public virtual ICollection<TblWtpWaterConsumptionInfo> TblWtpWaterConsumptionInfos { get; set; } = new List<TblWtpWaterConsumptionInfo>();
    public virtual ICollection<TblEtpPlanCostInfo> TblEtpPlanCostInfo { get; set; } = new List<TblEtpPlanCostInfo>();
    public virtual ICollection<TblRoPlantCostInfo> TblRoPlantCostInfos { get; set; } = new List<TblRoPlantCostInfo>();
    public virtual ICollection<TblChillerReadingInfo> TblChillerReadingInfos { get; set; } = new List<TblChillerReadingInfo>();


}
