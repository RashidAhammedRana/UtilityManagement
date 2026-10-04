using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UtilityManagement.Data;
using UtilityManagement.Models;
namespace UtilityManagement.Controllers;
public class MonthlyUtilityBillInfoController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MonthlyUtilityBillInfoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> MonthlyUtilityBillInfoList(
       int page = 1,
       string searchString = "")
    {
        int pageSize = 30;

        var userId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier
        )?.Value;


        // =========================
        // PERMISSION
        // =========================

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Monthly Utility Bill")
            .Select(x => x.MenuId)
            .FirstOrDefaultAsync();


        var userPermissions = await (
            from up in _context.TblUserPermission
            join pa in _context.TblPermissionAction
                on up.ActionId equals pa.ActionId

            where up.UserId == userId
                  && up.MenuId == menuId
                  && up.IsAllowed

            select pa.ActionName

        ).ToListAsync();


        // =========================
        // CURRENT USER COMPANY
        // =========================

        var currentUserCompany = await _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefaultAsync();


        ViewBag.CanView = userPermissions.Contains("View");
        ViewBag.CanCreate = userPermissions.Contains("Create");
        ViewBag.CanEdit = userPermissions.Contains("Edit");
        ViewBag.CanDelete = userPermissions.Contains("Delete");


        // =========================
        // BASE QUERY
        // =========================

        var query = _context.TblMonthlyUtilityBillInfo
            .Include(x => x.Com)
            .AsQueryable();


        // =========================
        // COMPANY WISE DATA
        // =========================

        if (!string.IsNullOrWhiteSpace(currentUserCompany))
        {
            currentUserCompany = currentUserCompany.Trim();

            query = query.Where(x =>
                x.Com != null &&
                x.Com.ComName == currentUserCompany
            );
        }


        // =========================
        // SEARCH LOGIC
        // =========================

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            var parts = searchString.Split(
                '-',
                StringSplitOptions.RemoveEmptyEntries
            );


            // =========================
            // MONTH NAME SEARCH
            // January
            // Jan
            // February
            // Feb
            // =========================

            var months = new Dictionary<string, int>(
                StringComparer.OrdinalIgnoreCase)
        {
            { "january", 1 },
            { "jan", 1 },

            { "february", 2 },
            { "feb", 2 },

            { "march", 3 },
            { "mar", 3 },

            { "april", 4 },
            { "apr", 4 },

            { "may", 5 },

            { "june", 6 },
            { "jun", 6 },

            { "july", 7 },
            { "jul", 7 },

            { "august", 8 },
            { "aug", 8 },

            { "september", 9 },
            { "sep", 9 },
            { "sept", 9 },

            { "october", 10 },
            { "oct", 10 },

            { "november", 11 },
            { "nov", 11 },

            { "december", 12 },
            { "dec", 12 }
        };


            // =========================
            // CASE 1: FULL DATE
            // 22-06-2026
            // 2026-06-22
            // 22/06/2026
            // =========================

            bool isFullDate =
                DateOnly.TryParseExact(
                    searchString,
                    new[]
                    {
                    "dd-MM-yyyy",
                    "d-M-yyyy",
                    "dd/MM/yyyy",
                    "d/M/yyyy",
                    "yyyy-MM-dd",
                    "yyyy/MM/dd"
                    },
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateOnly parsedDate
                );


            if (isFullDate)
            {
                query = query.Where(x =>
                    x.Trdate == parsedDate
                );
            }


            // =========================
            // CASE 2: YEAR-MONTH
            // 2026-06
            // 2026/06
            // =========================

            else if (parts.Length == 2 && parts[0].Length == 4)
            {
                if (int.TryParse(parts[0], out int year) &&
                    int.TryParse(parts[1], out int month))
                {
                    query = query.Where(x =>
                        x.Trdate.HasValue &&
                        x.Trdate.Value.Year == year &&
                        x.Trdate.Value.Month == month
                    );
                }
            }


            // =========================
            // CASE 3: MONTH-DAY
            // 06-22
            // 22-06
            // =========================

            else if (parts.Length == 2)
            {
                if (int.TryParse(parts[0], out int a) &&
                    int.TryParse(parts[1], out int b))
                {
                    query = query.Where(x =>
                        x.Trdate.HasValue &&
                        (
                            (
                                x.Trdate.Value.Month == a &&
                                x.Trdate.Value.Day == b
                            )
                            ||
                            (
                                x.Trdate.Value.Month == b &&
                                x.Trdate.Value.Day == a
                            )
                        )
                    );
                }
            }


            // =========================
            // CASE 4: MONTH NAME
            // January / Jan
            // June / Jun
            // =========================

            else if (months.TryGetValue(searchString, out int monthNumber))
            {
                query = query.Where(x =>
                    x.Trdate.HasValue &&
                    x.Trdate.Value.Month == monthNumber
                );
            }


            // =========================
            // CASE 5: SINGLE NUMBER
            // Day / Month / Year
            // =========================

            else if (int.TryParse(searchString, out int number))
            {
                query = query.Where(x =>
                    x.Trdate.HasValue &&
                    (
                        x.Trdate.Value.Day == number ||
                        x.Trdate.Value.Month == number ||
                        x.Trdate.Value.Year == number
                    )
                );
            }


            // =========================
            // CASE 6: MONTH FIELD SEARCH
            // Exact/partial month string
            // =========================

            else
            {
                query = query.Where(x =>
                    x.Month != null &&
                    x.Month.Contains(searchString)
                );
            }
        }


        // =========================
        // PAGINATION
        // =========================

        var totalRecords = await query.CountAsync();


        var monthlyUtilityBillInfo = await query
            .OrderByDescending(x => x.Trdate)
            .ThenByDescending(x => x.Trid)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


        // =========================
        // VIEWBAG
        // =========================

        ViewBag.CurrentPage = page;

        ViewBag.TotalPages =
            (int)Math.Ceiling(
                totalRecords / (double)pageSize
            );

        ViewBag.totalReading = totalRecords;

        ViewBag.SearchString = searchString;


        return View(monthlyUtilityBillInfo);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var userId = _userManager.GetUserId(User);

        var companyName = _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefault();

        var company = _context.TblCompanyInfo
            .FirstOrDefault(x => x.ComName == companyName);

        if (company == null)
        {
            TempData["ErrorMessage"] = "Company information not found.";
            return RedirectToAction(nameof(MonthlyUtilityBillInfoList));
        }

        ViewBag.CompanyName = company.ComName;

        var model = new TblMonthlyUtilityBillInfo
        {
            Trdate = DateOnly.FromDateTime(DateTime.Today),
            Comid = company.Comid
        };

        return View(model);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        TblMonthlyUtilityBillInfo monthlyUtilityBillInfo)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] =
                    "Please fill all required fields.";

                return View(monthlyUtilityBillInfo);
            }

            // ==========================================
            // CALCULATE TOTAL AMOUNT
            // ==========================================

            monthlyUtilityBillInfo.TotalAmount =
                (monthlyUtilityBillInfo.RebBill ?? 0)
                + (monthlyUtilityBillInfo.TitasCaptiveBill ?? 0)
                + (monthlyUtilityBillInfo.TitasIndustrialBill ?? 0)
                + (monthlyUtilityBillInfo.DieselTotalBill ?? 0)
                + (monthlyUtilityBillInfo.CngTotalBill ?? 0)
                + (monthlyUtilityBillInfo.LpgTotalBill ?? 0)
                + (monthlyUtilityBillInfo.BiomassTotalBill ?? 0);

            // ==========================================
            // CREATED INFORMATION
            // ==========================================

            monthlyUtilityBillInfo.CreatedAt = DateTime.Now;
            monthlyUtilityBillInfo.CreatedBy =
                User.Identity?.Name ?? "System";

            // ==========================================
            // SAVE
            // ==========================================

            _context.TblMonthlyUtilityBillInfo
                .Add(monthlyUtilityBillInfo);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Monthly utility bill created successfully.";

            return RedirectToAction(
                nameof(MonthlyUtilityBillInfoList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] =
                "Failed to create monthly utility bill.";

            return View(monthlyUtilityBillInfo);
        }
    }


    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var reading = await _context.TblMonthlyUtilityBillInfo
            .Include(x => x.Com)
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (reading == null)
            return NotFound();

        return View(reading);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblMonthlyUtilityBillInfo monthlyUtilityBillInfo)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(monthlyUtilityBillInfo);
            }

            var existing = await _context.TblMonthlyUtilityBillInfo
                .FirstOrDefaultAsync(x => x.Trid == monthlyUtilityBillInfo.Trid);

            if (existing == null)
            {
                return NotFound();
            }

            // Update editable fields
            existing.Trdate = monthlyUtilityBillInfo.Trdate;
            existing.Year = monthlyUtilityBillInfo.Year;
            existing.Month = monthlyUtilityBillInfo.Month;
            existing.RebTotalKwh = monthlyUtilityBillInfo.RebTotalKwh;
            existing.RebBill = monthlyUtilityBillInfo.RebBill;
            existing.TitasCaptiveUse = monthlyUtilityBillInfo.TitasCaptiveUse;
            existing.TitasCaptiveBill = monthlyUtilityBillInfo.TitasCaptiveBill;
            existing.TitasIndustrialUse = monthlyUtilityBillInfo.TitasIndustrialUse;
            existing.TitasIndustrialBill = monthlyUtilityBillInfo.TitasIndustrialBill;
            existing.DieselTotalIssue = monthlyUtilityBillInfo.DieselTotalIssue;
            existing.DieselTotalBill = monthlyUtilityBillInfo.DieselTotalBill;
            existing.CngTotalIssue = monthlyUtilityBillInfo.CngTotalIssue;
            existing.CngTotalBill = monthlyUtilityBillInfo.CngTotalBill;
            existing.LpgTotalIssue = monthlyUtilityBillInfo.LpgTotalIssue;
            existing.LpgTotalBill = monthlyUtilityBillInfo.LpgTotalBill;
            existing.BiomassTotalIssue = monthlyUtilityBillInfo.BiomassTotalIssue;
            existing.BiomassTotalBill = monthlyUtilityBillInfo.BiomassTotalBill;
            existing.TotalAmount = monthlyUtilityBillInfo.TotalAmount;


            // Update audit fields
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reading updated successfully.";

            return RedirectToAction(nameof(MonthlyUtilityBillInfoList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to update reading.";

            return View(monthlyUtilityBillInfo);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblMonthlyUtilityBillInfo.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblMonthlyUtilityBillInfo
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Readings not found.";
            return RedirectToAction(nameof(MonthlyUtilityBillInfoList));
        }

        _context.TblMonthlyUtilityBillInfo.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Readings deleted successfully.";

        return RedirectToAction(nameof(MonthlyUtilityBillInfoList));
    }
}



