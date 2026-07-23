using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UtilityManagement.Data;
using UtilityManagement.Models;

public class ReasonInfoController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReasonInfoController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ReasonInfoList(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Reason Information")
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

        var query = _context.TblReasonInfo.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();
            query = query.Where(x =>
                x.ReasonName.Contains(searchString) ||
                x.Status.Contains(searchString) ||
                (x.Remarks != null && x.Remarks.Contains(searchString))
            );
        }

        var totalRecords = await query.CountAsync();

        var data = await query
            .OrderBy(x => x.ReasonName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.SearchString = searchString;
        ViewBag.totalReasons = totalRecords;

        return View(data);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {

        ViewBag.InterruptiotypeList = await _context.TblInterruptionTypeInfo
        .OrderBy(x => x.InterruptionTypeName)
        .Select(x => new SelectListItem
        {
            Value = x.Itid.ToString(),
            Text = x.InterruptionTypeName
        })
        .ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblReasonInfo reasonInfo)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(reasonInfo);
            }

            // ✅ CHECK DUPLICATE SL NO
            var isExist = await _context.TblReasonInfo
                .AnyAsync(x => x.ReasonName == reasonInfo.ReasonName);

            if (isExist)
            {
                ModelState.AddModelError("ReasonName", "Reason already exists!");
                return View(reasonInfo);
            }

            _context.TblReasonInfo.Add(reasonInfo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reason Info Created successfully.";

            return RedirectToAction(nameof(ReasonInfoList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create.";
            return View(reasonInfo);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        ViewBag.InterruptiotypeList = await _context.TblInterruptionTypeInfo
        .OrderBy(x => x.InterruptionTypeName)
        .Select(x => new SelectListItem
        {
            Value = x.Itid.ToString(),
            Text = x.InterruptionTypeName
        })
        .ToListAsync();
        if (id == null)
        {
            return NotFound();
        }

        var Reasoninfo = await _context.TblReasonInfo.FindAsync(id);
        if (Reasoninfo == null)
        {
            return NotFound();
        }
        return View(Reasoninfo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblReasonInfo reasonInfo)
    {
        if (!ModelState.IsValid)
            return View(reasonInfo);

        var data = await _context.TblReasonInfo
            .FirstOrDefaultAsync(x => x.Rid == reasonInfo.Rid);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Reason not found.";
            return RedirectToAction(nameof(ReasonInfoList));
        }

        data.ReasonName = reasonInfo.ReasonName;
        data.Status = reasonInfo.Status;
        data.Remarks = reasonInfo.Remarks;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Reason Info Updated successfully.";

        return RedirectToAction(nameof(ReasonInfoList));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblReasonInfo.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblReasonInfo
            .FirstOrDefaultAsync(x => x.Rid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Reason Info not found.";
            return RedirectToAction(nameof(ReasonInfoList));
        }

        _context.TblReasonInfo.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Reason Info deleted successfully.";

        return RedirectToAction(nameof(ReasonInfoList));
    }
}

