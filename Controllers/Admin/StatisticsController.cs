using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_QLNhaHang.Data;

namespace Web_QLNhaHang.Controllers.Admin
{
    public class StatisticsController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Admin/Statistics")]
        [Route("Admin/Statistics/Index")]
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);

            // Thống kê tổng quan
            var stats = new
            {
                TotalOrders = await _context.Orders.CountAsync(),
                TotalRevenue = await _context.Orders
                    .Where(o => o.PaymentStatus == "Đã thanh toán")
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0,
                TotalCustomers = await _context.Customers.CountAsync(),
                TotalDishes = await _context.Dishes.CountAsync(),
                MonthlyRevenue = await _context.Orders
                    .Where(o => o.OrderDate >= thisMonth && o.PaymentStatus == "Đã thanh toán")
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0,
                MonthlyOrders = await _context.Orders
                    .Where(o => o.OrderDate >= thisMonth)
                    .CountAsync()
            };

            // Doanh thu theo tháng (12 tháng gần nhất)
            var monthlyRevenue = new List<object>();
            for (int i = 11; i >= 0; i--)
            {
                var month = today.AddMonths(-i);
                var startOfMonth = new DateTime(month.Year, month.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1);
                
                var revenue = await _context.Orders
                    .Where(o => o.OrderDate >= startOfMonth && o.OrderDate < endOfMonth && o.PaymentStatus == "Đã thanh toán")
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
                
                monthlyRevenue.Add(new
                {
                    Month = startOfMonth.ToString("MM/yyyy"),
                    Revenue = revenue
                });
            }

            // Đơn hàng theo ngày (7 ngày gần nhất)
            var dailyOrders = new List<object>();
            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var count = await _context.Orders
                    .CountAsync(o => o.OrderDate.Date == date);
                
                dailyOrders.Add(new
                {
                    Date = date.ToString("dd/MM"),
                    Count = count
                });
            }

            // Phương thức thanh toán
            var paymentMethods = await _context.Orders
                .GroupBy(o => o.PaymentMethod)
                .Select(g => new
                {
                    Method = g.Key ?? "COD",
                    Count = g.Count()
                })
                .ToListAsync();

            // Top 5 món ăn bán chạy
            var topDishes = await _context.OrderDetails
                .Include(od => od.Dish)
                .GroupBy(od => new { od.DishId, od.Dish!.DishName })
                .Select(g => new
                {
                    DishName = g.Key.DishName,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.Quantity)
                .Take(5)
                .ToListAsync();

            // Trạng thái đơn hàng
            var orderStatuses = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            ViewBag.Stats = stats;
            ViewBag.MonthlyRevenue = System.Text.Json.JsonSerializer.Serialize(monthlyRevenue);
            ViewBag.DailyOrders = System.Text.Json.JsonSerializer.Serialize(dailyOrders);
            ViewBag.PaymentMethods = System.Text.Json.JsonSerializer.Serialize(paymentMethods);
            ViewBag.TopDishes = System.Text.Json.JsonSerializer.Serialize(topDishes);
            ViewBag.OrderStatuses = System.Text.Json.JsonSerializer.Serialize(orderStatuses);

            return View();
        }
    }
}
