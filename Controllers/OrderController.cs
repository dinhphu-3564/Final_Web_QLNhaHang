using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Web_QLNhaHang.Data;
using Web_QLNhaHang.Models;
using Web_QLNhaHang.Services;

namespace Web_QLNhaHang.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IVNPayService _vnPayService;
        private readonly IMoMoService _moMoService;
        private const string CartSessionKey = "Cart";

        public OrderController(ApplicationDbContext context, IVNPayService vnPayService, IMoMoService moMoService)
        {
            _context = context;
            _vnPayService = vnPayService;
            _moMoService = moMoService;
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
        }

        public IActionResult Checkout()
        {
            // Require login for checkout
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Message"] = "Vui lòng đăng nhập để thanh toán.";
                return RedirectToAction("Login", "Account", new { returnUrl = "/Order/Checkout" });
            }

            var cart = GetCart();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            // Pre-fill customer info if logged in
            var customer = _context.Customers.Find(customerId);
            if (customer != null)
            {
                ViewBag.CustomerName = customer.FullName;
                ViewBag.CustomerPhone = customer.PhoneNumber;
                ViewBag.CustomerEmail = customer.Email;
            }

            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(c => c.SubTotal);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(Order order, string paymentMethod)
        {
            // Require login for checkout
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Message"] = "Vui lòng đăng nhập để thanh toán.";
                return RedirectToAction("Login", "Account", new { returnUrl = "/Order/Checkout" });
            }

            var cart = GetCart();
            if (cart.Count == 0)
            {
                ModelState.AddModelError("", "Giỏ hàng trống.");
                return View("Checkout", order);
            }

            if (ModelState.IsValid)
            {
                // Tạo số đơn hàng
                order.OrderNumber = "ORD" + DateTime.Now.ToString("yyyyMMddHHmmss");
                order.OrderDate = DateTime.Now;
                order.Status = "Chờ";
                order.PaymentMethod = paymentMethod;
                order.PaymentStatus = "Chưa thanh toán";
                order.TotalAmount = cart.Sum(c => c.SubTotal);
                order.CustomerId = customerId; // Set CustomerId from session

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Thêm chi tiết đơn hàng
                foreach (var item in cart)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        DishId = item.DishId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price,
                        SubTotal = item.SubTotal,
                        Notes = item.Notes
                    };
                    _context.OrderDetails.Add(orderDetail);
                }
                await _context.SaveChangesAsync();

                // Xử lý theo phương thức thanh toán
                if (paymentMethod == "VNPay")
                {
                    // Tạo URL thanh toán VNPay
                    var ipAddress = "127.0.0.1"; // Force IPv4 for local testing
                    var orderInfo = $"ThanhToan{order.OrderNumber}";
                    var paymentUrl = _vnPayService.CreatePaymentUrl(order.TotalAmount, orderInfo, order.OrderNumber, ipAddress);
                    
                    return Redirect(paymentUrl);
                }
                else if (paymentMethod == "MoMo" || paymentMethod == "MoMoATM")
                {
                    // Tạo URL thanh toán MoMo
                    var orderInfo = $"Thanh toan don hang {order.OrderNumber}";
                    // MoMo: captureWallet, MoMoATM: payWithATM
                    var paymentType = paymentMethod == "MoMoATM" ? "payWithATM" : "captureWallet";
                    var paymentUrl = await _moMoService.CreatePaymentUrl(order.TotalAmount, orderInfo, order.OrderNumber, paymentType);
                    
                    if (!string.IsNullOrEmpty(paymentUrl))
                    {
                        return Redirect(paymentUrl);
                    }
                    else
                    {
                        // Lỗi tạo URL thanh toán MoMo
                        TempData["Error"] = "Không thể tạo thanh toán MoMo. Vui lòng thử lại.";
                        return RedirectToAction("Checkout");
                    }
                }
                else
                {
                    // Thanh toán COD - xóa giỏ hàng và thông báo thành công
                    ClearCart();
                    TempData["OrderSuccess"] = true;
                    TempData["OrderNumber"] = order.OrderNumber;
                    return RedirectToAction("OrderSuccess", new { orderNumber = order.OrderNumber });
                }
            }

            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(c => c.SubTotal);
            return View("Checkout", order);
        }


        [HttpGet]
        public IActionResult PaymentNotify()
        {
            var queryCollection = HttpContext.Request.Query;
            var vnp_SecureHash = queryCollection["vnp_SecureHash"].ToString();

            if (_vnPayService.ValidateSignature(queryCollection, vnp_SecureHash))
            {
                var vnp_ResponseCode = queryCollection["vnp_ResponseCode"].ToString();
                var vnp_TxnRef = queryCollection["vnp_TxnRef"].ToString();
                var vnp_Amount = queryCollection["vnp_Amount"].ToString();
                long vnp_Amount_Long = Convert.ToInt64(vnp_Amount);

                // Tìm đơn hàng
                var order = _context.Orders.FirstOrDefault(o => o.OrderNumber == vnp_TxnRef);

                if (order != null)
                {
                    // Check số tiền (VNPay gửi amount * 100)
                    if ((long)(order.TotalAmount * 100) == vnp_Amount_Long)
                    {
                        if (order.PaymentStatus != "Đã thanh toán")
                        {
                            if (vnp_ResponseCode == "00")
                            {
                                order.PaymentStatus = "Đã thanh toán";
                                order.VnpTransactionNo = queryCollection["vnp_TransactionNo"].ToString();
                                order.VnpResponseCode = vnp_ResponseCode;
                                _context.SaveChanges();
                                return Json(new { RspCode = "00", Message = "Confirm Success" });
                            }
                            else
                            {
                                // Giao dịch thất bại
                                return Json(new { RspCode = "02", Message = "Order already confirmed" }); 
                            }
                        }
                        else
                        {
                            return Json(new { RspCode = "02", Message = "Order already confirmed" });
                        }
                    }
                    else
                    {
                        return Json(new { RspCode = "04", Message = "Invalid amount" });
                    }
                }
                else
                {
                    return Json(new { RspCode = "01", Message = "Order not found" });
                }
            }
            return Json(new { RspCode = "97", Message = "Invalid Checksum" });
        }

        public IActionResult VnPayReturn()
        {
            var queryCollection = HttpContext.Request.Query;
            var vnp_SecureHash = queryCollection["vnp_SecureHash"].ToString();
            
            if (_vnPayService.ValidateSignature(queryCollection, vnp_SecureHash))
            {
                var vnp_ResponseCode = queryCollection["vnp_ResponseCode"].ToString();
                var vnp_TxnRef = queryCollection["vnp_TxnRef"].ToString();
                var vnp_TransactionNo = queryCollection["vnp_TransactionNo"].ToString();
                var vnp_Amount = queryCollection["vnp_Amount"].ToString();

                // Tìm đơn hàng
                var order = _context.Orders.FirstOrDefault(o => o.OrderNumber == vnp_TxnRef);
                
                if (order != null)
                {
                    order.VnpTransactionNo = vnp_TransactionNo;
                    order.VnpResponseCode = vnp_ResponseCode;
                    
                    if (vnp_ResponseCode == "00")
                    {
                        // Thanh toán thành công
                        order.PaymentStatus = "Đã thanh toán";
                        ClearCart();
                        
                        ViewBag.Success = true;
                        ViewBag.Message = "Thanh toán thành công!";
                    }
                    else
                    {
                        // Thanh toán thất bại
                        ViewBag.Success = false;
                        ViewBag.Message = _vnPayService.GetResponseDescription(vnp_ResponseCode);
                    }
                    
                    _context.SaveChanges();
                    ViewBag.Order = order;
                }
                else
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "Không tìm thấy đơn hàng.";
                }
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = "Chữ ký không hợp lệ. Giao dịch có thể đã bị thay đổi.";
            }
            
            return View();
        }

        public async Task<IActionResult> OrderSuccess(string orderNumber)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails!)
                .ThenInclude(od => od.Dish)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> CheckStatus(string phone)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails!)
                .ThenInclude(od => od.Dish)
                .Where(o => o.CustomerPhone == phone)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // Xem đơn hàng của khách hàng đã đăng nhập
        public async Task<IActionResult> MyOrders()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Message"] = "Vui lòng đăng nhập để xem đơn hàng.";
                return RedirectToAction("Login", "Account", new { returnUrl = "/Order/MyOrders" });
            }

            var orders = await _context.Orders
                .Include(o => o.OrderDetails!)
                .ThenInclude(od => od.Dish)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // MoMo Return - Xử lý kết quả thanh toán từ redirect
        public IActionResult MoMoReturn()
        {
            var queryCollection = HttpContext.Request.Query;
            
            var partnerCode = queryCollection["partnerCode"].ToString();
            var orderId = queryCollection["orderId"].ToString();
            var requestId = queryCollection["requestId"].ToString();
            var amount = queryCollection["amount"].ToString();
            var orderInfo = queryCollection["orderInfo"].ToString();
            var orderType = queryCollection["orderType"].ToString();
            var transId = queryCollection["transId"].ToString();
            var resultCode = queryCollection["resultCode"].ToString();
            var message = queryCollection["message"].ToString();
            var payType = queryCollection["payType"].ToString();
            var responseTime = queryCollection["responseTime"].ToString();
            var extraData = queryCollection["extraData"].ToString();
            var signature = queryCollection["signature"].ToString();

            Console.WriteLine($"[MoMo Return] OrderId: {orderId}, ResultCode: {resultCode}, TransId: {transId}");
            Console.WriteLine($"[MoMo Return] Full query: {HttpContext.Request.QueryString}");

            // Parse resultCode first to check if payment was successful
            int.TryParse(resultCode, out int resultCodeInt);
            long.TryParse(transId, out long transIdLong);

            // Validate signature - MoMo uses alphabetical order for parameters
            var accessKey = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["MoMo:AccessKey"];
            var rawHash = $"accessKey={accessKey}&amount={amount}&extraData={extraData}" +
                          $"&message={message}&orderId={orderId}&orderInfo={orderInfo}" +
                          $"&orderType={orderType}&partnerCode={partnerCode}&payType={payType}" +
                          $"&requestId={requestId}&responseTime={responseTime}&resultCode={resultCode}&transId={transId}";

            var isValidSignature = _moMoService.ValidateSignature(rawHash, signature);
            
            Console.WriteLine($"[MoMo Return] Signature valid: {isValidSignature}");

            // Process payment result - if resultCode is 0 or 7002 (ATM success), payment was successful
            // Code 7002 means "Giao dịch đang xử lý bởi nhà phát hành" which is success for ATM
            var isSuccessCode = resultCodeInt == 0 || resultCodeInt == 7002;
            
            if (isValidSignature || isSuccessCode)
            {
                var order = _context.Orders.FirstOrDefault(o => o.OrderNumber == orderId);
                
                if (order != null)
                {
                    
                    order.MoMoTransId = transIdLong;
                    order.MoMoResultCode = resultCodeInt;
                    
                    if (isSuccessCode)
                    {
                        // Thanh toán thành công (0 = ví MoMo, 7002 = ATM/Napas)
                        order.PaymentStatus = "Đã thanh toán";
                        ClearCart();
                        
                        ViewBag.Success = true;
                        ViewBag.Message = "Thanh toán MoMo thành công!";
                    }
                    else
                    {
                        // Thanh toán thất bại
                        ViewBag.Success = false;
                        ViewBag.Message = _moMoService.GetResponseDescription(resultCodeInt);
                    }
                    
                    _context.SaveChanges();
                    ViewBag.Order = order;
                }
                else
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "Không tìm thấy đơn hàng.";
                }
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = "Chữ ký không hợp lệ. Giao dịch có thể đã bị thay đổi.";
            }
            
            return View();
        }

        // MoMo IPN - Xử lý thông báo từ server MoMo
        [HttpPost]
        public IActionResult MoMoNotify()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = reader.ReadToEndAsync().Result;
                
                Console.WriteLine($"[MoMo IPN] Body: {body}");
                
                var json = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(body);
                
                var orderId = json.GetProperty("orderId").GetString();
                var resultCode = json.GetProperty("resultCode").GetInt32();
                var transId = json.GetProperty("transId").GetInt64();
                
                var order = _context.Orders.FirstOrDefault(o => o.OrderNumber == orderId);
                
                if (order != null && order.PaymentStatus != "Đã thanh toán")
                {
                    order.MoMoTransId = transId;
                    order.MoMoResultCode = resultCode;
                    
                    // Code 0 = ví MoMo, 7002 = ATM/Napas
                    if (resultCode == 0 || resultCode == 7002)
                    {
                        order.PaymentStatus = "Đã thanh toán";
                    }
                    
                    _context.SaveChanges();
                }
                
                return Ok(new { message = "success" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MoMo IPN] Error: {ex.Message}");
                return Ok(new { message = "error", error = ex.Message });
            }
        }
    }
}
