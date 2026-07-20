using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UtilityManagement.Data;
using UtilityManagement.Models;


public class FncItemInfoController : Controller
{
    private readonly ApplicationDbContext _context;

    public FncItemInfoController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> FncItemInfoList(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "F&C Item Info")
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

        var query = _context.TblFncItems
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            query = query.Where(x =>
                x.ItemName.Contains(searchString) ||
                x.Uom.ToString().Contains(searchString) ||
                x.Status.ToString().Contains(searchString)
            );
        }

        var totalRecords = await query.CountAsync();

        var totalItems = await query
            .OrderBy(x => x.ItemName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.SearchString = searchString;
        ViewBag.totalItemInfo = totalRecords;

        return View(totalItems);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblFncItem fncItem)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(fncItem);
            }

            _context.TblFncItems.Add(fncItem);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Created successfully.";

            return RedirectToAction(nameof(FncItemInfoList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create.";
            return View(fncItem);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var itemInfo = await _context.TblFncItems.FindAsync(id);
        if (itemInfo == null)
        {
            return NotFound();
        }
        return View(itemInfo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblFncItem fncItem)
    {
        if (!ModelState.IsValid)
            return View(fncItem);

        var data = await _context.TblFncItems
            .FirstOrDefaultAsync(x => x.Fncid == fncItem.Fncid);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Data not found.";
            return RedirectToAction(nameof(FncItemInfoList));
        }

        data.Fncid = fncItem.Fncid;
        data.ItemName = fncItem.ItemName;
        data.Uom = fncItem.Uom;
        data.Status = fncItem.Status;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Updated successfully.";

        return RedirectToAction(nameof(FncItemInfoList));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblFncItems.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblFncItems
            .FirstOrDefaultAsync(x => x.Fncid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Data not found.";
            return RedirectToAction(nameof(FncItemInfoList));
        }

        _context.TblFncItems.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Data deleted successfully.";

        return RedirectToAction(nameof(FncItemInfoList));
    }
}

