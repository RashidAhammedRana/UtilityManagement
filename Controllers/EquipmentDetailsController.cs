using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UtilityManagement.Data;
using UtilityManagement.Models;


public class EquipmentDetailsController : Controller
{
    private readonly ApplicationDbContext _context;

    public EquipmentDetailsController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> EquipmentDetailsList(int page = 1, int pageSize = 15)
    {
        var equipmentsDetails = await _context.TblEquipmentDetails
            .OrderBy(r => r.EquipmentName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var totalRecords = await _context.TblEquipmentDetails.CountAsync();
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.TotalEquipmentDetails = totalRecords;

        return View(equipmentsDetails);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblEquipmentDetails equipmentDetails)
    {
        if (ModelState.IsValid)
        {
            _context.Add(equipmentDetails);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EquipmentDetailsList));
        }
        return View(equipmentDetails);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var equipments = await _context.TblEquipmentDetails.FindAsync(id);
        if (equipments == null)
        {
            return NotFound();
        }
        return View(equipments);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TblEquipmentDetails equipments)
    {
        if (id != equipments.Eqid)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Entry(equipments).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TblEquipmentDetails.Any(e => e.Eqid == equipments.Eqid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(EquipmentDetailsList));
        }

        return View(equipments);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblEquipmentDetails.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblEquipmentDetails.FindAsync(id);

        if (data != null)
        {
            _context.TblEquipmentDetails.Remove(data);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("EquipmentDetailsList");
    }
}

