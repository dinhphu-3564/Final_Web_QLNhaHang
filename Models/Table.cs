namespace Web_QLNhaHang.Models
{
    public class Table
    {
        public int TableId { get; set; }
        public string TableNumber { get; set; } = string.Empty;
        public int AreaId { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; } = "Trống"; // Trống, Đã đặt, Đang phục vụ, Bảo trì
        public bool IsActive { get; set; }
        
        // Navigation properties
        public virtual TableArea? Area { get; set; }
        public virtual ICollection<Booking>? Bookings { get; set; }
    }
}

