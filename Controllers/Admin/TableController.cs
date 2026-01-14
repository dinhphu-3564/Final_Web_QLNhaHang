using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_QLNhaHang.Data;
using Web_QLNhaHang.Models;
using Web_QLNhaHang.Controllers.Admin;

namespace Web_QLNhaHang.Controllers.Admin
{
    [Route("Admin/[controller]/[action]")]
    public class TableController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;

        public TableController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tables = await _context.Tables
                .Include(t => t.Area)
                .OrderBy(t => t.TableNumber)
                .ToListAsync();
            return View(tables);
        }

        public async Task<IActionResult> Create()
        {
            var areas = await _context.TableAreas.Where(a => a.IsActive).ToListAsync();
            ViewBag.Areas = new SelectList(areas, "AreaId", "AreaName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Table table)
        {
            if (ModelState.IsValid)
            {
                _context.Tables.Add(table);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var areas = await _context.TableAreas.Where(a => a.IsActive).ToListAsync();
            ViewBag.Areas = new SelectList(areas, "AreaId", "AreaName", table.AreaId);
            return View(table);
        }

        [Route("Admin/Table/Edit")]
        [Route("Admin/Table/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(Index));
            }
            
            var table = await _context.Tables.FindAsync(id.Value);
            if (table == null)
            {
                return NotFound();
            }

            var areas = await _context.TableAreas.Where(a => a.IsActive).ToListAsync();
            ViewBag.Areas = new SelectList(areas, "AreaId", "AreaName", table.AreaId);
            return View(table);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Table/Edit/{id}")]
        public async Task<IActionResult> Edit(int id, Table table)
        {
            if (id != table.TableId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(table);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TableExists(table.TableId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var areas = await _context.TableAreas.Where(a => a.IsActive).ToListAsync();
            ViewBag.Areas = new SelectList(areas, "AreaId", "AreaName", table.AreaId);
            return View(table);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table != null)
            {
                _context.Tables.Remove(table);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TableExists(int id)
        {
            return _context.Tables.Any(e => e.TableId == id);
        }
    }
}

