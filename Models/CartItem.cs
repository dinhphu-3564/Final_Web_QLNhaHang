namespace Web_QLNhaHang.Models
{
    public class CartItem
    {
        public int DishId { get; set; }
        public string DishName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
        public string? Notes { get; set; }
        
        public decimal SubTotal => Price * Quantity;
    }
}

