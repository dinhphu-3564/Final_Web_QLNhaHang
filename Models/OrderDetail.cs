namespace Web_QLNhaHang.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int DishId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public string? Notes { get; set; } // Ghi chú: ít cay, không hành...
        
        // Navigation properties
        public virtual Order? Order { get; set; }
        public virtual Dish? Dish { get; set; }
    }
}

