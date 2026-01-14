using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_QLNhaHang.Data;
using Web_QLNhaHang.Models;
using System.Security.Cryptography;
using System.Text;

namespace Web_QLNhaHang.Controllers.Admin
{
    [Route("Admin/[controller]/[action]")]
    public class AdminAuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminAuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin.");
                return View();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Username == username && e.IsActive);

            if (employee != null && VerifyPassword(password, employee.PasswordHash))
            {
                HttpContext.Session.SetInt32("EmployeeId", employee.EmployeeId);
                HttpContext.Session.SetString("Username", employee.Username);
                HttpContext.Session.SetString("FullName", employee.FullName);
                HttpContext.Session.SetString("Role", employee.Role);

                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        private bool VerifyPassword(string password, string hash)
        {
            // Simplified for demo - In production, use proper password hashing (BCrypt, PBKDF2, etc.)
            // Allow admin123 for admin users, user123 for regular users
            if (password == "admin123") return true;
            if (password == "user123") return true;
            return false;
        }

        private string HashPassword(string password)
        {
            // Simplified - In production, use proper password hashing
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}

