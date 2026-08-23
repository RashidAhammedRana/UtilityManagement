using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UtilityManagement.Data;
using UtilityManagement.Models;
using UtilityManagement.ViewModels;

public class DailyStockConsumptionController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;


    public DailyStockConsumptionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> DailyStockConsumptionList(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Fuel Stock & Cons.")
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

        ViewBag.CanView = userPermissions.Contains("View");
        ViewBag.CanCreate = userPermissions.Contains("Create");
        ViewBag.CanEdit = userPermissions.Contains("Edit");
        ViewBag.CanDelete = userPermissions.Contains("Delete");

        var currentUserCompany = await _context.Users.Where(x => x.Id == userId).Select(x => x.Company).FirstOrDefaultAsync();
        // =========================
        // BASE QUERY
        // =========================
        var query = _context.TblDailyStockConsumptions
            .AsQueryable();

        // =========================================================
        // SEARCH LOGIC
        // =========================================================

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            var parts = searchString.Split(
                '-',
                StringSplitOptions.RemoveEmptyEntries
            );

            bool isNumber =
                int.TryParse(searchString, out int number);


            // =====================================================
            // CASE 1: FULL DATE
            //
            // 09-07-2026  -> 09 July 2026
            // 9-7-2026    -> 09 July 2026
            // 09/07/2026  -> 09 July 2026
            //
            // Also supports:
            // 2026-07-09
            // 2026/07/09
            // =====================================================

            bool isFullDate =
                DateTime.TryParseExact(
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

                    out DateTime parsedDate
                );


            if (isFullDate)
            {
                // Convert DateTime to DateOnly
                var searchDate =
                    DateOnly.FromDateTime(parsedDate);


                // EXACT DATE MATCH
                query = query.Where(x =>
                    x.Trdate.HasValue &&
                    x.Trdate.Value == searchDate
                );
            }


            // =====================================================
            // CASE 2: YEAR-MONTH
            //
            // 2026-07
            // 2026/07
            // =====================================================

            else if (
                parts.Length == 2 &&
                parts[0].Length == 4
            )
            {
                if (
                    int.TryParse(parts[0], out int year) &&
                    int.TryParse(parts[1], out int month)
                )
                {
                    if (month >= 1 && month <= 12)
                    {
                        query = query.Where(x =>
                            x.Trdate.HasValue &&
                            x.Trdate.Value.Year == year &&
                            x.Trdate.Value.Month == month
                        );
                    }
                }
            }


            // =====================================================
            // CASE 3: MONTH-DAY / DAY-MONTH
            //
            // 07-09
            // 09-07
            //
            // Both will find:
            // 09 July
            // =====================================================

            else if (parts.Length == 2)
            {
                if (
                    int.TryParse(parts[0], out int a) &&
                    int.TryParse(parts[1], out int b)
                )
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


            // =====================================================
            // CASE 4: SINGLE NUMBER
            //
            // 9
            // 7
            // 2026
            // =====================================================

            else if (isNumber)
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
        }


        // =========================
        // PAGINATION
        // =========================
        var totalRecords = await query.CountAsync();

        var rebReadings = await query
            .OrderByDescending(x => x.Trdate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // =========================
        // VIEWBAG
        // =========================
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.totalReadings = totalRecords;
        ViewBag.SearchString = searchString;

        return View(rebReadings);
    }

    [HttpGet]
    public IActionResult Create()
    {
        LoadCompanyList();
            var model = new TblDailyStockConsumption
        {
            Trdate = DateOnly.FromDateTime(DateTime.Today)
        };

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblDailyStockConsumption dailyStockConsumption)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(dailyStockConsumption);
            }

            // Normalize date (only date part)
            var dateOnly = dailyStockConsumption.Trdate;

            // Save current time (keep datetime but same date)
            var now = DateTime.Now;
            var currentUser = User.Identity?.Name ?? "System";
            // Created Information
            dailyStockConsumption.CreatedtAt = now;
            dailyStockConsumption.CreatedBy = currentUser;

            _context.TblDailyStockConsumptions.Add(dailyStockConsumption);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reading created successfully.";

            return RedirectToAction(nameof(DailyStockConsumptionList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create reading.";
            return View(dailyStockConsumption);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        LoadCompanyList();
        var reading = await _context.TblDailyStockConsumptions
            .FirstOrDefaultAsync(x => x.Trid == id);
        if (reading == null)
            return NotFound();

        return View(reading);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblDailyStockConsumption dailyStockConsumption)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(dailyStockConsumption);
            }

            var existing = await _context.TblDailyStockConsumptions
                .FirstOrDefaultAsync(x => x.Trid == dailyStockConsumption.Trid);

            if (existing == null)
            {
                return NotFound();
            }

            // Update editable fields
            existing.Trdate = dailyStockConsumption.Trdate;
            existing.OsDiesel = dailyStockConsumption.OsDiesel;
            existing.ReceiveDiesel = dailyStockConsumption.ReceiveDiesel;
            existing.ConsDiesel = dailyStockConsumption.ConsDiesel;
            existing.CsDiesel = dailyStockConsumption.CsDiesel;
            existing.OsCng = dailyStockConsumption.OsCng;
            existing.ReceiveCng = dailyStockConsumption.ReceiveCng;
            existing.ConsCng = dailyStockConsumption.ConsCng;
            existing.CsCng = dailyStockConsumption.CsCng;
            existing.OsLps = dailyStockConsumption.OsLps;
            existing.ReceiveLpg = dailyStockConsumption.ReceiveLpg;
            existing.ConsLpg = dailyStockConsumption.ConsLpg;
            existing.CsLpg = dailyStockConsumption.CsLpg;

            // Update audit fields
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reading updated successfully.";

            return RedirectToAction(nameof(DailyStockConsumptionList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to update reading.";

            return View(dailyStockConsumption);
        }
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblDailyStockConsumptions.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblDailyStockConsumptions
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Readings not found.";
            return RedirectToAction(nameof(DailyStockConsumptionList));
        }

        _context.TblDailyStockConsumptions.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Readings deleted successfully.";

        return RedirectToAction(nameof(DailyStockConsumptionList));
    }
    [HttpGet]
    private void LoadCompanyList()
    {
        var userId = _userManager.GetUserId(User);

        var currentCompany = _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefault();

        ViewBag.CurrentCompany = currentCompany;
    }
}


