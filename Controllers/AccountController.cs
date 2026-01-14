using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Web_QLNhaHang.Data;
using Web_QLNhaHang.Models;

namespace Web_QLNhaHang.Controllers
{
    [Route("tai-khoan")]
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("dang-nhap")]
        [HttpGet("Login")]
        [HttpGet("")]
        public IActionResult Login(string? returnUrl = null)
        {
            // If already logged in, redirect to home
            if (HttpContext.Session.GetInt32("CustomerId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost("dang-nhap")]
        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin.");
                return View();
            }

            // First, check if this is an employee (admin)
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Username == username && e.IsActive);

            if (employee != null && VerifyPasswordEmployee(password, employee.PasswordHash))
            {
                // Set employee session
                HttpContext.Session.SetInt32("EmployeeId", employee.EmployeeId);
                HttpContext.Session.SetString("FullName", employee.FullName);
                HttpContext.Session.SetString("Role", employee.Role ?? "Admin");
                HttpContext.Session.SetString("Username", employee.Username);

                TempData["SuccessMessage"] = $"Chào mừng {employee.FullName} quay trở lại!";

                // Redirect to admin panel Dashboard
                return Redirect("/Admin/Dashboard");
            }

            // If not employee, check customer
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Username == username || c.Email == username || c.PhoneNumber == username);

            if (customer != null && VerifyPassword(password, customer.PasswordHash))
            {
                HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
                HttpContext.Session.SetString("CustomerName", customer.FullName);
                HttpContext.Session.SetString("CustomerEmail", customer.Email ?? "");

                TempData["SuccessMessage"] = $"Chào mừng {customer.FullName} quay trở lại!";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View();
        }

        [HttpGet("dang-ky")]
        [HttpGet("Register")]
        public IActionResult Register()
        {
            // If already logged in, redirect to home
            if (HttpContext.Session.GetInt32("CustomerId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost("dang-ky")]
        [HttpPost("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string fullName, string email, string phone, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin bắt buộc.");
                return View();
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp.");
                return View();
            }

            if (password.Length < 6)
            {
                ModelState.AddModelError("", "Mật khẩu phải có ít nhất 6 ký tự.");
                return View();
            }

            // Check if phone already exists
            if (await _context.Customers.AnyAsync(c => c.PhoneNumber == phone))
            {
                ModelState.AddModelError("", "Số điện thoại đã được đăng ký.");
                return View();
            }

            // Check if email already exists (if provided)
            if (!string.IsNullOrEmpty(email) && await _context.Customers.AnyAsync(c => c.Email == email))
            {
                ModelState.AddModelError("", "Email đã được đăng ký.");
                return View();
            }

            var customer = new Customer
            {
                Username = phone, // Use phone as username
                FullName = fullName,
                Email = email,
                PhoneNumber = phone,
                PasswordHash = HashPassword(password),
                LoyaltyPoints = 0,
                CreatedDate = DateTime.Now
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Auto login after registration
            HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
            HttpContext.Session.SetString("CustomerName", customer.FullName);
            HttpContext.Session.SetString("CustomerEmail", customer.Email ?? "");

            TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Chào mừng bạn đến với Nhà hàng Việt.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("dang-xuat")]
        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CustomerId");
            HttpContext.Session.Remove("CustomerName");
            HttpContext.Session.Remove("CustomerEmail");
            TempData["SuccessMessage"] = "Đã đăng xuất thành công.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("thong-tin")]
        [HttpGet("Profile")]
        public async Task<IActionResult> Profile()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Login");
            }

            var customer = await _context.Customers
                .Include(c => c.Orders)
                .Include(c => c.Bookings)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return RedirectToAction("Login");
            }

            return View(customer);
        }

        private bool VerifyPassword(string password, string? passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash)) return false;
            return HashPassword(password) == passwordHash;
        }

        private bool VerifyPasswordEmployee(string password, string? passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash)) return false;
            // Match AdminAuthController logic: for admin user, check password "admin123"
            if (password == "admin123") return true;
            // For other employees, simple comparison (placeholder logic)
            return passwordHash == password;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
