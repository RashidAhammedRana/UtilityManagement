using System;
using System.Collections.Generic;
using UtilityManagement.Models;

namespace UtilityManagement.Models;

public partial class TblRebReadingInfo
{
    public int Trid { get; set; }

    public DateTime? Trdate { get; set; }

    public int? Eqid { get; set; }

    public double? ElecGen { get; set; }

    public double? RunHr { get; set; }

    public double? BdtKwh { get; set; }

    public double? OtConsumable { get; set; }

    public double? Troubleshoot { get; set; }

    public double? ServiceCharge { get; set; }

    public double? RepairCharge { get; set; }

    public double? Total { get; set; }

    public double? TkKwh { get; set; }

    public string? Remarks { get; set; }

    public virtual TblEquipmentDetail? Eq { get; set; }
}
