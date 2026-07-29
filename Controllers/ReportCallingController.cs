using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;

public class ReportCallingController : Controller
{
    //public IActionResult Index(string reportName = "XtraReport1")
    //{
    //    try
    //    {
    //        // Full namespace + report class name
    //        string rptPath = $"UtilityManagement.Reports.{reportName}";

    //        // Find report type
    //        Type reportType = Type.GetType(rptPath);

    //        if (reportType == null)
    //        {
    //            return Content($"Report not found: {rptPath}");
    //        }

    //        // Create report instance
    //        XtraReport report = Activator.CreateInstance(reportType) as XtraReport;

    //        if (report == null)
    //        {
    //            return Content("Report instance creation failed.");
    //        }

    //        // Test report loaded
    //        ViewBag.ReportName = reportName;

    //        return View(report);
    //    }
    //    catch (Exception ex)
    //    {
    //        return Content(
    //            "Error: " + ex.Message +
    //            "\nInner Exception: " + ex.InnerException?.Message
    //        );
    //    }
    //}

    public IActionResult RebCostReport(string reportName = "rptShiftWiseCounterMetrics")
    {
        try
        {
            var rptPath = $"UtilityManagement.Reports.{reportName}";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            return View(report);
        }
        catch (Exception ex)
        {
            throw ex.InnerException;
        }
    }
}