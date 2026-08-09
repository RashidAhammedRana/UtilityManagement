using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UtilityManagement.Data;
using UtilityManagement.Models;
using UtilityManagement.ViewModels;

public class DailyEnergyFuelConsumptionController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;


    public DailyEnergyFuelConsumptionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> DailyEnergyFuelConsumptionList(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Energy & Fuel Cons.")
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
        var query = _context.TblDailyEnergyFuelConsumption
            .AsQueryable();

        //Company Wise Data
        if (!string.IsNullOrWhiteSpace(currentUserCompany))
        {
            currentUserCompany = currentUserCompany.Trim();
            query = query.Where(x => x.Company != null && x.Company == currentUserCompany);
        }

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

        var model = new DailyEnergyFuelConsumptionCreateViewModel
        {
            Items = new List<TblDailyEnergyFuelConsumption>
        {
            new TblDailyEnergyFuelConsumption
            {
                Trdate = DateOnly.FromDateTime(DateTime.Today)
            }
        }
        };

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        DailyEnergyFuelConsumptionCreateViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                LoadCompanyList();

                TempData["ErrorMessage"] =
                    "Please fill all required fields.";

                return View(model);
            }

            var now = DateTime.Now;
            var currentUser = User.Identity?.Name ?? "System";

            foreach (var energyFuelConsumption in model.Items)
            {
                // Calculate Total
                energyFuelConsumption.Total =
                    energyFuelConsumption.Reb +
                    energyFuelConsumption.Gg1 +
                    energyFuelConsumption.Gg2 +
                    energyFuelConsumption.Gg3 +
                    energyFuelConsumption.Gg4 +
                    energyFuelConsumption.Dg1 +
                    energyFuelConsumption.Dg2 +
                    energyFuelConsumption.Dg3 +
                    energyFuelConsumption.Dg4 +
                    energyFuelConsumption.Solar;

                // Created Information
                energyFuelConsumption.CreatedAt = now;
                energyFuelConsumption.CreatedBy = currentUser;

                _context.TblDailyEnergyFuelConsumption
                    .Add(energyFuelConsumption);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Reading(s) created successfully.";

            return RedirectToAction(
                nameof(DailyEnergyFuelConsumptionList));
        }
        catch (Exception)
        {
            LoadCompanyList();

            TempData["ErrorMessage"] =
                "Failed to create reading.";

            return View(model);
        }
    }


    private void LoadCompanyList()
    {
        var userId = _userManager.GetUserId(User);

        var currentCompany = _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefault();

        ViewBag.CurrentCompany = currentCompany;
    }

[HttpGet]
public async Task<IActionResult> Edit(int id)
    {
        var energyFuelConsumption =
            await _context.TblDailyEnergyFuelConsumption
                .FirstOrDefaultAsync(x => x.Trid == id);

        if (energyFuelConsumption == null)
        {
            return NotFound();
        }

        // Current user company
        var userId =
            _userManager.GetUserId(User);

        var currentCompany =
            await _context.Users
                .Where(x => x.Id == userId)
                .Select(x => x.Company)
                .FirstOrDefaultAsync();

        // Security:
        // User with company can edit only own company data.
        // User without company can edit all data.
        if (!string.IsNullOrWhiteSpace(currentCompany))
        {
            if (energyFuelConsumption.Company != currentCompany)
            {
                return Forbid();
            }
        }

        ViewBag.CurrentCompany =
            currentCompany;

        return View(energyFuelConsumption);
    }


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(
    int id,
    TblDailyEnergyFuelConsumption energyFuelConsumption)
    {
        // Declare outside try so it can be used in catch
        var userId = _userManager.GetUserId(User);

        var currentCompany =
            await _context.Users
                .Where(x => x.Id == userId)
                .Select(x => x.Company)
                .FirstOrDefaultAsync();

        try
        {
            // =============================================
            // GET EXISTING RECORD
            // =============================================

            var existingRecord =
                await _context.TblDailyEnergyFuelConsumption
                    .FirstOrDefaultAsync(x => x.Trid == id);

            if (existingRecord == null)
            {
                return NotFound();
            }


            // =============================================
            // COMPANY SECURITY
            // =============================================

            // If user has a company,
            // user can edit only that company's data.
            if (!string.IsNullOrWhiteSpace(currentCompany))
            {
                if (existingRecord.Company != currentCompany)
                {
                    return Forbid();
                }

                // Don't allow company change
                energyFuelConsumption.Company =
                    existingRecord.Company;
            }
            else
            {
                // User without company
                // keeps existing company
                energyFuelConsumption.Company =
                    existingRecord.Company;
            }


            // =============================================
            // MODEL VALIDATION
            // =============================================

            if (!ModelState.IsValid)
            {
                ViewBag.CurrentCompany = currentCompany;

                return View(energyFuelConsumption);
            }


            // =============================================
            // UPDATE FIELDS
            // =============================================

            existingRecord.Trdate =
                energyFuelConsumption.Trdate;

            existingRecord.Time =
                energyFuelConsumption.Time;

            existingRecord.Reb =
                energyFuelConsumption.Reb;

            existingRecord.Gg1 =
                energyFuelConsumption.Gg1;

            existingRecord.Gg2 =
                energyFuelConsumption.Gg2;

            existingRecord.Gg3 =
                energyFuelConsumption.Gg3;

            existingRecord.Gg4 =
                energyFuelConsumption.Gg4;

            existingRecord.Dg1 =
                energyFuelConsumption.Dg1;

            existingRecord.Dg2 =
                energyFuelConsumption.Dg2;

            existingRecord.Dg3 =
                energyFuelConsumption.Dg3;

            existingRecord.Dg4 =
                energyFuelConsumption.Dg4;

            existingRecord.Solar =
                energyFuelConsumption.Solar;

            existingRecord.Total =
                energyFuelConsumption.Total;

            existingRecord.CaptiveGenerator =
                energyFuelConsumption.CaptiveGenerator;

            existingRecord.IndustrialBoiler =
                energyFuelConsumption.IndustrialBoiler;


            // =============================================
            // UPDATED INFORMATION
            // =============================================

            existingRecord.UpdatedAt =
                DateTime.Now;

            existingRecord.UpdatedBy =
                User.Identity?.Name ?? "System";


            // =============================================
            // SAVE
            // =============================================

            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                "Reading updated successfully.";

            return RedirectToAction(
                nameof(DailyEnergyFuelConsumptionList)
            );
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists =
                await _context.TblDailyEnergyFuelConsumption
                    .AnyAsync(x => x.Trid == id);

            if (!exists)
            {
                return NotFound();
            }

            TempData["ErrorMessage"] =
                "The record was modified by another user. Please try again.";

            ViewBag.CurrentCompany = currentCompany;

            return View(energyFuelConsumption);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] =
                "Failed to update reading.";

            ViewBag.CurrentCompany = currentCompany;

            return View(energyFuelConsumption);
        }
    }


    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblDailyEnergyFuelConsumption.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblDailyEnergyFuelConsumption
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Readings not found.";
            return RedirectToAction(nameof(DailyEnergyFuelConsumptionList));
        }

        _context.TblDailyEnergyFuelConsumption.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Readings deleted successfully.";

        return RedirectToAction(nameof(DailyEnergyFuelConsumptionList));
    }
}

