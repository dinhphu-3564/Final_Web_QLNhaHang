using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Net;

namespace Web_QLNhaHang.Services
{
    public interface IVNPayService
    {
        string CreatePaymentUrl(decimal amount, string orderInfo, string orderId, string ipAddress);
        bool ValidateSignature(IQueryCollection queryCollection, string inputHash);
        string GetResponseDescription(string responseCode);
    }

    public class VNPayService : IVNPayService
    {
        private readonly IConfiguration _configuration;
        
        public VNPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(decimal amount, string orderInfo, string orderId, string ipAddress)
        {
            var vnp_TmnCode = _configuration["VNPay:vnp_TmnCode"];
            var vnp_HashSecret = _configuration["VNPay:vnp_HashSecret"];
            var vnp_Url = _configuration["VNPay:vnp_Url"];
            var vnp_ReturnUrl = _configuration["VNPay:vnp_ReturnUrl"];

            // Log parameters for debugging
            try 
            {
                var logMsg = $"Creating Payment URL. Amount: {amount}, OrderId: {orderId}, IP: {ipAddress}, Time: {DateTime.Now}\n";
                // File.AppendAllText("vnpay_log.txt", logMsg); 
                // Using console for less permission issues, or specific path if needed.
                Console.WriteLine(logMsg);
            }
            catch {}

            var vnpay = new VnPayLibrary();
            
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode ?? "");
            vnpay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", ipAddress);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", orderInfo);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl ?? "");
            vnpay.AddRequestData("vnp_TxnRef", orderId);

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url ?? "", vnp_HashSecret ?? "");
            
            return paymentUrl;
        }

        public bool ValidateSignature(IQueryCollection queryCollection, string inputHash)
        {
            var vnp_HashSecret = _configuration["VNPay:vnp_HashSecret"];
            var vnpay = new VnPayLibrary();
            
            foreach (var key in queryCollection.Keys)
            {
                if (!string.IsNullOrEmpty(queryCollection[key]) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, queryCollection[key]!);
                }
            }
            
            return vnpay.ValidateSignature(inputHash, vnp_HashSecret ?? "");
        }

        public string GetResponseDescription(string responseCode)
        {
            return responseCode switch
            {
                "00" => "Giao dịch thành công",
                "07" => "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).",
                "09" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng.",
                "10" => "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần",
                "11" => "Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.",
                "12" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa.",
                "13" => "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP).",
                "24" => "Giao dịch không thành công do: Khách hàng hủy giao dịch",
                "51" => "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.",
                "65" => "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày.",
                "75" => "Ngân hàng thanh toán đang bảo trì.",
                "79" => "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định.",
                "99" => "Có lỗi xảy ra trong quá trình xử lý.",
                _ => "Giao dịch thất bại"
            };
        }
    }

    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string GetResponseData(string key)
        {
            return _responseData.TryGetValue(key, out var value) ? value : string.Empty;
        }

        public string CreateRequestUrl(string baseUrl, string vnpHashSecret)
        {
            var queryData = new StringBuilder();
            var hashData = new StringBuilder();
            
            foreach (var kv in _requestData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    // Build hash data first (raw data)
                    hashData.Append(kv.Key + "=" + kv.Value + "&");
                    
                    // Build query data (encoded)
                    // Use Uri.EscapeDataString for RFC 3986 compliance (preferred by VNPay)
                    queryData.Append(Uri.EscapeDataString(kv.Key) + "=" + Uri.EscapeDataString(kv.Value) + "&");
                }
            }

            var queryString = queryData.ToString();
            if (queryString.Length > 0)
            {
                queryString = queryString.Remove(queryString.Length - 1, 1);
            }
            
            var signData = hashData.ToString();
            if (signData.Length > 0)
            {
                signData = signData.Remove(signData.Length - 1, 1);
            }
            
            // Log sign data
            try 
            {
                // File.AppendAllText("vnpay_log.txt", $"SignData: {signData}\n");
                Console.WriteLine($"SignData: {signData}");
            }
            catch {}

            var vnpSecureHash = HmacSha512(vnpHashSecret, signData);
            
            return baseUrl + "?" + queryString + "&vnp_SecureHash=" + vnpSecureHash;
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            var rspRaw = GetResponseRaw();
            var myChecksum = HmacSha512(secretKey, rspRaw);
            
            // Log validation
            try 
            {
                // File.AppendAllText("vnpay_log.txt", $"Validate: Input={inputHash}, MyCalc={myChecksum}, Raw={rspRaw}\n");
                Console.WriteLine($"Validate: Input={inputHash}, MyCalc={myChecksum}, Raw={rspRaw}");
            }
            catch {}

            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetResponseRaw()
        {
            var data = new StringBuilder();
            if (_responseData.ContainsKey("vnp_SecureHashType"))
            {
                _responseData.Remove("vnp_SecureHashType");
            }
            if (_responseData.ContainsKey("vnp_SecureHash"))
            {
                _responseData.Remove("vnp_SecureHash");
            }
            foreach (var kv in _responseData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(kv.Key + "=" + kv.Value + "&");
                }
            }
            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }
            return data.ToString();
        }

        private static string HmacSha512(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }
    }

    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpCompare = CompareInfo.GetCompareInfo("en-US");
            return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
        }
    }
}
