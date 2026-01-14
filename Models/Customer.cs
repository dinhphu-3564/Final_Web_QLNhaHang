namespace Web_QLNhaHang.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public int LoyaltyPoints { get; set; }
        public DateTime CreatedDate { get; set; }
        
        // Navigation properties
        public virtual ICollection<Booking>? Bookings { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
    }
}

