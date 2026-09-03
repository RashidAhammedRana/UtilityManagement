using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UtilityManagement.Data;
using UtilityManagement.Models;
using UtilityManagement.ViewModels;

public class BoilerSteamGenerationInfoController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    private const int PageSize = 15;
    private const int StartHour = 6;

    public BoilerSteamGenerationInfoController(ApplicationDbContext context,UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> BoilerSteamGenerationInfoList(
        int page = 1,
        string searchString = "")
    {
        if (page < 1)
        {
            page = 1;
        }

        // -----------------------------------------------------
        // Current User
        // -----------------------------------------------------

        var userId = _userManager.GetUserId(User);

        // -----------------------------------------------------
        // Permissions
        // -----------------------------------------------------

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Boiler Steam Gen.")
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

        // -----------------------------------------------------
        // Current User Company
        // -----------------------------------------------------

        var currentUserCompany = await _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefaultAsync();

        if (!string.IsNullOrWhiteSpace(currentUserCompany))
        {
            currentUserCompany = currentUserCompany.Trim();
        }

        // -----------------------------------------------------
        // BASE QUERY
        // -----------------------------------------------------

        var query = _context.TblBoilerSteamGenerationInfo
            .AsNoTracking()
            .AsQueryable();

        // -----------------------------------------------------
        // COMPANY FILTER
        //
        // User with a company:
        //     Only see own company data.
        //
        // User without a company:
        //     Can see all data.
        // -----------------------------------------------------

        if (!string.IsNullOrWhiteSpace(currentUserCompany))
        {
            query = query.Where(x =>
                x.Company != null &&
                x.Company == currentUserCompany);
        }

        // =====================================================
        // SEARCH
        // =====================================================

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            // -------------------------------------------------
            // FULL DATE SEARCH
            //
            // Examples:
            //
            // 09-07-2026
            // 9-7-2026
            // 09/07/2026
            // 9/7/2026
            // 2026-07-09
            // 2026/07/09
            // -------------------------------------------------

            bool isFullDate = DateTime.TryParseExact(
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
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsedDate
            );

            if (isFullDate)
            {
                var searchDate = DateOnly.FromDateTime(parsedDate);

                query = query.Where(x =>
                    x.Trdate.HasValue &&
                    x.Trdate.Value == searchDate);
            }
            else
            {
                // -------------------------------------------------
                // Normalize separators
                //
                // Convert:
                // 2026/07 -> 2026-07
                // 09/07   -> 09-07
                // -------------------------------------------------

                var normalizedSearch = searchString.Replace('/', '-');

                var parts = normalizedSearch
                    .Split(
                        '-',
                        StringSplitOptions.RemoveEmptyEntries |
                        StringSplitOptions.TrimEntries
                    );

                // -------------------------------------------------
                // YEAR-MONTH
                //
                // 2026-07
                // 2026/07
                // -------------------------------------------------

                if (
                    parts.Length == 2 &&
                    parts[0].Length == 4 &&
                    int.TryParse(parts[0], out int year) &&
                    int.TryParse(parts[1], out int month)
                )
                {
                    if (month >= 1 && month <= 12)
                    {
                        query = query.Where(x =>
                            x.Trdate.HasValue &&
                            x.Trdate.Value.Year == year &&
                            x.Trdate.Value.Month == month);
                    }
                }

                // -------------------------------------------------
                // MONTH-DAY / DAY-MONTH
                //
                // 07-09
                // 09-07
                //
                // Both can find:
                // 09 July
                // -------------------------------------------------

                else if (
                    parts.Length == 2 &&
                    int.TryParse(parts[0], out int firstNumber) &&
                    int.TryParse(parts[1], out int secondNumber)
                )
                {
                    query = query.Where(x =>
                        x.Trdate.HasValue &&
                        (
                            (
                                x.Trdate.Value.Month == firstNumber &&
                                x.Trdate.Value.Day == secondNumber
                            )
                            ||
                            (
                                x.Trdate.Value.Month == secondNumber &&
                                x.Trdate.Value.Day == firstNumber
                            )
                        )
                    );
                }

                // -------------------------------------------------
                // SINGLE NUMBER
                //
                // 9
                // 7
                // 2026
                // -------------------------------------------------

                else if (
                    parts.Length == 1 &&
                    int.TryParse(parts[0], out int number)
                )
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
        }

        // =====================================================
        // PAGINATION
        // =====================================================

        var totalRecords = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(
            totalRecords / (double)PageSize
        );

        // Prevent invalid page
        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        var readings = await query
            .OrderByDescending(x => x.Trdate)
            .ThenByDescending(x => x.Time)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        // =====================================================
        // VIEWBAG
        // =====================================================

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.totalReadings = totalRecords;
        ViewBag.SearchString = searchString;

        return View(readings);
    }

    // =========================================================
    // CREATE - GET
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new BoilerSteamGenerationViewModel
        {
            Items = new List<TblBoilerSteamGenerationInfo>()
        };

        await LoadCreateTimeDataAsync(model);

        // =====================================================
        // IMPORTANT:
        // If database has no record for today,
        // first slot MUST be 06:00.
        // =====================================================

        var existingTimes =
            ViewBag.ExistingTimes as List<string>
            ?? new List<string>();

        var firstAvailable =
            GetFirstAvailableTimeFromList(existingTimes);

        ViewBag.NextAvailableTime =
            firstAvailable ?? "06:00";

        return View(model);
    }

    // =========================================================
    // CREATE - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BoilerSteamGenerationViewModel model)
    {
        try
        {
            // -------------------------------------------------
            // Validate Model
            // -------------------------------------------------

            if (!ModelState.IsValid)
            {
                await LoadCreateTimeDataAsync(model);

                TempData["ErrorMessage"] =
                    "Please fill all required fields.";

                return View(model);
            }

            // -------------------------------------------------
            // Check Items
            // -------------------------------------------------

            if (model.Items == null || model.Items.Count == 0)
            {
                await LoadCreateTimeDataAsync(model);

                TempData["ErrorMessage"] =
                    "Please add at least one reading.";

                return View(model);
            }

            // -------------------------------------------------
            // Current User
            // -------------------------------------------------

            var userId = _userManager.GetUserId(User);

            var currentCompany = await _context.Users
                .Where(x => x.Id == userId)
                .Select(x => x.Company)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrWhiteSpace(currentCompany))
            {
                currentCompany = currentCompany.Trim();
            }

            var currentUser =
                User.Identity?.Name ?? "System";

            var now = DateTime.Now;

            // -------------------------------------------------
            // Process Each Reading
            // -------------------------------------------------

            foreach (var boilerSteamGenerationInfo in model.Items)
            {
                // -------------------------------------------------
                // COMPANY SECURITY
                //
                // If current user has a company,
                // always force the record to that company.
                // -------------------------------------------------

                if (!string.IsNullOrWhiteSpace(currentCompany))
                {
                    boilerSteamGenerationInfo.Company =
                        currentCompany;
                }

                // -------------------------------------------------
                // Calculate Total
                // -------------------------------------------------

                //energyFuelConsumption.Total =
                //    energyFuelConsumption.Reb +
                //    energyFuelConsumption.Gg1 +
                //    energyFuelConsumption.Gg2 +
                //    energyFuelConsumption.Gg3 +
                //    energyFuelConsumption.Gg4 +
                //    energyFuelConsumption.Dg1 +
                //    energyFuelConsumption.Dg2 +
                //    energyFuelConsumption.Dg3 +
                //    energyFuelConsumption.Dg4 +
                //    energyFuelConsumption.Solar;

                // -------------------------------------------------
                // Created Information
                // -------------------------------------------------

                boilerSteamGenerationInfo.CreatedAt = now;
                boilerSteamGenerationInfo.CreatedBy = currentUser;

                _context.TblBoilerSteamGenerationInfo.Add(boilerSteamGenerationInfo);
            }

            // -------------------------------------------------
            // Save
            // -------------------------------------------------

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Reading(s) created successfully.";

            return RedirectToAction(
                nameof(BoilerSteamGenerationInfoList)
            );
        }
        catch (Exception)
        {
            await LoadCreateTimeDataAsync(model);

            TempData["ErrorMessage"] =
                "Failed to create reading.";

            return View(model);
        }
    }

    // =========================================================
    // LOAD CURRENT COMPANY
    // =========================================================

    private async Task<string?> GetCurrentUserCompanyAsync()
    {
        var userId = _userManager.GetUserId(User);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        var company = await _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefaultAsync();

        return string.IsNullOrWhiteSpace(company)
            ? null
            : company.Trim();
    }

    // =========================================================
    // LOAD COMPANY LIST / VIEWBAG
    // =========================================================

    private async Task LoadCompanyListAsync()
    {
        var currentCompany =
            await GetCurrentUserCompanyAsync();

        ViewBag.CurrentCompany =
            currentCompany;
    }

    // =========================================================
    // EDIT - GET
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var boilerSteamGenerationInfo = await _context.TblBoilerSteamGenerationInfo.FirstOrDefaultAsync(x => x.Trid == id);

        if (boilerSteamGenerationInfo == null)
        {
            return NotFound();
        }

        // -----------------------------------------------------
        // Current User Company
        // -----------------------------------------------------

        var currentCompany =
            await GetCurrentUserCompanyAsync();

        // -----------------------------------------------------
        // COMPANY SECURITY
        //
        // User with company:
        // only own company records.
        //
        // User without company:
        // can edit any company.
        // -----------------------------------------------------

        if (!string.IsNullOrWhiteSpace(currentCompany))
        {
            if (
                !string.Equals(
                    boilerSteamGenerationInfo.Company,
                    currentCompany,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return Forbid();
            }
        }

        ViewBag.CurrentCompany =
            currentCompany;

        return View(boilerSteamGenerationInfo);
    }

    // =========================================================
    // EDIT - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,TblBoilerSteamGenerationInfo boilerSteamGenerationInfo)
    {
        // -----------------------------------------------------
        // Current User Company
        // -----------------------------------------------------

        var currentCompany =
            await GetCurrentUserCompanyAsync();

        try
        {
            // =================================================
            // GET EXISTING RECORD
            // =================================================

            var existingRecord =
                await _context.TblBoilerSteamGenerationInfo
                    .FirstOrDefaultAsync(x => x.Trid == id);

            if (existingRecord == null)
            {
                return NotFound();
            }

            // =================================================
            // COMPANY SECURITY
            // =================================================

            if (!string.IsNullOrWhiteSpace(currentCompany))
            {
                // User can edit only own company data.
                if (
                    !string.Equals(
                        existingRecord.Company,
                        currentCompany,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    return Forbid();
                }

                // Never allow company to be changed.
                boilerSteamGenerationInfo.Company =
                    existingRecord.Company;
            }
            else
            {
                // User without company:
                // keep the original company.
                boilerSteamGenerationInfo.Company =
                    existingRecord.Company;
            }

            // =================================================
            // MODEL VALIDATION
            // =================================================

            if (!ModelState.IsValid)
            {
                ViewBag.CurrentCompany =
                    currentCompany;

                return View(boilerSteamGenerationInfo);
            }

            // =================================================
            // UPDATE FIELDS
            // =================================================

            existingRecord.Company = boilerSteamGenerationInfo.Company;
            existingRecord.Trdate = boilerSteamGenerationInfo.Trdate;
            existingRecord.Time = boilerSteamGenerationInfo.Time;
            existingRecord.GasPressure = boilerSteamGenerationInfo.GasPressure;
            existingRecord.HeaderSteamPressure = boilerSteamGenerationInfo.HeaderSteamPressure;
            existingRecord.Boiler1SteamGeneration = boilerSteamGenerationInfo.Boiler1SteamGeneration;
            existingRecord.Boiler2SteamGeneration = boilerSteamGenerationInfo.Boiler2SteamGeneration;
            existingRecord.Boiler3SteamGeneration = boilerSteamGenerationInfo.Boiler3SteamGeneration;
            existingRecord.EgbBoilerSteamGeneration = boilerSteamGenerationInfo.EgbBoilerSteamGeneration;
            existingRecord.B1UsageFuel = boilerSteamGenerationInfo.B1UsageFuel;
            existingRecord.B2UsageFuel = boilerSteamGenerationInfo.B2UsageFuel;
            existingRecord.B3UsageFuel = boilerSteamGenerationInfo.B3UsageFuel;
            existingRecord.Remarks = boilerSteamGenerationInfo.Remarks;
            existingRecord.TotalGeneration = boilerSteamGenerationInfo.TotalGeneration;

            // =================================================
            // RECALCULATE TOTAL
            //
            // Do not trust the Total value submitted by browser.
            // =================================================

            //existingRecord.Total =
            //    existingRecord.Reb +
            //    existingRecord.Gg1 +
            //    existingRecord.Gg2 +
            //    existingRecord.Gg3 +
            //    existingRecord.Gg4 +
            //    existingRecord.Dg1 +
            //    existingRecord.Dg2 +
            //    existingRecord.Dg3 +
            //    existingRecord.Dg4 +
                //existingRecord.Solar;

            // =================================================
            // UPDATED INFORMATION
            // =================================================

            existingRecord.UpdatedAt =
                DateTime.Now;

            existingRecord.UpdatedBy =
                User.Identity?.Name ?? "System";

            // =================================================
            // SAVE
            // =================================================

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Reading updated successfully.";

            return RedirectToAction(
                nameof(BoilerSteamGenerationInfoList)
            );
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists =
                await _context.TblBoilerSteamGenerationInfo
                    .AnyAsync(x => x.Trid == id);

            if (!exists)
            {
                return NotFound();
            }

            TempData["ErrorMessage"] =
                "The record was modified by another user. Please try again.";

            ViewBag.CurrentCompany =
                currentCompany;

            return View(boilerSteamGenerationInfo);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] =
                "Failed to update reading.";

            ViewBag.CurrentCompany =
                currentCompany;

            return View(boilerSteamGenerationInfo);
        }
    }

    // =========================================================
    // DELETE - GET
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data =
            await _context.TblBoilerSteamGenerationInfo
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Trid == id);

        if (data == null)
        {
            return NotFound();
        }

        // -----------------------------------------------------
        // COMPANY SECURITY
        // -----------------------------------------------------

        var currentCompany =
            await GetCurrentUserCompanyAsync();

        if (!string.IsNullOrWhiteSpace(currentCompany))
        {
            if (
                !string.Equals(
                    data.Company,
                    currentCompany,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return Forbid();
            }
        }

        return View(data);
    }

    // =========================================================
    // DELETE - POST
    // =========================================================

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data =
            await _context.TblBoilerSteamGenerationInfo
                .FirstOrDefaultAsync(x => x.Trid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] =
                "Reading not found.";

            return RedirectToAction(
                nameof(BoilerSteamGenerationInfoList)
            );
        }

        // -----------------------------------------------------
        // COMPANY SECURITY
        // -----------------------------------------------------

        var currentCompany =
            await GetCurrentUserCompanyAsync();

        if (!string.IsNullOrWhiteSpace(currentCompany))
        {
            if (
                !string.Equals(
                    data.Company,
                    currentCompany,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return Forbid();
            }
        }

        // -----------------------------------------------------
        // DELETE
        // -----------------------------------------------------

        _context.TblBoilerSteamGenerationInfo
            .Remove(data);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Reading deleted successfully.";

        return RedirectToAction(
            nameof(BoilerSteamGenerationInfoList)
        );
    }

    // =========================================================
    // GET AVAILABLE TIMES FOR SELECTED DATE
    // =========================================================
    //
    // JavaScript can call:
    //
    // /DailyEnergyFuelConsumption/GetAvailableTimes?date=2026-08-19
    //
    // It checks:
    //
    // Company + Selected Date
    //
    // It does NOT automatically use today's date.
    //
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetAvailableTimes(
        DateOnly date)
    {
        var company =
            await GetCurrentUserCompanyAsync();

        // -----------------------------------------------------
        // Company validation
        // -----------------------------------------------------

        if (string.IsNullOrWhiteSpace(company))
        {
            return Json(new
            {
                success = false,
                message = "Company is not available.",
                existingTimes = new List<string>(),
                nextTime = ""
            });
        }

        // -----------------------------------------------------
        // Get existing times for selected date
        // -----------------------------------------------------

        var existingTimeValues =
            await _context
                .TblBoilerSteamGenerationInfo
                .AsNoTracking()
                .Where(x =>
                    x.Company == company &&
                    x.Trdate.HasValue &&
                    x.Trdate.Value == date &&
                    x.Time.HasValue)
                .Select(x => x.Time!.Value)
                .OrderBy(x => x)
                .ToListAsync();

        // -----------------------------------------------------
        // Convert to HH:mm AFTER EF query
        //
        // This avoids SQL translation problems with:
        // TimeOnly.ToString("HH:mm")
        // -----------------------------------------------------

        var existingTimes =
            existingTimeValues
                .Select(x => x.ToString("HH:mm"))
                .ToList();

        // -----------------------------------------------------
        // Get next available time
        // -----------------------------------------------------

        var nextTime =
            await GetFirstAvailableTimeAsync(
                company,
                date
            );

        // -----------------------------------------------------
        // Return JSON
        // -----------------------------------------------------

        return Json(new
        {
            success = true,

            existingTimes = existingTimes,

            nextTime = nextTime.HasValue
                ? nextTime.Value.ToString("HH:mm")
                : ""
        });
    }

    // =========================================================
    // GET FIRST AVAILABLE TIME
    // =========================================================
    //
    // Daily sequence:
    //
    // 06:00
    // 07:00
    // 08:00
    // ...
    // 23:00
    // 00:00
    // 01:00
    // 02:00
    // 03:00
    // 04:00
    // 05:00
    //
    // Total = 24 hourly slots.
    //
    // =========================================================

    private async Task<TimeOnly?> GetFirstAvailableTimeAsync(
        string company,
        DateOnly date)
    {
        // -----------------------------------------------------
        // Company validation
        // -----------------------------------------------------

        if (string.IsNullOrWhiteSpace(company))
        {
            return null;
        }

        company = company.Trim();

        // -----------------------------------------------------
        // Get existing times
        // -----------------------------------------------------

        var existingTimes =
            await _context
                .TblDailyEnergyFuelConsumption
                .AsNoTracking()
                .Where(x =>
                    x.Company == company &&
                    x.Trdate.HasValue &&
                    x.Trdate.Value == date &&
                    x.Time.HasValue)
                .Select(x => x.Time!.Value)
                .ToListAsync();

        // -----------------------------------------------------
        // HashSet for quick lookup
        // -----------------------------------------------------

        var existingSet =
            existingTimes.ToHashSet();

        // -----------------------------------------------------
        // Check all 24 hourly slots
        //
        // Starts at 06:00.
        //
        // 06 -> 23
        // 00 -> 05
        // -----------------------------------------------------

        for (int i = 0; i < 24; i++)
        {
            int hour =
                (StartHour + i) % 24;

            var time =
                new TimeOnly(hour, 0);

            if (!existingSet.Contains(time))
            {
                return time;
            }
        }

        // -----------------------------------------------------
        // All 24 slots are already used.
        // -----------------------------------------------------

        return null;
    }

    // =========================================================
    // LOAD CREATE TIME DATA
    // =========================================================
    //
    // Loads:
    //
    // ViewBag.ExistingTimes
    // ViewBag.NextAvailableTime
    //
    // Date selection:
    //
    // 1. If model has a date -> use model date.
    // 2. Otherwise -> use today's date.
    //
    // =========================================================

    private async Task LoadCreateTimeDataAsync(
        BoilerSteamGenerationViewModel model)
    {
        // -----------------------------------------------------
        // Load current company
        // -----------------------------------------------------

        await LoadCompanyListAsync();

        var company =
            ViewBag.CurrentCompany as string;

        // -----------------------------------------------------
        // No company
        // -----------------------------------------------------

        if (string.IsNullOrWhiteSpace(company))
        {
            ViewBag.ExistingTimes =
                new List<string>();

            ViewBag.NextAvailableTime =
                "";

            return;
        }

        company = company.Trim();

        // -----------------------------------------------------
        // Default date = Today
        // -----------------------------------------------------

        DateOnly selectedDate =
            DateOnly.FromDateTime(
                DateTime.Today
            );

        // -----------------------------------------------------
        // If model has date, use that date
        // -----------------------------------------------------

        if (
            model.Items != null &&
            model.Items.Count > 0
        )
        {
            var firstItem =
                model.Items.FirstOrDefault();

            if (firstItem != null &&
                firstItem.Trdate.HasValue)
            {
                selectedDate =
                    firstItem.Trdate.Value;
            }
        }

        // -----------------------------------------------------
        // Get existing times
        // -----------------------------------------------------

        var existingTimeValues =
            await _context
                .TblDailyEnergyFuelConsumption
                .AsNoTracking()
                .Where(x =>
                    x.Company == company &&
                    x.Trdate.HasValue &&
                    x.Trdate.Value == selectedDate &&
                    x.Time.HasValue)
                .Select(x => x.Time!.Value)
                .OrderBy(x => x)
                .ToListAsync();

        // -----------------------------------------------------
        // Convert TimeOnly to HH:mm after DB query
        // -----------------------------------------------------

        var existingTimes =
            existingTimeValues
                .Select(x => x.ToString("HH:mm"))
                .ToList();

        // -----------------------------------------------------
        // Get next available time
        // -----------------------------------------------------

        var nextTime =
            await GetFirstAvailableTimeAsync(
                company,
                selectedDate
            );

        // -----------------------------------------------------
        // ViewBag
        // -----------------------------------------------------

        ViewBag.ExistingTimes =
            existingTimes;

        ViewBag.NextAvailableTime =
            nextTime.HasValue
                ? nextTime.Value.ToString("HH:mm")
                : "";
    }
    // =========================================================
    // GET FIRST AVAILABLE TIME FROM STRING LIST
    // =========================================================

    [HttpGet]
    private string? GetFirstAvailableTimeFromList(
        List<string> existingTimes)
    {
        var existingSet =
            existingTimes
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim().Substring(0, 5))
                .ToHashSet();

        // =====================================================
        // 06:00 -> 23:00
        // 00:00 -> 05:00
        // =====================================================

        for (int i = 0; i < 24; i++)
        {
            int hour =
                (StartHour + i) % 24;

            string time =
                $"{hour:00}:00";

            if (!existingSet.Contains(time))
            {
                return time;
            }
        }

        return null;
    }
}
