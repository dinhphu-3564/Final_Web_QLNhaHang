namespace Web_QLNhaHang.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = "User"; // Admin, User
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

