namespace Web_QLNhaHang.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public int? TableId { get; set; }
        public DateTime BookingDate { get; set; }
        public int NumberOfGuests { get; set; }
        public string Status { get; set; } = "Chờ"; // Chờ, Xác nhận, Hoàn tất, Hủy
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        
        // Navigation properties
        public virtual Customer? Customer { get; set; }
        public virtual Table? Table { get; set; }
    }
}

