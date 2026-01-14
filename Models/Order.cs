namespace Web_QLNhaHang.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public string? CustomerAddress { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = "Chờ"; // Chờ, Chuẩn bị, Đang giao, Hoàn thành, Hủy
        public string PaymentStatus { get; set; } = "Chưa thanh toán"; // Đã thanh toán, Chưa thanh toán
        public string PaymentMethod { get; set; } = "COD"; // COD, VNPay, MoMo
        public string? VnpTransactionNo { get; set; }
        public string? VnpResponseCode { get; set; }
        public long? MoMoTransId { get; set; }
        public int? MoMoResultCode { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        
        // Navigation properties
        public virtual Customer? Customer { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}

