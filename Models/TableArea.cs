using System.ComponentModel.DataAnnotations;

namespace Web_QLNhaHang.Models
{
    public class TableArea
    {
        [Key]
        public int AreaId { get; set; }
        public string AreaName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        
        // Navigation property
        public virtual ICollection<Table>? Tables { get; set; }
    }
}

