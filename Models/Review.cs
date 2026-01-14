namespace Web_QLNhaHang.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int? DishId { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedDate { get; set; }
        
        // Navigation properties
        public virtual Dish? Dish { get; set; }
        public virtual Customer? Customer { get; set; }
    }
}

