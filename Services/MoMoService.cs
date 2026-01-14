using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Web_QLNhaHang.Services
{
    public interface IMoMoService
    {
        Task<string?> CreatePaymentUrl(decimal amount, string orderInfo, string orderId, string paymentType = "captureWallet");
        bool ValidateSignature(string rawData, string inputSignature);
        string GetResponseDescription(int resultCode);
    }

    public class MoMoService : IMoMoService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public MoMoService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// Tạo URL thanh toán MoMo
        /// </summary>
        /// <param name="amount">Số tiền</param>
        /// <param name="orderInfo">Thông tin đơn hàng</param>
        /// <param name="orderId">Mã đơn hàng</param>
        /// <param name="paymentType">Loại thanh toán: captureWallet (ví MoMo) hoặc payWithATM (thẻ ATM)</param>
        public async Task<string?> CreatePaymentUrl(decimal amount, string orderInfo, string orderId, string paymentType = "captureWallet")
        {
            var partnerCode = _configuration["MoMo:PartnerCode"];
            var accessKey = _configuration["MoMo:AccessKey"];
            var secretKey = _configuration["MoMo:SecretKey"];
            var endpoint = _configuration["MoMo:Endpoint"];
            var redirectUrl = _configuration["MoMo:ReturnUrl"];
            var ipnUrl = _configuration["MoMo:IpnUrl"];

            var requestId = Guid.NewGuid().ToString();
            // paymentType: "captureWallet" cho ví MoMo, "payWithATM" cho thẻ ATM
            var requestType = paymentType;
            var extraData = "";

            // Log for debugging
            Console.WriteLine($"[MoMo] Creating payment - OrderId: {orderId}, Amount: {amount}, Type: {paymentType}");

            // Create signature
            // rawSignature = accessKey=$accessKey&amount=$amount&extraData=$extraData
            // &ipnUrl=$ipnUrl&orderId=$orderId&orderInfo=$orderInfo
            // &partnerCode=$partnerCode&redirectUrl=$redirectUrl
            // &requestId=$requestId&requestType=$requestType
            var rawSignature = $"accessKey={accessKey}&amount={(long)amount}&extraData={extraData}" +
                               $"&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}" +
                               $"&partnerCode={partnerCode}&redirectUrl={redirectUrl}" +
                               $"&requestId={requestId}&requestType={requestType}";

            var signature = ComputeHmacSha256(rawSignature, secretKey ?? "");

            Console.WriteLine($"[MoMo] Raw signature: {rawSignature}");
            Console.WriteLine($"[MoMo] Signature: {signature}");

            var requestBody = new
            {
                partnerCode = partnerCode,
                requestType = requestType,
                ipnUrl = ipnUrl,
                redirectUrl = redirectUrl,
                orderId = orderId,
                amount = (long)amount,
                orderInfo = orderInfo,
                requestId = requestId,
                extraData = extraData,
                signature = signature,
                lang = "vi"
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await _httpClient.PostAsync(endpoint, jsonContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[MoMo] Response: {responseContent}");

                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                
                if (jsonResponse.TryGetProperty("resultCode", out var resultCodeElement))
                {
                    var resultCode = resultCodeElement.GetInt32();
                    if (resultCode == 0)
                    {
                        if (jsonResponse.TryGetProperty("payUrl", out var payUrlElement))
                        {
                            return payUrlElement.GetString();
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[MoMo] Error: {GetResponseDescription(resultCode)}");
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MoMo] Exception: {ex.Message}");
                return null;
            }
        }

        public bool ValidateSignature(string rawData, string inputSignature)
        {
            var secretKey = _configuration["MoMo:SecretKey"];
            var computedSignature = ComputeHmacSha256(rawData, secretKey ?? "");
            
            Console.WriteLine($"[MoMo] Validate - Raw: {rawData}");
            Console.WriteLine($"[MoMo] Validate - Input: {inputSignature}, Computed: {computedSignature}");
            
            return computedSignature.Equals(inputSignature, StringComparison.OrdinalIgnoreCase);
        }

        public string GetResponseDescription(int resultCode)
        {
            return resultCode switch
            {
                0 => "Giao dịch thành công",
                9000 => "Giao dịch đã được xác nhận thành công",
                8000 => "Giao dịch đang được xử lý",
                7000 => "Giao dịch đang được xử lý bởi MoMo",
                7002 => "Giao dịch thành công (thẻ ATM/Napas)",
                1000 => "Giao dịch đã được khởi tạo, đang chờ thanh toán",
                11 => "Truy cập bị từ chối",
                12 => "Phiên bản API không được hỗ trợ cho yêu cầu này",
                13 => "Xác thực đối tác không thành công",
                20 => "Yêu cầu sai định dạng",
                21 => "Số tiền giao dịch không hợp lệ",
                22 => "Số tiền vượt quá hạn mức thanh toán",
                40 => "RequestId bị trùng",
                41 => "OrderId bị trùng",
                42 => "OrderId không hợp lệ hoặc không tìm thấy",
                43 => "Yêu cầu bị từ chối vì xung đột trong quá trình xử lý",
                1001 => "Giao dịch thất bại do tài khoản người dùng không đủ tiền",
                1002 => "Giao dịch bị từ chối bởi nhà phát hành",
                1003 => "Giao dịch bị hủy",
                1004 => "Giao dịch thất bại do số tiền vượt quá hạn mức",
                1005 => "Giao dịch thất bại do URL hoặc QR code hết hạn",
                1006 => "Giao dịch thất bại do người dùng từ chối",
                1007 => "Giao dịch bị từ chối do tài khoản không tồn tại",
                1017 => "Giao dịch bị hủy bởi người dùng",
                1026 => "Giao dịch thất bại do giới hạn số lần",
                1080 => "Giao dịch hoàn tiền bị từ chối",
                1081 => "Giao dịch hoàn tiền thất bại",
                2001 => "Giao dịch thất bại",
                2007 => "Giao dịch thất bại do link đã được sử dụng thanh toán",
                3001 => "Liên kết thất bại",
                3002 => "Hủy liên kết thất bại",
                3003 => "Không tìm thấy liên kết",
                3004 => "Liên kết không hoạt động",
                4001 => "Giao dịch bị hạn chế do người dùng chưa hoàn tất xác thực",
                4010 => "OTP đã hết hạn",
                4011 => "OTP không chính xác",
                4100 => "Giao dịch thất bại do người dùng không đăng nhập thành công",
                10 => "Hệ thống đang được bảo trì",
                99 => "Lỗi không xác định",
                _ => $"Lỗi không xác định (Mã: {resultCode})"
            };
        }

        private static string ComputeHmacSha256(string data, string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            
            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);
            
            var hash = new StringBuilder();
            foreach (var b in hashBytes)
            {
                hash.Append(b.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
