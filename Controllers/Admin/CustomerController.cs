using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_QLNhaHang.Data;
using Web_QLNhaHang.Controllers.Admin;

namespace Web_QLNhaHang.Controllers.Admin
{
    [Route("Admin/[controller]/[action]")]
    public class CustomerController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchTerm)
        {
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.FullName.Contains(searchTerm) ||
                                        (c.PhoneNumber != null && c.PhoneNumber.Contains(searchTerm)) ||
                                        (c.Email != null && c.Email.Contains(searchTerm)));
            }

            var customers = await query.OrderByDescending(c => c.CreatedDate).ToListAsync();
            ViewBag.SearchTerm = searchTerm;
            return View(customers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Orders)
                .Include(c => c.Bookings)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
    }
}

