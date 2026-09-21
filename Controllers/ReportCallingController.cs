using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UtilityManagement.Data;
using UtilityManagement.Models;

public class ReportCallingController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    public ReportCallingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    private async Task<bool> HasViewPermission(int menuId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return await (
            from up in _context.TblUserPermission
            join pa in _context.TblPermissionAction
                on up.ActionId equals pa.ActionId
            where up.UserId == userId
                  && up.MenuId == menuId
                  && up.IsAllowed
                  && pa.ActionName == "View"
            select up
        ).AnyAsync();
    }

    [HttpGet]
    public async Task<IActionResult> RebCostReport(
        int menuId,
        string reportName = "rptRebCost")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            // Logged-in user
            var userId = _userManager.GetUserId(User);

            // User-Company
            var currentCompany = await _context.Users
                .Where(x => x.Id == userId)
                .Select(x => x.Company)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(currentCompany))
            {
                return BadRequest("User company not found.");
            }

            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            // Company parameter
            var companyParameter = report.Parameters["Company"];

            if (companyParameter != null)
            {
                var lookupSettings = new StaticListLookUpSettings();

                lookupSettings.LookUpValues.Add(
                    new LookUpValue(currentCompany, currentCompany)
                );

                companyParameter.ValueSourceSettings = lookupSettings;

                companyParameter.Value = currentCompany;
                companyParameter.MultiValue = false;

                // If you don't want Company dropdown then set "false"
                companyParameter.Visible = true;
            }

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }



    [HttpGet]
    public async Task<IActionResult> NgGeneratorCostReport(
    int menuId,
    string reportName = "rptNgGeneratorCost")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> DieselGeneratorCostReport(
    int menuId,
    string reportName = "rptDieselGeneratorCost")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> SolarCostReport(
    int menuId,
    string reportName = "rptSolarCost")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> BoilerCostReport(
    int menuId,
    string reportName = "rptBoilerCost")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> ChillerCostReport(
    int menuId,
    string reportName = "rptChillerCost")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> AirCompressorCostReport(
    int menuId,
    string reportName = "rptAirCompressorCost")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> SteamConsumptionReport(
    int menuId,
    string reportName = "rptSteamConsumption")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> ElectricityConsumptionReport(
    int menuId,
    string reportName = "rptElectricityConsumption")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> EtpPlantCostReport(
    int menuId,
    string reportName = "rptEtpPlantCost")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> RoPlantCostReport(
    int menuId,
    string reportName = "rptRoPlantCost")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> WtpWaterConsumptionReport(
    int menuId,
    string reportName = "rptWtpWaterConsumption")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> RmsRoomReport(
    int menuId,
    string reportName = "rptRmsRoom")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EquipmentDetailsReport(
    int menuId,
    string reportName = "rptEquipmentDetails")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> DailyEnergyFuelConsumptionReport(
    int menuId,
    string reportName = "rptDailyEnergyFuelConsumption")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> DailyUtilityEnergyAndFuelConsumptionReport(
    int menuId,
    string reportName = "rptDailyEnergyPowerFuel")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> HourlyKwReport(
    int menuId,
    string reportName = "rptHourlyKW")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> BoilerSteamGenerationReport(
    int menuId,
    string reportName = "rptBoilerSteamGeneration")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> DailyAverageEnergyPowerFuelReport(
    int menuId,
    string reportName = "rptDailyEnergyPowerFuelAvg")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> DailyRMSRoomGasPressureReport(
    int menuId,
    string reportName = "rptDailyRmsRoomGasPressure")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> BiomassBoilerReport(
    int menuId,
    string reportName = "rptBiomassBoiler")
    {
        if (!await HasViewPermission(menuId))
        {
            return Forbid();
        }

        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";

            var reportType = Type.GetType(rptPath);

            if (reportType == null)
            {
                return NotFound($"Report '{reportName}' not found.");
            }

            var report = (XtraReport)Activator.CreateInstance(reportType);

            return View(report);
        }
        catch (Exception ex)
        {
            return Content(ex.InnerException?.Message ?? ex.Message);
        }
    }
}