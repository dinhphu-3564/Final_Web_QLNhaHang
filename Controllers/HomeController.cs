using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_QLNhaHang.Data;
using Web_QLNhaHang.Models;

namespace Web_QLNhaHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var featuredDishes = await _context.Dishes
                .Where(d => d.IsFeatured && d.IsAvailable)
                .Include(d => d.Category)
                .OrderBy(d => d.DisplayOrder)
                .Take(6)
                .ToListAsync();

            var promotions = await _context.Promotions
                .Where(p => p.IsActive && 
                           (p.StartDate == null || p.StartDate <= DateTime.Now) &&
                           (p.EndDate == null || p.EndDate >= DateTime.Now))
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();

            ViewBag.FeaturedDishes = featuredDishes;
            ViewBag.Promotions = promotions;

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}

