using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_QLNhaHang.Data;
using Web_QLNhaHang.Models;

namespace Web_QLNhaHang.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review)
        {
            if (ModelState.IsValid)
            {
                review.CreatedDate = DateTime.Now;
                review.IsApproved = false; // Cần admin duyệt

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                TempData["ReviewMessage"] = "Cảm ơn bạn đã đánh giá! Đánh giá của bạn sẽ được duyệt trước khi hiển thị.";
                
                if (review.DishId.HasValue)
                {
                    return RedirectToAction("Details", "Menu", new { id = review.DishId });
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}

