using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_QLNhaHang.Data;
using Web_QLNhaHang.Controllers.Admin;

namespace Web_QLNhaHang.Controllers.Admin
{
    public class DashboardController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Admin")]
        [Route("Admin/Dashboard")]
        [Route("Admin/Dashboard/Index")]
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);

            var stats = new
            {
                TotalOrders = await _context.Orders.CountAsync(),
                TodayOrders = await _context.Orders.CountAsync(o => o.OrderDate.Date == today),
                MonthlyRevenue = await _context.Orders
                    .Where(o => o.OrderDate >= thisMonth && o.PaymentStatus == "Đã thanh toán")
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0,
                PendingBookings = await _context.Bookings.CountAsync(b => b.Status == "Chờ"),
                PendingOrders = await _context.Orders.CountAsync(o => o.Status == "Chờ"),
                TotalCustomers = await _context.Customers.CountAsync(),
                AvailableDishes = await _context.Dishes.CountAsync(d => d.IsAvailable),
                TotalTables = await _context.Tables.CountAsync(t => t.IsActive)
            };

            var recentOrders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .ToListAsync();

            var recentBookings = await _context.Bookings
                .Include(b => b.Table)
                .OrderByDescending(b => b.CreatedDate)
                .Take(10)
                .ToListAsync();

            ViewBag.Stats = stats;
            ViewBag.RecentOrders = recentOrders;
            ViewBag.RecentBookings = recentBookings;

            return View();
        }
    }
}

