using Microsoft.EntityFrameworkCore;
using Web_QLNhaHang.Models;

namespace Web_QLNhaHang.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Add missing columns if they don't exist (for database migration)
            try
            {
                context.Database.ExecuteSqlRaw(@"
                    IF COL_LENGTH('Customers', 'Username') IS NULL 
                    ALTER TABLE Customers ADD Username NVARCHAR(100) NULL;
                    IF COL_LENGTH('Customers', 'PasswordHash') IS NULL 
                    ALTER TABLE Customers ADD PasswordHash NVARCHAR(255) NULL;
                ");
            }
            catch { /* Ignore if columns already exist */ }

            try
            {
                context.Database.ExecuteSqlRaw(@"
                    IF COL_LENGTH('Dishes', 'ImageFileName') IS NULL 
                    ALTER TABLE Dishes ADD ImageFileName NVARCHAR(255) NULL;
                ");
            }
            catch { /* Ignore if column already exists */ }

            try
            {
                context.Database.ExecuteSqlRaw(@"
                    IF COL_LENGTH('Orders', 'PaymentMethod') IS NULL 
                    ALTER TABLE Orders ADD PaymentMethod NVARCHAR(50) DEFAULT 'COD';
                    IF COL_LENGTH('Orders', 'VnpTransactionNo') IS NULL 
                    ALTER TABLE Orders ADD VnpTransactionNo NVARCHAR(100) NULL;
                    IF COL_LENGTH('Orders', 'VnpResponseCode') IS NULL 
                    ALTER TABLE Orders ADD VnpResponseCode NVARCHAR(50) NULL;
                ");
            }
            catch { /* Ignore if columns already exist */ }

            // Only seed data if database is empty - DO NOT overwrite existing data
            if (!context.Categories.Any())
            {
                var category = new Category 
                { 
                    CategoryName = "Món đặc trưng", 
                    DisplayOrder = 1, 
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };
                context.Categories.Add(category);
                context.SaveChanges();
            }

            // Only seed dishes if none exist
            if (!context.Dishes.Any())
            {
                var category = context.Categories.FirstOrDefault(c => c.CategoryName == "Món đặc trưng");
                if (category == null) return;

                var dishes = new Dish[]
                {
                    new Dish { DishName = "Bánh tráng cuốn ba chỉ", Price = 158000, Description = "Bánh tráng mỏng cuốn cùng thịt ba chỉ luộc mềm, rau sống tươi xanh, dưa leo và xoài bào sợi. Món ăn hấp dẫn nhờ sự hòa quyện giữa vị béo của thịt, vị tươi mát của rau và nước mắm chấm đậm đà pha tỏi ớt chuẩn vị miền Trung – Tây Nguyên.", ImageUrl = "/images/dishes/banh-trang-ba-chi.jpg", CategoryId = category.CategoryId, IsAvailable = true, IsFeatured = true, DisplayOrder = 1 },
                    new Dish { DishName = "Bánh tráng cuốn", Price = 128000, Description = "Bánh tráng cuốn truyền thống với thịt luộc, trứng, rau sống và bún. Khi cuốn chặt tay và chấm cùng mắm nêm hoặc mắm me, món ăn mang đến cảm giác vừa dân dã vừa cuốn hút, rất thích hợp cho những bữa ăn nhẹ.", ImageUrl = "/images/dishes/banh-trang-cuon.png", CategoryId = category.CategoryId, IsAvailable = true, IsFeatured = true, DisplayOrder = 2 },
                    new Dish { DishName = "Bánh xèo", Price = 138000, Description = "Bánh xèo vàng giòn, thơm mùi bột gạo và nước cốt dừa, bên trong là tôm tươi, thịt ba chỉ và giá đỗ. Ăn kèm rau sống đa dạng, cuốn bánh tráng và chấm nước mắm chua ngọt, tạo nên hương vị giòn – béo – tươi mát đặc trưng.", ImageUrl = "/images/dishes/banh-xeo.png", CategoryId = category.CategoryId, IsAvailable = true, DisplayOrder = 3 },
                    new Dish { DishName = "Bò lá lốt cuốn bánh hỏi", Price = 148000, Description = "Thịt bò xay ướp gia vị đậm đà, cuốn lá lốt rồi nướng thơm lừng, ăn kèm bánh hỏi mềm mịn, rau sống và nước mắm pha. Món ăn vừa thơm, vừa béo, rất bắt vị và giàu hương vị truyền thống.", ImageUrl = "/images/dishes/bo_la_lot_cuon_banh_hoi.jpg", CategoryId = category.CategoryId, IsAvailable = true, IsFeatured = true, DisplayOrder = 4 },
                    new Dish { DishName = "Cá kèo muối ớt", Price = 128000, Description = "Cá kèo tươi được nướng hoặc chiên muối ớt, dậy mùi cay nồng, thịt cá ngọt và săn chắc. Thường ăn kèm rau sống, bánh tráng và muối ớt chanh, rất thích hợp cho những bữa ăn lai rai.", ImageUrl = "/images/dishes/ca-keo-muoi-ot.jpg", CategoryId = category.CategoryId, IsAvailable = true, IsFeatured = true, DisplayOrder = 5 },
                    new Dish { DishName = "Cá lóc hấp bầu", Price = 448000, Description = "Cá lóc tươi hấp cùng bầu non, giữ trọn vị ngọt tự nhiên. Món ăn thanh mát, bổ dưỡng, ăn kèm bún, rau sống và nước mắm me hoặc mắm gừng, phù hợp cho bữa cơm gia đình.", ImageUrl = "/images/dishes/ca-loc-hap-bau.jpg", CategoryId = category.CategoryId, IsAvailable = true, DisplayOrder = 6 },
                    new Dish { DishName = "Cháo bò bằm", Price = 58000, Description = "Cháo nấu nhuyễn, nóng hổi, kết hợp thịt bò bằm mềm ngọt, rắc thêm hành lá, tiêu và gừng. Món ăn giàu dinh dưỡng, dễ ăn, thích hợp dùng vào buổi sáng hoặc khi cần bồi bổ sức khỏe.", ImageUrl = "/images/dishes/chao_bo_bam.jpg", CategoryId = category.CategoryId, IsAvailable = true, DisplayOrder = 7 },
                    new Dish { DishName = "Cháo chim bồ câu", Price = 88000, Description = "Cháo nấu từ gạo rang thơm, kết hợp thịt chim bồ câu hầm mềm, giàu dinh dưỡng. Vị cháo ngọt tự nhiên, thường ăn kèm rau răm, tiêu và gừng, rất tốt cho người cần hồi phục thể lực.", ImageUrl = "/images/dishes/chao_chim_bo_cau.jpg", CategoryId = category.CategoryId, IsAvailable = true, DisplayOrder = 8 },
                    new Dish { DishName = "Cơm chiên cá mặn", Price = 78000, Description = "Cơm chiên tơi hạt với cá mặn đậm đà, trứng và hành lá, tạo nên hương vị mặn mà, thơm béo. Món ăn đơn giản nhưng cực kỳ đưa cơm, thích hợp cho bữa ăn nhanh.", ImageUrl = "/images/dishes/com_chien_ca_man.jpg", CategoryId = category.CategoryId, IsAvailable = true, DisplayOrder = 9 },
                    new Dish { DishName = "Cơm chiên hải sản", Price = 98000, Description = "Cơm chiên vàng đều, kết hợp tôm, mực tươi, trứng và rau củ. Hạt cơm săn chắc, đậm vị hải sản, thơm ngon và đầy đủ dinh dưỡng, phù hợp cho mọi lứa tuổi.", ImageUrl = "/images/dishes/com_chien_hai_san.jpg", CategoryId = category.CategoryId, IsAvailable = true, DisplayOrder = 10 },
                    new Dish { DishName = "Lẩu cua sông Cửu Long", Price = 458000, Description = "Lẩu cua đồng tươi ngon từ sông Cửu Long, nước dùng đậm đà, rau sống phong phú.", ImageUrl = "/images/dishes/lau_cua_song_cuu_long.jpg", CategoryId = category.CategoryId, IsAvailable = true, IsFeatured = true, DisplayOrder = 11 },
                    new Dish { DishName = "Lẩu mắm U Minh Hạ", Price = 398000, Description = "Lẩu mắm đặc sản U Minh Hạ, hương vị đậm đà khó quên của miền Tây sông nước.", ImageUrl = "/images/dishes/lau_mam_u_minh_ha.jpg", CategoryId = category.CategoryId, IsAvailable = true, IsFeatured = true, DisplayOrder = 12 },
                    new Dish { DishName = "Lẩu vịt nấu chao", Price = 428000, Description = "Lẩu vịt nấu chao thơm ngon, vịt ta mềm ngọt trong nước lẩu chao đậm đà.", ImageUrl = "/images/dishes/lau_vit_nau_chao.jpg", CategoryId = category.CategoryId, IsAvailable = true, DisplayOrder = 13 },
                    new Dish { DishName = "Nem nướng cuốn bánh hỏi", Price = 128000, Description = "Nem nướng thơm phức cuốn bánh hỏi mềm mịn, ăn kèm rau sống và nước chấm đặc biệt.", ImageUrl = "/images/dishes/nem_nuong_cuon_banh_hoi.jpg", CategoryId = category.CategoryId, IsAvailable = true, DisplayOrder = 14 }
                };

                context.Dishes.AddRange(dishes);
                context.SaveChanges();
            }
            
            // Seed admin employee only if not exists
            if (!context.Employees.Any(e => e.Username == "admin"))
            {
                var adminEmployee = new Employee
                {
                    Username = "admin",
                    PasswordHash = "admin",
                    FullName = "Quản trị viên",
                    Email = "admin@nhahangviet.com",
                    PhoneNumber = "0901234567",
                    Role = "Admin",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };
                context.Employees.Add(adminEmployee);
                context.SaveChanges();
            }

            // Seed test customer only if not exists
            if (!context.Customers.Any(c => c.Email == "test@gmail.com"))
            {
                using var sha256 = System.Security.Cryptography.SHA256.Create();
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes("123"));
                var passwordHash = Convert.ToBase64String(hashedBytes);

                var testCustomer = new Customer
                {
                    Username = "test@gmail.com",
                    FullName = "Khách hàng Test",
                    Email = "test@gmail.com",
                    PhoneNumber = "0987654321",
                    PasswordHash = passwordHash,
                    LoyaltyPoints = 0,
                    CreatedDate = DateTime.Now
                };
                context.Customers.Add(testCustomer);
                context.SaveChanges();
            }
        }
    }
}
