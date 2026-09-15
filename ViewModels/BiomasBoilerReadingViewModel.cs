using Microsoft.AspNetCore.Mvc.Rendering;
using UtilityManagement.Models;

namespace UtilityManagement.ViewModels
{
    public class BiomasBoilerReadingViewModel
    {
        public TblBiomasBoilerReading BiomasBoilerReading { get; set; }

        public double? JuteRate { get; set; }
        public double? RiceHuskRate { get; set; }
        public double? WasteWoodRate { get; set; }
        public double? CartonRate { get; set; }
        public double? CharcoilRate { get; set; }

        public List<SelectListItem> EquipmentList { get; set; } = new();
    }
}
