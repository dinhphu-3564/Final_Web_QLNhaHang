using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace Web_QLNhaHang.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendBookingConfirmationAsync(string customerEmail, string customerName, string bookingId, 
            DateTime bookingDate, TimeSpan bookingTime, int guestCount, string? notes);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderPassword = _configuration["EmailSettings:SenderPassword"];
                var senderName = _configuration["EmailSettings:SenderName"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(senderEmail, senderPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            }
        }

        public async Task SendBookingConfirmationAsync(string customerEmail, string customerName, 
            string bookingId, DateTime bookingDate, TimeSpan bookingTime, int guestCount, string? notes)
        {
            var adminEmail = _configuration["EmailSettings:AdminEmail"] ?? "tuanrx298@gmail.com";

            // Email template cho khách hàng
            var customerHtml = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #4CAF50 0%, #388E3C 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #fff; padding: 30px; border: 1px solid #e0e0e0; }}
        .booking-details {{ background: #FFF8E1; padding: 20px; border-radius: 10px; margin: 20px 0; }}
        .detail-row {{ display: flex; padding: 10px 0; border-bottom: 1px solid #e0e0e0; }}
        .detail-label {{ font-weight: bold; color: #5D4037; width: 150px; }}
        .footer {{ background: #5D4037; color: white; padding: 20px; text-align: center; border-radius: 0 0 10px 10px; }}
        .highlight {{ color: #4CAF50; font-weight: bold; font-size: 24px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🍽️ Nhà Hàng Việt</h1>
            <p>Xác nhận đặt bàn thành công</p>
        </div>
        <div class='content'>
            <p>Xin chào <strong>{customerName}</strong>,</p>
            <p>Cảm ơn bạn đã đặt bàn tại <strong>Nhà Hàng Việt</strong>. Đơn đặt bàn của bạn đã được tiếp nhận thành công!</p>
            
            <div class='booking-details'>
                <h3 style='color: #5D4037; margin-top: 0;'>📋 Chi tiết đặt bàn</h3>
                <p><strong>Mã đặt bàn:</strong> <span class='highlight'>#{bookingId}</span></p>
                <p><strong>Ngày:</strong> {bookingDate:dd/MM/yyyy}</p>
                <p><strong>Giờ:</strong> {bookingTime.Hours:D2}:{bookingTime.Minutes:D2}</p>
                <p><strong>Số khách:</strong> {guestCount} người</p>
                {(string.IsNullOrEmpty(notes) ? "" : $"<p><strong>Ghi chú:</strong> {notes}</p>")}
            </div>
            
            <p>📞 Chúng tôi sẽ liên hệ với bạn để xác nhận đặt bàn trong thời gian sớm nhất.</p>
            <p>Nếu có bất kỳ thay đổi nào, vui lòng liên hệ hotline: <strong>1800 2028</strong> (Miễn phí)</p>
        </div>
        <div class='footer'>
            <p>© 2026 Nhà Hàng Việt - Hương vị Việt Nam chính gốc</p>
            <p>📍 123 Đường ABC, Quận 1, TP. Hồ Chí Minh</p>
        </div>
    </div>
</body>
</html>";

            // Email template cho Admin
            var adminHtml = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #FF9800 0%, #F57C00 100%); color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #fff; padding: 30px; border: 1px solid #e0e0e0; }}
        .booking-details {{ background: #FFF8E1; padding: 20px; border-radius: 10px; margin: 20px 0; }}
        .footer {{ background: #5D4037; color: white; padding: 15px; text-align: center; border-radius: 0 0 10px 10px; font-size: 12px; }}
        .new-badge {{ background: #F44336; color: white; padding: 5px 15px; border-radius: 20px; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <span class='new-badge'>🔔 ĐƠN MỚI</span>
            <h2>Có đơn đặt bàn mới!</h2>
        </div>
        <div class='content'>
            <div class='booking-details'>
                <h3 style='color: #5D4037; margin-top: 0;'>📋 Thông tin đặt bàn #{bookingId}</h3>
                <p><strong>Khách hàng:</strong> {customerName}</p>
                <p><strong>Email:</strong> {customerEmail}</p>
                <p><strong>Ngày:</strong> {bookingDate:dd/MM/yyyy}</p>
                <p><strong>Giờ:</strong> {bookingTime.Hours:D2}:{bookingTime.Minutes:D2}</p>
                <p><strong>Số khách:</strong> {guestCount} người</p>
                {(string.IsNullOrEmpty(notes) ? "" : $"<p><strong>Ghi chú:</strong> {notes}</p>")}
            </div>
            <p style='color: #F57C00;'><strong>⚡ Vui lòng liên hệ khách hàng để xác nhận đặt bàn!</strong></p>
        </div>
        <div class='footer'>
            <p>Email tự động từ hệ thống Nhà Hàng Việt</p>
        </div>
    </div>
</body>
</html>";

            // Gửi email cho khách hàng
            if (!string.IsNullOrEmpty(customerEmail))
            {
                await SendEmailAsync(customerEmail, 
                    $"[Nhà Hàng Việt] Xác nhận đặt bàn #{bookingId}", 
                    customerHtml);
            }

            // Gửi email cho Admin
            await SendEmailAsync(adminEmail, 
                $"🔔 [ĐƠN MỚI] Đặt bàn #{bookingId} - {customerName}", 
                adminHtml);
        }
    }
}
