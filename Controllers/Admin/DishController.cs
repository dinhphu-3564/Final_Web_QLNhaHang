using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_QLNhaHang.Data;
using Web_QLNhaHang.Models;
using Web_QLNhaHang.Controllers.Admin;

namespace Web_QLNhaHang.Controllers.Admin
{
    [Route("Admin/[controller]/[action]")]
    public class DishController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DishController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(int? categoryId, string? searchTerm)
        {
            var query = _context.Dishes.Include(d => d.Category).AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(d => d.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(d => d.DishName.Contains(searchTerm));
            }

            var dishes = await query.OrderBy(d => d.DisplayOrder).ToListAsync();
            var categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName", categoryId);
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SearchTerm = searchTerm;

            return View(dishes);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Dish dish, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "dishes");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    dish.ImageUrl = "/images/dishes/" + fileName;
                }

                dish.CreatedDate = DateTime.Now;
                _context.Dishes.Add(dish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName", dish.CategoryId);
            return View(dish);
        }

        [Route("/Admin/Dish/Edit")]
        [Route("/Admin/Dish/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(Index));
            }
            
            var dish = await _context.Dishes.FindAsync(id.Value);
            if (dish == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName", dish.CategoryId);
            return View(dish);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/Admin/Dish/Edit/{id}")]
        public async Task<IActionResult> Edit(int id, Dish dish, IFormFile? imageFile)
        {
            if (id != dish.DishId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get existing dish to preserve ImageUrl if no new image uploaded
                    var existingDish = await _context.Dishes.AsNoTracking().FirstOrDefaultAsync(d => d.DishId == id);
                    
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "dishes");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        dish.ImageUrl = "/images/dishes/" + fileName;
                    }
                    else if (existingDish != null)
                    {
                        // Keep existing image URL if no new image uploaded
                        dish.ImageUrl = existingDish.ImageUrl;
                    }

                    // Preserve CreatedDate from original dish
                    if (existingDish != null)
                    {
                        dish.CreatedDate = existingDish.CreatedDate;
                    }

                    _context.Update(dish);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DishExists(dish.DishId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName", dish.CategoryId);
            return View(dish);
        }

        [HttpGet]
        [Route("/Admin/Dish/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dish = await _context.Dishes
                .Include(d => d.Category)
                .FirstOrDefaultAsync(d => d.DishId == id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/Admin/Dish/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish != null)
            {
                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DishExists(int id)
        {
            return _context.Dishes.Any(e => e.DishId == id);
        }
    }
}

