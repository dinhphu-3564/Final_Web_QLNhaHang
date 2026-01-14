using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_QLNhaHang.Data;
using Web_QLNhaHang.Controllers.Admin;

namespace Web_QLNhaHang.Controllers.Admin
{
    [Route("Admin/Booking/[action]")]
    public class AdminBookingController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;

        public AdminBookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? status)
        {
            var query = _context.Bookings
                .Include(b => b.Table)
                .ThenInclude(t => t!.Area)
                .Include(b => b.Customer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(b => b.Status == status);
            }

            var bookings = await query.OrderByDescending(b => b.CreatedDate).ToListAsync();
            ViewBag.Status = status;
            return View("~/Views/Admin/Booking/Index.cshtml", bookings);
        }

        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Table)
                .ThenInclude(t => t!.Area)
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Booking/Details.cshtml", booking);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.Status = status;

            if (status == "Xác nhận" && booking.TableId.HasValue)
            {
                var table = await _context.Tables.FindAsync(booking.TableId.Value);
                if (table != null)
                {
                    table.Status = "Đã đặt";
                }
            }
            else if (status == "Hoàn tất" && booking.TableId.HasValue)
            {
                var table = await _context.Tables.FindAsync(booking.TableId.Value);
                if (table != null)
                {
                    table.Status = "Trống";
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                if (booking.TableId.HasValue)
                {
                    var table = await _context.Tables.FindAsync(booking.TableId.Value);
                    if (table != null)
                    {
                        table.Status = "Trống";
                    }
                }
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

