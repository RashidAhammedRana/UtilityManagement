using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UtilityManagement.Data;
using UtilityManagement.Models;


public class FncItemRateController : Controller
{
    private readonly ApplicationDbContext _context;

    public FncItemRateController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> FncItemRateList(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "F&C Item Rate")
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

        var query = _context.TblFncItemRates
            .Include(x => x.Fnc) 
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            query = query.Where(x =>
                x.Fnc.ItemName.Contains(searchString) ||
                x.Date.ToString().Contains(searchString) ||
                x.Rate.ToString().Contains(searchString)
            );
        }

        var totalRecords = await query.CountAsync();

        var totalRates = await query
            .OrderBy(x => x.Fnc.ItemName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.SearchString = searchString;
        ViewBag.totalRates = totalRecords;

        return View(totalRates);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new TblFncItemRate
        {
            Date = DateTime.Today
        };
        ViewBag.ItemList = _context.TblFncItems
           .Select(x => new SelectListItem
           {
               Value = x.Fncid.ToString(),
               Text = $"{x.ItemName}"
           })
           .ToList();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblFncItemRate fncItemRate)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(fncItemRate);
            }

            _context.TblFncItemRates.Add(fncItemRate);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Created successfully.";

            return RedirectToAction(nameof(FncItemRateList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create.";
            return View(fncItemRate);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var itemRates = await _context.TblFncItemRates.FindAsync(id);
        if (itemRates == null)
        {
            return NotFound();
        }
        ViewBag.ItemList = _context.TblFncItems
            .Select(x => new SelectListItem
            {
                Value = x.Fncid.ToString(),
                Text = $"{x.ItemName}"
            })
            .ToList();
        return View(itemRates);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblFncItemRate fncItemRate)
    {
        if (!ModelState.IsValid)
            return View(fncItemRate);

        var data = await _context.TblFncItemRates
            .FirstOrDefaultAsync(x => x.Fncrid == fncItemRate.Fncrid);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Data not found.";
            return RedirectToAction(nameof(FncItemRateList));
        }

        data.Fncid = fncItemRate.Fncid;
        data.Rate = fncItemRate.Rate;
        data.Date = fncItemRate.Date;
        data.Remarks = fncItemRate.Remarks;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Updated successfully.";

        return RedirectToAction(nameof(FncItemRateList));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblFncItemRates.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblFncItemRates
            .FirstOrDefaultAsync(x => x.Fncrid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Data not found.";
            return RedirectToAction(nameof(FncItemRateList));
        }

        _context.TblFncItemRates.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Data deleted successfully.";

        return RedirectToAction(nameof(FncItemRateList));
    }
}

