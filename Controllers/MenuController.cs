using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_QLNhaHang.Data;
using Web_QLNhaHang.Models;

namespace Web_QLNhaHang.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId, string? searchTerm, string? sort, int page = 1)
        {
            int pageSize = 5; // 5 dishes per page

            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            var query = _context.Dishes
                .Include(d => d.Category)
                .Where(d => d.IsAvailable);

            if (categoryId.HasValue)
            {
                query = query.Where(d => d.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(d => d.DishName.Contains(searchTerm) || 
                                        (d.Description != null && d.Description.Contains(searchTerm)));
            }

            // Sorting
            switch (sort)
            {
                case "price_asc":
                    query = query.OrderBy(d => d.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(d => d.Price);
                    break;
                default:
                    query = query.OrderBy(d => d.DisplayOrder);
                    break;
            }

            int totalItems = await query.CountAsync();
            var dishes = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CurrentSort = sort;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(dishes);
        }

        public async Task<IActionResult> Details(int id)
        {
            var dish = await _context.Dishes
                .Include(d => d.Category)
                .Include(d => d.Reviews!.Where(r => r.IsApproved))
                .FirstOrDefaultAsync(d => d.DishId == id);

            if (dish == null)
            {
                return NotFound();
            }

            // Related dishes (same category, exclude current)
            var relatedDishes = await _context.Dishes
                .Where(d => d.CategoryId == dish.CategoryId && d.DishId != id && d.IsAvailable)
                .OrderBy(r => Guid.NewGuid()) // Randomize
                .Take(4)
                .ToListAsync();

            ViewBag.RelatedDishes = relatedDishes;

            return View(dish);
        }
    }
}

