//using DevExpress.XtraReports.UI;
//using Microsoft.AspNetCore.Mvc;

//public class ReportCallingController : Controller
//{
//    public IActionResult RebCostReport(string reportName = "rptShiftWiseCounterMetrics")
//    {
//        try
//        {
//            var rptPath = $"UtilityManagement.Reports.{reportName}";
//            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
//            return View(report);
//        }
//        catch (Exception ex)
//        {
//            throw ex.InnerException;
//        }
//    }
//}

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

    public async Task<IActionResult> RebCostReport(
        int menuId,
        string reportName = "rptRebCost")
    {
        // Get logged-in user
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Get user permissions
        var userPermissions = await (
            from up in _context.TblUserPermission
            join pa in _context.TblPermissionAction
                on up.ActionId equals pa.ActionId
            where up.UserId == userId
                  && up.MenuId == menuId
                  && up.IsAllowed
            select pa.ActionName
        ).ToListAsync();

        ViewBag.CanView = userPermissions.Contains("View");

        // Check View permission
        if (!ViewBag.CanView)
        {
            return Forbid(); // or Unauthorized();
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