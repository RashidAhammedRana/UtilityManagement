using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UtilityManagement.Data;
using UtilityManagement.Models;
using System.Globalization;

public class BoilerRmsRoomInfoController : Controller
{
    private readonly ApplicationDbContext _context;

    public BoilerRmsRoomInfoController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> BoilerRmsRoomInfoList(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Boiler RMS Room")
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

        // =========================
        // BASE QUERY
        // =========================
        var query = _context.TblBoilerRmsRoom
            .Include(x => x.Eq)
            .AsQueryable();

        // =========================
        // SEARCH LOGIC
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            var parts = searchString.Split('-', StringSplitOptions.RemoveEmptyEntries);

            int number;
            bool isNumber = int.TryParse(searchString, out number);

            // =========================
            // CASE 1: FULL DATE (22-06-2026 OR 2026-06-22 OR 22/06/2026)
            // =========================
            bool isFullDate =
                DateTime.TryParseExact(
                    searchString,
                    new[] {
                "dd-MM-yyyy", "d-M-yyyy",
                "dd/MM/yyyy", "d/M/yyyy",
                "yyyy-MM-dd", "yyyy/MM/dd"
                    },
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime parsedDate
                );

            if (isFullDate)
            {
                var start = parsedDate.Date;
                var end = start.AddDays(1);

                query = query.Where(x =>
                    x.Trdate.HasValue &&
                    x.Trdate.Value >= start &&
                    x.Trdate.Value < end
                );
            }

            // =========================
            // CASE 2: YEAR-MONTH (2026-06 / 2026/06)
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
            // CASE 3: MONTH-DAY (06-22 / 22-06)
            // =========================
            else if (parts.Length == 2)
            {
                if (int.TryParse(parts[0], out int a) &&
                    int.TryParse(parts[1], out int b))
                {
                    // assume MM-DD or DD-MM both supported
                    query = query.Where(x =>
                        x.Trdate.HasValue &&
                        (
                            (x.Trdate.Value.Month == a && x.Trdate.Value.Day == b) ||
                            (x.Trdate.Value.Month == b && x.Trdate.Value.Day == a)
                        )
                    );
                }
            }

            // =========================
            // CASE 4: SINGLE NUMBER (day/month/year)
            // =========================
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

            // =========================
            // CASE 5: TEXT SEARCH
            // =========================
            else
            {
                query = query.Where(x =>
                    x.Eq.EquipmentName.Contains(searchString) ||
                    x.Eq.CurrentLocation.Contains(searchString)
                );
            }
        }

        // =========================
        // PAGINATION
        // =========================
        var totalRecords = await query.CountAsync();

        var roomReadings = await query
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

        return View(roomReadings);
    }
    [HttpGet]
    public IActionResult Create()
    {
        var model = new TblBoilerRmsRoom
        {
            Trdate = DateTime.Today
        };

        var ngRate = _context.TblNgRate
                     .OrderByDescending(x => x.NgrId)
                     .Select(x => x.Rate)
                     .FirstOrDefault();

        ViewBag.NgRate = ngRate;

        ViewBag.EquipmentList = _context.TblEquipmentDetails
            .Select(x => new SelectListItem
            {
                Value = x.Eqid.ToString(),
                Text = $"{x.EquipmentName} - {x.CurrentLocation}"
            })
            .ToList();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblBoilerRmsRoom boilerRmsRoom)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(boilerRmsRoom);
            }

            // Normalize date (only date part)
            var dateOnly = boilerRmsRoom.Trdate?.Date;

            // ❌ CHECK DUPLICATE: Same Machine + Same Date
            var isExists = await _context.TblSolarReadingInfos
                .AnyAsync(x =>
                    x.Eqid == boilerRmsRoom.Eqid &&
                    x.Trdate.HasValue &&
                    x.Trdate.Value.Date == dateOnly
                );

            if (isExists)
            {
                ModelState.AddModelError("", "This machine already has a reading for this date!");
                return View(boilerRmsRoom);
            }

            // Save current time (keep datetime but same date)
            var now = DateTime.Now;

            boilerRmsRoom.Trdate = dateOnly?
                .AddHours(now.Hour)
                .AddMinutes(now.Minute)
                .AddSeconds(now.Second);

            _context.TblBoilerRmsRoom.Add(boilerRmsRoom);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reading created successfully.";

            return RedirectToAction(nameof(BoilerRmsRoomInfoList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create reading.";
            return View(boilerRmsRoom);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var reading = await _context.TblBoilerRmsRoom
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (reading == null)
            return NotFound();


        ViewBag.EquipmentList = _context.TblEquipmentDetails
            .Select(x => new SelectListItem
            {
                Value = x.Eqid.ToString(),
                Text = $"{x.EquipmentName} - {x.CurrentLocation}"
            })
            .ToList();

        ViewBag.NgRate = await _context.TblNgRate
            .Select(x => x.Rate)
            .FirstOrDefaultAsync();


        return View(reading);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblBoilerRmsRoom boilerRmsRoom)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                ViewBag.EquipmentList = _context.TblEquipmentDetails
                    .Select(x => new SelectListItem
                    {
                        Value = x.Eqid.ToString(),
                        Text = x.EquipmentName
                    })
                    .ToList();

                return View(boilerRmsRoom);
            }

            _context.Update(boilerRmsRoom);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reading updated successfully.";

            return RedirectToAction(nameof(BoilerRmsRoomInfoList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to update reading.";

            ViewBag.EquipmentList = _context.TblEquipmentDetails
                .Select(x => new SelectListItem
                {
                    Value = x.Eqid.ToString(),
                    Text = x.EquipmentName
                })
                .ToList();

            return View(boilerRmsRoom);
        }
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblBoilerRmsRoom.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblBoilerRmsRoom
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Readings not found.";
            return RedirectToAction(nameof(BoilerRmsRoomInfoList));
        }

        _context.TblBoilerRmsRoom.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Readings deleted successfully.";

        return RedirectToAction(nameof(BoilerRmsRoomInfoList));
    }
}

