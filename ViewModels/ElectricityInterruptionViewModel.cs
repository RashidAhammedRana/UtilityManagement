using UtilityManagement.Models;

namespace UtilityManagement.ViewModels
{
    public class ElectricityInterruptionViewModel
    {
        public TblElectricityInterruptionInfo Item { get; set; }

        public List<TblElectricityInterruptionInfo> Items { get; set; }
            = new List<TblElectricityInterruptionInfo>();
    }
}
