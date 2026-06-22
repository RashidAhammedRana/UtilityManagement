using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UtilityManagement.Data;
using UtilityManagement.Models;

public class RebReadingInfoController : Controller
{
    private readonly ApplicationDbContext _context;

    public RebReadingInfoController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> RebReadingInfoList(int page = 1, int pageSize = 15)
    {

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "REB Reading Info")
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

        // 📄 Pagination data
        var rebReadings = await _context.TblRebReadingInfo
            .OrderBy(r => r.Trid)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalRecords = await _context.TblRebReadingInfo.CountAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.totalRebReadings = totalRecords;

        return View(rebReadings);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new TblRebReadingInfo
        {
            Trdate = DateTime.Today
        };
        ViewBag.EquipmentList = _context.TblEquipmentDetails
        .Select(x => new SelectListItem
        {
            Value = x.Eqid.ToString(),
            Text = x.EquipmentName
        })
        .ToList();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblRebReadingInfo rebReadingInfo)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(rebReadingInfo);
            }
            // Save current date & time
            var now = DateTime.Now;

            rebReadingInfo.Trdate = rebReadingInfo.Trdate?.Date
                .AddHours(now.Hour)
                .AddMinutes(now.Minute)
                .AddSeconds(now.Second);

            _context.TblRebReadingInfo.Add(rebReadingInfo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reading created successfully.";

            return RedirectToAction(nameof(RebReadingInfoList));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Failed to create reading.";

            return View(rebReadingInfo);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var reading = await _context.TblRebReadingInfo
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (reading == null)
            return NotFound();

        ViewBag.EquipmentList = _context.TblEquipmentDetails
            .Select(x => new SelectListItem
            {
                Value = x.Eqid.ToString(),
                Text = x.EquipmentName
            })
            .ToList();

        return View(reading);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblRebReadingInfo rebReadingInfo)
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

                return View(rebReadingInfo);
            }

            _context.Update(rebReadingInfo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reading updated successfully.";

            return RedirectToAction(nameof(RebReadingInfoList));
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

            return View(rebReadingInfo);
        }
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblRebReadingInfo.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblRebReadingInfo
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Readings not found.";
            return RedirectToAction(nameof(RebReadingInfoList));
        }

        _context.TblRebReadingInfo.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Readings deleted successfully.";

        return RedirectToAction(nameof(RebReadingInfoList));
    }
}
