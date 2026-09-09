using UtilityManagement.Models;

namespace UtilityManagement.ViewModels
{
    public class DailyGasPressureRecordViewModel
    {
        public List<TblDailyGasPressureRecord> Items { get; set; }
            = new List<TblDailyGasPressureRecord>();
    }
}
