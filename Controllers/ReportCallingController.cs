using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UtilityManagement.Data;

public class ReportCallingController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReportCallingController(ApplicationDbContext context)
    {
        _context = context;
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
}