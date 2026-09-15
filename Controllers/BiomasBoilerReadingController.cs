using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Globalization;
using UtilityManagement.Data;
using UtilityManagement.Models;
using UtilityManagement.ViewModels;

public class BiomasBoilerReadingController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public BiomasBoilerReadingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> BiomasBoilerReadingList(
        int page = 1,
        string searchString = "")
    {
        const int pageSize = 15;

        if (page < 1)
            page = 1;

        var userId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;


        // =========================
        // MENU & PERMISSIONS
        // =========================

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Biomass Boiler")
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

        var currentUserCompany = await _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefaultAsync();


        // =========================
        // USER PERMISSIONS
        // =========================

        ViewBag.CanView = userPermissions.Contains("View");
        ViewBag.CanCreate = userPermissions.Contains("Create");
        ViewBag.CanEdit = userPermissions.Contains("Edit");
        ViewBag.CanDelete = userPermissions.Contains("Delete");


        // =========================
        // BASE QUERY
        // =========================

        var query = _context.TblBiomasBoilerReading
            .Include(x => x.Eq)
            .AsQueryable();


        // =========================
        // COMPANY WISE DATA
        // =========================

        if (!string.IsNullOrWhiteSpace(currentUserCompany))
        {
            currentUserCompany = currentUserCompany.Trim();

            query = query.Where(x =>
                x.Company == currentUserCompany);
        }


        // =========================
        // SEARCH
        // =========================

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            var parts = searchString.Split(
                '-',
                StringSplitOptions.RemoveEmptyEntries);

            bool isNumber = int.TryParse(
                searchString,
                out int number);


            // =========================
            // FULL DATE
            // Examples:
            // 22-06-2026
            // 22/06/2026
            // 2026-06-22
            // =========================

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
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedDate);


            if (isFullDate)
            {
                var searchDate =
                    DateOnly.FromDateTime(parsedDate);

                query = query.Where(x =>
                    x.Trdate == searchDate);
            }


            // =========================
            // YEAR - MONTH
            // Example:
            // 2026-06
            // =========================

            else if (parts.Length == 2 &&
                     parts[0].Length == 4)
            {
                if (int.TryParse(parts[0], out int year) &&
                    int.TryParse(parts[1], out int month) &&
                    month >= 1 &&
                    month <= 12)
                {
                    query = query.Where(x =>
                        x.Trdate.HasValue &&
                        x.Trdate.Value.Year == year &&
                        x.Trdate.Value.Month == month);
                }
            }


            // =========================
            // MONTH - DAY / DAY - MONTH
            // Examples:
            // 06-22
            // 22-06
            // =========================

            else if (parts.Length == 2)
            {
                if (int.TryParse(parts[0], out int first) &&
                    int.TryParse(parts[1], out int second))
                {
                    query = query.Where(x =>
                        x.Trdate.HasValue &&
                        (
                            (
                                x.Trdate.Value.Month == first &&
                                x.Trdate.Value.Day == second
                            )
                            ||
                            (
                                x.Trdate.Value.Month == second &&
                                x.Trdate.Value.Day == first
                            )
                        ));
                }
            }


            // =========================
            // SINGLE NUMBER
            // Day / Month / Year
            // =========================

            else if (isNumber)
            {
                query = query.Where(x =>
                    x.Trdate.HasValue &&
                    (
                        x.Trdate.Value.Day == number ||
                        x.Trdate.Value.Month == number ||
                        x.Trdate.Value.Year == number
                    ));
            }


            // =========================
            // TEXT SEARCH
            // Equipment / Company
            // =========================

            else
            {
                query = query.Where(x =>
                    (x.Eq != null &&
                     x.Eq.EquipmentName.Contains(searchString))
                    ||
                    (x.Company != null &&
                     x.Company.Contains(searchString))
                );
            }
        }


        // =========================
        // TOTAL RECORDS
        // =========================

        var totalRecords = await query.CountAsync();


        // =========================
        // TOTAL PAGES
        // =========================

        var totalPages = (int)Math.Ceiling(
            totalRecords / (double)pageSize);


        // =========================
        // PAGE VALIDATION
        // =========================

        if (totalPages > 0 && page > totalPages)
            page = totalPages;


        // =========================
        // PAGINATION
        // =========================

        var readings = await query
            .OrderByDescending(x => x.Trdate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


        // =========================
        // VIEW MODEL
        // =========================

        var biomasBoilerReadings = readings
            .Select(x => new BiomasBoilerReadingViewModel
            {
                BiomasBoilerReading = x
            })
            .ToList();


        // =========================
        // VIEWBAG
        // =========================

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.totalReadings = totalRecords;
        ViewBag.SearchString = searchString;


        // =========================
        // RETURN VIEW
        // =========================

        return View(biomasBoilerReadings);
    }


    private BiomassFuelRateViewModel GetBiomassFuelRate()
    {
        return new BiomassFuelRateViewModel
        {
            JuteRate = _context.TblFncItemRates //Biomas/Jute
                .Where(x => x.Fncid == 32)
                .OrderByDescending(x => x.Date)
                .Select(x => x.Rate)
                .FirstOrDefault(),

            RiceHuskRate = _context.TblFncItemRates //Rice Husk
                .Where(x => x.Fncid == 33)
                .OrderByDescending(x => x.Date)
                .Select(x => x.Rate)
                .FirstOrDefault(),

            WasteWoodRate = _context.TblFncItemRates //Waste Wood
                .Where(x => x.Fncid == 34)
                .OrderByDescending(x => x.Date)
                .Select(x => x.Rate)
                .FirstOrDefault(),

            CartonRate = _context.TblFncItemRates //Carton
                .Where(x => x.Fncid == 35)
                .OrderByDescending(x => x.Date)
                .Select(x => x.Rate)
                .FirstOrDefault(),

            CharcoilRate = _context.TblFncItemRates //Charcoil
                .Where(x => x.Fncid == 36)
                .OrderByDescending(x => x.Date)
                .Select(x => x.Rate)
                .FirstOrDefault()
        };
    }



    [HttpGet]
    public IActionResult Create()
    {
        var userId = _userManager.GetUserId(User);

        var currentLocation = _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefault();

        // ===============================
        // Company Wise Last Data Entry Date
        // ===============================

        var lastTrDate = _context.TblBiomasBoilerReading
            .Where(x => x.Company == currentLocation)
            .OrderByDescending(x => x.Trdate)
            .Select(x => x.Trdate)
            .FirstOrDefault();

        var biomasBoilerReading = new TblBiomasBoilerReading
        {
            Company = currentLocation,

            Trdate = lastTrDate.HasValue
                ? lastTrDate.Value.AddDays(1)
                : DateOnly.FromDateTime(DateTime.Today)
        };


        // ===============================
        // Equipment List
        // ===============================

        var query = _context.TblEquipmentDetails
            .Where(x =>
                EF.Functions.Like(
                    x.EquipmentName,
                    "%BIOMASS BOILER%"
                ));

        if (!string.IsNullOrEmpty(currentLocation))
        {
            query = query.Where(x =>
                x.CurrentLocation == currentLocation);
        }

        var equipmentList = query
            .Select(x => new SelectListItem
            {
                Value = x.Eqid.ToString(),
                Text = $"{x.EquipmentName} - {x.CurrentLocation}"
            })
            .ToList();


        // ===============================
        // Biomass Fuel Rates
        // ===============================

        var fuelRates = GetBiomassFuelRate();


        // ===============================
        // Main ViewModel
        // ===============================

        var model = new BiomasBoilerReadingViewModel
        {
            BiomasBoilerReading = biomasBoilerReading,

            JuteRate = fuelRates.JuteRate,
            RiceHuskRate = fuelRates.RiceHuskRate,
            WasteWoodRate = fuelRates.WasteWoodRate,
            CartonRate = fuelRates.CartonRate,
            CharcoilRate = fuelRates.CharcoilRate,

            EquipmentList = equipmentList
        };

        return View(model);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BiomasBoilerReadingViewModel biomasBoilerReadingViewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";

                await PopulateCreateViewModel(biomasBoilerReadingViewModel);

                return View(biomasBoilerReadingViewModel);
            }

            var biomasBoilerReading = biomasBoilerReadingViewModel.BiomasBoilerReading;

            // Normalize date (only date part)
            var dateOnly = biomasBoilerReading.Trdate;

            // Check duplicate: Same Machine + Same Date
            var isExists = await _context.TblBiomasBoilerReading
                .AnyAsync(x =>
                    x.Eqid == biomasBoilerReading.Eqid &&
                    x.Trdate == biomasBoilerReading.Trdate
                );


            if (isExists)
            {
                ModelState.AddModelError(
                    "",
                    "This machine already has a reading for this date!"
                );

                await PopulateCreateViewModel(biomasBoilerReadingViewModel);

                return View(biomasBoilerReadingViewModel);
            }

            // Save current time while keeping selected date
            var now = DateTime.Now;

            biomasBoilerReading.Trdate = biomasBoilerReading.Trdate;
            _context.TblBiomasBoilerReading.Add(biomasBoilerReading);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reading created successfully.";
            return RedirectToAction(nameof(BiomasBoilerReadingList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create reading.";

            await PopulateCreateViewModel(biomasBoilerReadingViewModel);

            return View(biomasBoilerReadingViewModel);
        }
    }
    //Helper Method
    private async Task PopulateCreateViewModel(BiomasBoilerReadingViewModel biomasBoilerReadingViewModel)
    {
        // Get current user
        var userId = _userManager.GetUserId(User);

        var currentLocation = await _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefaultAsync();

        // Equipment List
        var query = _context.TblEquipmentDetails
            .Where(x => EF.Functions.Like(x.EquipmentName, "%BOILER%"));

        if (!string.IsNullOrEmpty(currentLocation))
        {
            query = query.Where(x => x.CurrentLocation == currentLocation);
        }

        biomasBoilerReadingViewModel.EquipmentList = await query
            .Select(x => new SelectListItem
            {
                Value = x.Eqid.ToString(),
                Text = $"{x.EquipmentName} - {x.CurrentLocation}"
            })
            .ToListAsync();

        // Biomass Fuel Rates
        var fuelRates = GetBiomassFuelRate();

        biomasBoilerReadingViewModel.JuteRate = fuelRates.JuteRate;
        biomasBoilerReadingViewModel.RiceHuskRate = fuelRates.RiceHuskRate;
        biomasBoilerReadingViewModel.WasteWoodRate = fuelRates.WasteWoodRate;
        biomasBoilerReadingViewModel.CartonRate = fuelRates.CartonRate;
    }


    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var reading = await _context.TblBiomasBoilerReading
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (reading == null)
            return NotFound();

        var biomasBoilerReadingViewModel = new BiomasBoilerReadingViewModel
        {
            BiomasBoilerReading = reading
        };

        await PopulateCreateViewModel(biomasBoilerReadingViewModel);

        return View(biomasBoilerReadingViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BiomasBoilerReadingViewModel biomasBoilerReadingViewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                await PopulateCreateViewModel(biomasBoilerReadingViewModel);
                return View(biomasBoilerReadingViewModel);
            }

            var input = biomasBoilerReadingViewModel.BiomasBoilerReading;

            var existingReading = await _context.TblBiomasBoilerReading
                .FirstOrDefaultAsync(x => x.Trid == input.Trid);

            if (existingReading == null)
                return NotFound();

            // Basic Information
            existingReading.Trdate = input.Trdate;
            existingReading.Eqid = input.Eqid;
            existingReading.Company = input.Company;

            // Biomass Consumption
            existingReading.JuteCons = input.JuteCons;
            existingReading.RhCons = input.RhCons;
            existingReading.WwCons = input.WwCons;
            existingReading.CartonCons = input.CartonCons;
            existingReading.CcCons = input.CcCons;

            // Biomass Cost
            existingReading.JuteCost = input.JuteCost;
            existingReading.RhCost = input.RhCost;
            existingReading.WwCost = input.WwCost;
            existingReading.CartonCost = input.CartonCost;
            existingReading.CcCost = input.CcCost;

            // Total Biomass Cost
            existingReading.TotalBioCost = input.TotalBioCost;

            // Other Costs
            existingReading.LabourCost = input.LabourCost;
            existingReading.ElectricityCost = input.ElectricityCost;
            existingReading.MaintenanceCost = input.MaintenanceCost;
            existingReading.OtherCost = input.OtherCost;

            // Total Cost
            existingReading.TotalCost = input.TotalCost;

            // Production / Consumption
            existingReading.WaterCons = input.WaterCons;
            existingReading.SteamGeneration = input.SteamGeneration;
            existingReading.CostPerKgSteam = input.CostPerKgSteam;

            // Audit Information
            existingReading.UpdatedAt = DateTime.Now;
            existingReading.UpdatedBy = _userManager.GetUserId(User);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reading updated successfully.";

            return RedirectToAction(nameof(BiomasBoilerReadingList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to update reading.";

            await PopulateCreateViewModel(biomasBoilerReadingViewModel);

            return View(biomasBoilerReadingViewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblBiomasBoilerReading
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (data == null)
            return NotFound();

        var model = new BiomasBoilerReadingViewModel
        {
            BiomasBoilerReading = data
        };

        return View(model);
    }


    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblBiomasBoilerReading
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Reading not found.";
            return RedirectToAction(nameof(BiomasBoilerReadingList));
        }

        _context.TblBiomasBoilerReading.Remove(data);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Reading deleted successfully.";

        return RedirectToAction(nameof(BiomasBoilerReadingList));
    }
}



