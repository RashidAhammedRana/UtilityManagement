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
}