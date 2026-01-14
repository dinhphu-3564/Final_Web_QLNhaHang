-- =====================================================
-- SQL Script: Insert Sample Data for Restaurant System
-- Dựa trên dữ liệu món ăn từ add_all_dishes.sql
-- =====================================================

-- =====================================================
-- 1. CLEAR EXISTING DATA (Optional - Uncomment if needed)
-- =====================================================
-- DELETE FROM Reviews;
-- DELETE FROM OrderDetails;
-- DELETE FROM Orders;
-- DELETE FROM Bookings;
-- DELETE FROM Dishes;
-- DELETE FROM Categories;
-- DELETE FROM [Tables];
-- DELETE FROM TableAreas;
-- DELETE FROM Customers;
-- DELETE FROM Employees;

-- =====================================================
-- 2. INSERT CATEGORIES (Danh mục) - Dựa trên món ăn
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = N'Món đặc trưng')
BEGIN
    INSERT INTO Categories (CategoryName, Description, DisplayOrder, IsActive, CreatedDate)
    VALUES (N'Món đặc trưng', N'Các món ăn đặc trưng của nhà hàng', 1, 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = N'Lẩu')
BEGIN
    INSERT INTO Categories (CategoryName, Description, DisplayOrder, IsActive, CreatedDate)
    VALUES (N'Lẩu', N'Các món lẩu nóng hổi, đậm đà', 2, 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = N'Món chính')
BEGIN
    INSERT INTO Categories (CategoryName, Description, DisplayOrder, IsActive, CreatedDate)
    VALUES (N'Món chính', N'Các món ăn chính đầy đủ dinh dưỡng', 3, 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = N'Món nước')
BEGIN
    INSERT INTO Categories (CategoryName, Description, DisplayOrder, IsActive, CreatedDate)
    VALUES (N'Món nước', N'Các món canh, soup, cháo', 4, 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = N'Khai vị')
BEGIN
    INSERT INTO Categories (CategoryName, Description, DisplayOrder, IsActive, CreatedDate)
    VALUES (N'Khai vị', N'Các món khai vị, gỏi, nộm', 5, 1, GETDATE());
END

-- =====================================================
-- 3. INSERT EMPLOYEES (Nhân viên/Admin)
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM Employees WHERE Username = 'admin')
BEGIN
    INSERT INTO Employees (Username, PasswordHash, FullName, Email, PhoneNumber, Role, IsActive, CreatedDate)
    VALUES ('admin', 'admin', N'Quản trị viên hệ thống', 'admin@nhahangviet.com', '0901234567', 'Admin', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Employees WHERE Username = 'manager')
BEGIN
    INSERT INTO Employees (Username, PasswordHash, FullName, Email, PhoneNumber, Role, IsActive, CreatedDate)
    VALUES ('manager', 'admin123', N'Quản lý nhà hàng', 'manager@nhahangviet.com', '0902345678', 'Admin', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Employees WHERE Username = 'staff1')
BEGIN
    INSERT INTO Employees (Username, PasswordHash, FullName, Email, PhoneNumber, Role, IsActive, CreatedDate)
    VALUES ('staff1', 'staff123', N'Nguyễn Thị Hoa', 'staff1@nhahangviet.com', '0903456789', 'User', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Employees WHERE Username = 'staff2')
BEGIN
    INSERT INTO Employees (Username, PasswordHash, FullName, Email, PhoneNumber, Role, IsActive, CreatedDate)
    VALUES ('staff2', 'staff123', N'Trần Văn Nam', 'staff2@nhahangviet.com', '0904567890', 'User', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Employees WHERE Username = 'kitchen')
BEGIN
    INSERT INTO Employees (Username, PasswordHash, FullName, Email, PhoneNumber, Role, IsActive, CreatedDate)
    VALUES ('kitchen', 'kitchen123', N'Phạm Văn Bếp', 'kitchen@nhahangviet.com', '0905678901', 'User', 1, GETDATE());
END

-- =====================================================
-- 4. INSERT CUSTOMERS (Khách hàng)
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM Customers WHERE PhoneNumber = '0910111213')
BEGIN
    INSERT INTO Customers (Username, FullName, Email, PhoneNumber, PasswordHash, LoyaltyPoints, CreatedDate)
    VALUES ('0910111213', N'Nguyễn Văn An', 'nguyenvanan@gmail.com', '0910111213', 
            'hash123', 150, DATEADD(DAY, -30, GETDATE()));
END

IF NOT EXISTS (SELECT 1 FROM Customers WHERE PhoneNumber = '0911121314')
BEGIN
    INSERT INTO Customers (Username, FullName, Email, PhoneNumber, PasswordHash, LoyaltyPoints, CreatedDate)
    VALUES ('0911121314', N'Trần Thị Bình', 'tranthibinh@gmail.com', '0911121314', 
            'hash123', 350, DATEADD(DAY, -25, GETDATE()));
END

IF NOT EXISTS (SELECT 1 FROM Customers WHERE PhoneNumber = '0912131415')
BEGIN
    INSERT INTO Customers (Username, FullName, Email, PhoneNumber, PasswordHash, LoyaltyPoints, CreatedDate)
    VALUES ('0912131415', N'Phạm Văn Cường', 'phamvancuong@gmail.com', '0912131415', 
            'hash123', 200, DATEADD(DAY, -20, GETDATE()));
END

IF NOT EXISTS (SELECT 1 FROM Customers WHERE PhoneNumber = '0913141516')
BEGIN
    INSERT INTO Customers (Username, FullName, Email, PhoneNumber, PasswordHash, LoyaltyPoints, CreatedDate)
    VALUES ('0913141516', N'Lê Thị Dung', 'lethidung@gmail.com', '0913141516', 
            'hash123', 50, DATEADD(DAY, -15, GETDATE()));
END

IF NOT EXISTS (SELECT 1 FROM Customers WHERE PhoneNumber = '0914151617')
BEGIN
    INSERT INTO Customers (Username, FullName, Email, PhoneNumber, PasswordHash, LoyaltyPoints, CreatedDate)
    VALUES ('0914151617', N'Hoàng Văn Em', 'hoangvanem@gmail.com', '0914151617', 
            'hash123', 600, DATEADD(DAY, -60, GETDATE()));
END

IF NOT EXISTS (SELECT 1 FROM Customers WHERE PhoneNumber = '0915161718')
BEGIN
    INSERT INTO Customers (Username, FullName, Email, PhoneNumber, PasswordHash, LoyaltyPoints, CreatedDate)
    VALUES ('0915161718', N'Võ Thị Phượng', 'vothiphuong@gmail.com', '0915161718', 
            'hash123', 0, DATEADD(DAY, -5, GETDATE()));
END

-- =====================================================
-- 5. INSERT TABLE AREAS (Khu vực bàn)
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM TableAreas WHERE AreaName = N'Tầng 1 - Trong nhà')
BEGIN
    INSERT INTO TableAreas (AreaName, Description, IsActive)
    VALUES (N'Tầng 1 - Trong nhà', N'Khu vực trong nhà tầng 1, thoáng mát', 1);
END

IF NOT EXISTS (SELECT 1 FROM TableAreas WHERE AreaName = N'Tầng 1 - Phòng riêng')
BEGIN
    INSERT INTO TableAreas (AreaName, Description, IsActive)
    VALUES (N'Tầng 1 - Phòng riêng', N'Phòng riêng cho nhóm lớn, yên tĩnh', 1);
END

IF NOT EXISTS (SELECT 1 FROM TableAreas WHERE AreaName = N'Tầng 2 - VIP')
BEGIN
    INSERT INTO TableAreas (AreaName, Description, IsActive)
    VALUES (N'Tầng 2 - VIP', N'Khu vực VIP cao cấp tầng 2', 1);
END

IF NOT EXISTS (SELECT 1 FROM TableAreas WHERE AreaName = N'Tầng 2 - Thường')
BEGIN
    INSERT INTO TableAreas (AreaName, Description, IsActive)
    VALUES (N'Tầng 2 - Thường', N'Khu vực thường tầng 2', 1);
END

IF NOT EXISTS (SELECT 1 FROM TableAreas WHERE AreaName = N'Ngoài trời')
BEGIN
    INSERT INTO TableAreas (AreaName, Description, IsActive)
    VALUES (N'Ngoài trời', N'Bàn ngoài trời, sân vườn thoáng mát', 1);
END

-- =====================================================
-- 6. INSERT TABLES (Bàn ăn)
-- =====================================================
DECLARE @Area1 INT = (SELECT TOP 1 AreaId FROM TableAreas WHERE AreaName = N'Tầng 1 - Trong nhà');
DECLARE @Area2 INT = (SELECT TOP 1 AreaId FROM TableAreas WHERE AreaName = N'Tầng 1 - Phòng riêng');
DECLARE @Area3 INT = (SELECT TOP 1 AreaId FROM TableAreas WHERE AreaName = N'Tầng 2 - VIP');
DECLARE @Area4 INT = (SELECT TOP 1 AreaId FROM TableAreas WHERE AreaName = N'Tầng 2 - Thường');
DECLARE @Area5 INT = (SELECT TOP 1 AreaId FROM TableAreas WHERE AreaName = N'Ngoài trời');

-- Tầng 1 - Trong nhà
IF NOT EXISTS (SELECT 1 FROM [Tables] WHERE TableNumber = 'T1-01' AND AreaId = @Area1)
    INSERT INTO [Tables] (TableNumber, AreaId, Capacity, Status, IsActive)
    VALUES ('T1-01', @Area1, 2, N'Trống', 1);

IF NOT EXISTS (SELECT 1 FROM [Tables] WHERE TableNumber = 'T1-02' AND AreaId = @Area1)
    INSERT INTO [Tables] (TableNumber, AreaId, Capacity, Status, IsActive)
    VALUES ('T1-02', @Area1, 4, N'Trống', 1);

IF NOT EXISTS (SELECT 1 FROM [Tables] WHERE TableNumber = 'T1-03' AND AreaId = @Area1)
    INSERT INTO [Tables] (TableNumber, AreaId, Capacity, Status, IsActive)
    VALUES ('T1-03', @Area1, 4, N'Đã đặt', 1);

IF NOT EXISTS (SELECT 1 FROM [Tables] WHERE TableNumber = 'T1-04' AND AreaId = @Area1)
    INSERT INTO [Tables] (TableNumber, AreaId, Capacity, Status, IsActive)
    VALUES ('T1-04', @Area1, 6, N'Đang phục vụ', 1);

IF NOT EXISTS (SELECT 1 FROM [Tables] WHERE TableNumber = 'T1-05' AND AreaId = @Area1)
    INSERT INTO [Tables] (TableNumber, AreaId, Capacity, Status, IsActive)
    VALUES ('T1-05', @Area1, 8, N'Trống', 1);

-- Tầng 1 - Phòng riêng
IF NOT EXISTS (SELECT 1 FROM [Tables] WHERE TableNumber = 'P1-01' AND AreaId = @Area2)
    INSERT INTO [Tables] (TableNumber, AreaId, Capacity, Status, IsActive)
    VALUES ('P1-01', @Area2, 10, N'Trống', 1);

IF NOT EXISTS (SELECT 1 FROM [Tables] WHERE TableNumber = 'P1-02' AND AreaId = @Area2)
    INSERT INTO [Tables] (TableNumber, AreaId, Capacity, Status, IsActive)
    VALUES ('P1-02', @Area2, 12, N'Trống', 1);

-- Tầng 2 - VIP
IF NOT EXISTS (SELECT 1 FROM [Tables] WHERE TableNumber = 'VIP-01' AND AreaId = @Area3)
    INSERT INTO [Tables] (TableNumber, AreaId, Capacity, Status, IsActive)
    VALUES ('VIP-01', @Area3, 4, N'Đang phục vụ', 1);

IF NOT EXISTS (SELECT 1 FROM [Tables] WHERE TableNumber = 'VIP-02' AND AreaId = @Area3)
    INSERT INTO [Tables] (TableNumber, AreaId, Capacity, Status, IsActive)
    VALUES ('VIP-02', @Area3, 6, N'Trống', 1);

-- Tầng 2 - Thường
IF NOT EXISTS (SELECT 1 FROM [Tables] WHERE TableNumber = 'T2-01' AND AreaId = @Area4)
    INSERT INTO [Tables] (TableNumber, AreaId, Capacity, Status, IsActive)
    VALUES ('T2-01', @Area4, 2, N'Trống', 1);

IF NOT EXISTS (SELECT 1 FROM [Tables] WHERE TableNumber = 'T2-02' AND AreaId = @Area4)
    INSERT INTO [Tables] (TableNumber, AreaId, Capacity, Status, IsActive)
    VALUES ('T2-02', @Area4, 4, N'Trống', 1);

-- Ngoài trời
IF NOT EXISTS (SELECT 1 FROM [Tables] WHERE TableNumber = 'NT-01' AND AreaId = @Area5)
    INSERT INTO [Tables] (TableNumber, AreaId, Capacity, Status, IsActive)
    VALUES ('NT-01', @Area5, 4, N'Trống', 1);

IF NOT EXISTS (SELECT 1 FROM [Tables] WHERE TableNumber = 'NT-02' AND AreaId = @Area5)
    INSERT INTO [Tables] (TableNumber, AreaId, Capacity, Status, IsActive)
    VALUES ('NT-02', @Area5, 4, N'Trống', 1);

-- =====================================================
-- 7. INSERT BOOKINGS (Đặt bàn) - Dựa trên Customers và Tables
-- =====================================================
DECLARE @Customer1 INT = (SELECT TOP 1 CustomerId FROM Customers WHERE PhoneNumber = '0910111213');
DECLARE @Customer2 INT = (SELECT TOP 1 CustomerId FROM Customers WHERE PhoneNumber = '0911121314');
DECLARE @Customer3 INT = (SELECT TOP 1 CustomerId FROM Customers WHERE PhoneNumber = '0912131415');
DECLARE @Customer4 INT = (SELECT TOP 1 CustomerId FROM Customers WHERE PhoneNumber = '0913141516');
DECLARE @Customer5 INT = (SELECT TOP 1 CustomerId FROM Customers WHERE PhoneNumber = '0914151617');

DECLARE @Table3 INT = (SELECT TOP 1 TableId FROM [Tables] WHERE TableNumber = 'T1-03' AND AreaId = @Area1);
DECLARE @TableVIP1 INT = (SELECT TOP 1 TableId FROM [Tables] WHERE TableNumber = 'VIP-01' AND AreaId = @Area3);

-- Booking đã xác nhận
IF NOT EXISTS (SELECT 1 FROM Bookings WHERE CustomerId = @Customer1 AND BookingDate = DATEADD(HOUR, 2, CAST(GETDATE() AS DATE)))
BEGIN
    INSERT INTO Bookings (CustomerId, CustomerName, CustomerPhone, CustomerEmail, TableId, BookingDate, NumberOfGuests, Status, Notes, CreatedDate)
    VALUES (@Customer1, N'Nguyễn Văn An', '0910111213', 'nguyenvanan@gmail.com', 
            @Table3, DATEADD(HOUR, 2, CAST(GETDATE() AS DATE)), 4, N'Xác nhận', 
            N'Yêu cầu bàn thoáng mát, gần cửa sổ', DATEADD(DAY, -2, GETDATE()));
END

-- Booking đang chờ
IF NOT EXISTS (SELECT 1 FROM Bookings WHERE CustomerId = @Customer2 AND BookingDate = DATEADD(DAY, 1, CAST(GETDATE() AS DATE)))
BEGIN
    INSERT INTO Bookings (CustomerId, CustomerName, CustomerPhone, CustomerEmail, TableId, BookingDate, NumberOfGuests, Status, Notes, CreatedDate)
    VALUES (@Customer2, N'Trần Thị Bình', '0911121314', 'tranthibinh@gmail.com', 
            NULL, DATEADD(DAY, 1, CAST(GETDATE() AS DATE)), 8, N'Chờ', 
            N'Cần bàn yên tĩnh cho buổi họp mặt', DATEADD(DAY, -1, GETDATE()));
END

-- Booking đã hoàn tất
IF NOT EXISTS (SELECT 1 FROM Bookings WHERE CustomerId = @Customer3 AND BookingDate = DATEADD(DAY, -3, GETDATE()))
BEGIN
    INSERT INTO Bookings (CustomerId, CustomerName, CustomerPhone, CustomerEmail, TableId, BookingDate, NumberOfGuests, Status, Notes, CreatedDate)
    VALUES (@Customer3, N'Phạm Văn Cường', '0912131415', 'phamvancuong@gmail.com', 
            @TableVIP1, DATEADD(DAY, -3, GETDATE()), 2, N'Hoàn tất', 
            N'', DATEADD(DAY, -5, GETDATE()));
END

-- Booking cho sinh nhật
IF NOT EXISTS (SELECT 1 FROM Bookings WHERE CustomerId = @Customer4 AND BookingDate = DATEADD(DAY, 2, CAST(GETDATE() AS DATE)))
BEGIN
    INSERT INTO Bookings (CustomerId, CustomerName, CustomerPhone, CustomerEmail, TableId, BookingDate, NumberOfGuests, Status, Notes, CreatedDate)
    VALUES (@Customer4, N'Lê Thị Dung', '0913141516', 'lethidung@gmail.com', 
            NULL, DATEADD(DAY, 2, CAST(GETDATE() AS DATE)), 15, N'Chờ', 
            N'Cần phòng riêng để tổ chức sinh nhật, cần bánh kem', DATEADD(DAY, -3, GETDATE()));
END

-- Booking đã xác nhận
IF NOT EXISTS (SELECT 1 FROM Bookings WHERE CustomerId = @Customer5 AND BookingDate = DATEADD(DAY, 3, CAST(GETDATE() AS DATE)))
BEGIN
    INSERT INTO Bookings (CustomerId, CustomerName, CustomerPhone, CustomerEmail, TableId, BookingDate, NumberOfGuests, Status, Notes, CreatedDate)
    VALUES (@Customer5, N'Hoàng Văn Em', '0914151617', 'hoangvanem@gmail.com', 
            @TableVIP1, DATEADD(DAY, 3, CAST(GETDATE() AS DATE)), 6, N'Xác nhận', 
            N'Yêu cầu thực đơn ăn chay, không có tỏi', DATEADD(DAY, -4, GETDATE()));
END

-- =====================================================
-- 8. INSERT ORDERS (Đơn hàng) - Dựa trên Customers và Dishes
-- =====================================================
DECLARE @Dish1 INT = (SELECT TOP 1 DishId FROM Dishes WHERE DishName = N'Bánh tráng cuốn ba chỉ');
DECLARE @Dish2 INT = (SELECT TOP 1 DishId FROM Dishes WHERE DishName = N'Bánh tráng cuốn');
DECLARE @Dish3 INT = (SELECT TOP 1 DishId FROM Dishes WHERE DishName = N'Bánh xèo');
DECLARE @Dish4 INT = (SELECT TOP 1 DishId FROM Dishes WHERE DishName = N'Bò lá lốt cuốn bánh hỏi');
DECLARE @Dish5 INT = (SELECT TOP 1 DishId FROM Dishes WHERE DishName = N'Cá kèo muối ớt');
DECLARE @Dish11 INT = (SELECT TOP 1 DishId FROM Dishes WHERE DishName = N'Lẩu cá kèo');
DECLARE @Dish12 INT = (SELECT TOP 1 DishId FROM Dishes WHERE DishName = N'Lẩu gà lá giang');
DECLARE @Dish13 INT = (SELECT TOP 1 DishId FROM Dishes WHERE DishName = N'Lẩu cua sông Cửu Long');
DECLARE @Dish18 INT = (SELECT TOP 1 DishId FROM Dishes WHERE DishName = N'Vịt nướng chao');

-- Đơn hàng 1: Đã thanh toán, đã hoàn thành
IF NOT EXISTS (SELECT 1 FROM Orders WHERE OrderNumber = 'ORD20241218001')
BEGIN
    INSERT INTO Orders (OrderNumber, CustomerId, CustomerName, CustomerPhone, CustomerEmail, CustomerAddress, OrderDate, Status, PaymentStatus, PaymentMethod, TotalAmount, Notes)
    VALUES ('ORD20241218001', @Customer1, N'Nguyễn Văn An', '0910111213', 'nguyenvanan@gmail.com', 
            N'123 Đường Lê Lợi, Phường Bến Nghé, Quận 1, TP.HCM', 
            DATEADD(DAY, -5, GETDATE()), N'Hoàn thành', N'Đã thanh toán', 'COD', 
            316000, N'Giao hàng nhanh trong 30 phút');
END

-- Đơn hàng 2: Đã thanh toán VNPay, đang giao
IF NOT EXISTS (SELECT 1 FROM Orders WHERE OrderNumber = 'ORD20241218002')
BEGIN
    INSERT INTO Orders (OrderNumber, CustomerId, CustomerName, CustomerPhone, CustomerEmail, CustomerAddress, OrderDate, Status, PaymentStatus, PaymentMethod, VnpTransactionNo, VnpResponseCode, TotalAmount, Notes)
    VALUES ('ORD20241218002', @Customer2, N'Trần Thị Bình', '0911121314', 'tranthibinh@gmail.com', 
            N'456 Đường Võ Văn Kiệt, Phường Cô Giang, Quận 1, TP.HCM', 
            DATEADD(DAY, -2, GETDATE()), N'Đang giao', N'Đã thanh toán', 'VNPay', 
            'VNP123456789', '00', 784000, N'Giao vào buổi tối');
END

-- Đơn hàng 3: Chưa thanh toán, đang chờ
IF NOT EXISTS (SELECT 1 FROM Orders WHERE OrderNumber = 'ORD20241218003')
BEGIN
    INSERT INTO Orders (OrderNumber, CustomerId, CustomerName, CustomerPhone, CustomerEmail, CustomerAddress, OrderDate, Status, PaymentStatus, PaymentMethod, TotalAmount, Notes)
    VALUES ('ORD20241218003', @Customer3, N'Phạm Văn Cường', '0912131415', 'phamvancuong@gmail.com', 
            N'789 Đường Nguyễn Huệ, Phường Đa Kao, Quận 1, TP.HCM', 
            DATEADD(HOUR, -2, GETDATE()), N'Chờ', N'Chưa thanh toán', 'COD', 
            414000, N'Ít cay, không hành');
END

-- Đơn hàng 4: Đã thanh toán VNPay, đã hoàn thành
IF NOT EXISTS (SELECT 1 FROM Orders WHERE OrderNumber = 'ORD20241218004')
BEGIN
    INSERT INTO Orders (OrderNumber, CustomerId, CustomerName, CustomerPhone, CustomerEmail, CustomerAddress, OrderDate, Status, PaymentStatus, PaymentMethod, VnpTransactionNo, VnpResponseCode, TotalAmount, Notes)
    VALUES ('ORD20241218004', @Customer4, N'Lê Thị Dung', '0913141516', 'lethidung@gmail.com', 
            N'321 Đường Tôn Đức Thắng, Phường Bến Thành, Quận 1, TP.HCM', 
            DATEADD(DAY, -7, GETDATE()), N'Hoàn thành', N'Đã thanh toán', 'VNPay', 
            'VNP987654321', '00', 1226000, N'Ghi chú đặc biệt: không tỏi, không ớt');
END

-- Đơn hàng 5: Đang chuẩn bị, chưa thanh toán
IF NOT EXISTS (SELECT 1 FROM Orders WHERE OrderNumber = 'ORD20241218005')
BEGIN
    INSERT INTO Orders (OrderNumber, CustomerId, CustomerName, CustomerPhone, CustomerEmail, CustomerAddress, OrderDate, Status, PaymentStatus, PaymentMethod, TotalAmount, Notes)
    VALUES ('ORD20241218005', @Customer5, N'Hoàng Văn Em', '0914151617', 'hoangvanem@gmail.com', 
            N'654 Đường Hai Bà Trưng, Phường Đa Kao, Quận 1, TP.HCM', 
            DATEADD(HOUR, -1, GETDATE()), N'Chuẩn bị', N'Chưa thanh toán', 'COD', 
            896000, N'Giao vào giờ trưa');
END

-- Đơn hàng 6: Đã thanh toán, hoàn thành
IF NOT EXISTS (SELECT 1 FROM Orders WHERE OrderNumber = 'ORD20241218006')
BEGIN
    INSERT INTO Orders (OrderNumber, CustomerId, CustomerName, CustomerPhone, CustomerEmail, CustomerAddress, OrderDate, Status, PaymentStatus, PaymentMethod, TotalAmount, Notes)
    VALUES ('ORD20241218006', @Customer1, N'Nguyễn Văn An', '0910111213', 'nguyenvanan@gmail.com', 
            N'123 Đường Lê Lợi, Phường Bến Nghé, Quận 1, TP.HCM', 
            DATEADD(DAY, -1, GETDATE()), N'Hoàn thành', N'Đã thanh toán', 'COD', 
            256000, N'Không cần đũa, chỉ cần muỗng');
END

-- =====================================================
-- 9. INSERT ORDER DETAILS (Chi tiết đơn hàng) - Dựa trên Orders và Dishes
-- =====================================================
DECLARE @Order1 INT = (SELECT TOP 1 OrderId FROM Orders WHERE OrderNumber = 'ORD20241218001');
DECLARE @Order2 INT = (SELECT TOP 1 OrderId FROM Orders WHERE OrderNumber = 'ORD20241218002');
DECLARE @Order3 INT = (SELECT TOP 1 OrderId FROM Orders WHERE OrderNumber = 'ORD20241218003');
DECLARE @Order4 INT = (SELECT TOP 1 OrderId FROM Orders WHERE OrderNumber = 'ORD20241218004');
DECLARE @Order5 INT = (SELECT TOP 1 OrderId FROM Orders WHERE OrderNumber = 'ORD20241218005');
DECLARE @Order6 INT = (SELECT TOP 1 OrderId FROM Orders WHERE OrderNumber = 'ORD20241218006');

-- Đơn hàng 1: Bánh tráng cuốn ba chỉ x2
IF @Order1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM OrderDetails WHERE OrderId = @Order1 AND DishId = @Dish1)
BEGIN
    INSERT INTO OrderDetails (OrderId, DishId, Quantity, UnitPrice, SubTotal, Notes)
    VALUES (@Order1, @Dish1, 2, 158000, 316000, N'Không rau thơm');
END

-- Đơn hàng 2: Bánh xèo x2 + Bò lá lốt x2 + Cá kèo muối ớt x2
IF @Order2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM OrderDetails WHERE OrderId = @Order2 AND DishId = @Dish3)
BEGIN
    INSERT INTO OrderDetails (OrderId, DishId, Quantity, UnitPrice, SubTotal, Notes)
    VALUES 
    (@Order2, @Dish3, 2, 138000, 276000, N'Ít cay'),
    (@Order2, @Dish4, 2, 148000, 296000, N'Thêm lá lốt'),
    (@Order2, @Dish5, 2, 128000, 256000, N'Không ớt, thêm chanh');
END

-- Đơn hàng 3: Bánh tráng cuốn x1 + Bò lá lốt x2
IF @Order3 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM OrderDetails WHERE OrderId = @Order3 AND DishId = @Dish2)
BEGIN
    INSERT INTO OrderDetails (OrderId, DishId, Quantity, UnitPrice, SubTotal, Notes)
    VALUES 
    (@Order3, @Dish2, 1, 128000, 128000, N'Không hành'),
    (@Order3, @Dish4, 2, 148000, 296000, NULL);
END

-- Đơn hàng 4: Lẩu cá kèo x1 + Lẩu gà lá giang x1 + Vịt nướng chao x1
IF @Order4 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM OrderDetails WHERE OrderId = @Order4 AND DishId = @Dish11)
BEGIN
    INSERT INTO OrderDetails (OrderId, DishId, Quantity, UnitPrice, SubTotal, Notes)
    VALUES 
    (@Order4, @Dish11, 1, 498000, 498000, N'Ít cay, thêm rau'),
    (@Order4, @Dish12, 1, 398000, 398000, N'Thêm bún'),
    (@Order4, @Dish18, 1, 368000, 368000, N'Chặt sẵn');
END

-- Đơn hàng 5: Lẩu cua sông Cửu Long x1 + Lẩu gà lá giang x1
IF @Order5 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM OrderDetails WHERE OrderId = @Order5 AND DishId = @Dish13)
BEGIN
    INSERT INTO OrderDetails (OrderId, DishId, Quantity, UnitPrice, SubTotal, Notes)
    VALUES 
    (@Order5, @Dish13, 1, 458000, 458000, N'Thêm gạch cua'),
    (@Order5, @Dish12, 1, 398000, 398000, N'Ít chua');
END

-- Đơn hàng 6: Bánh tráng cuốn ba chỉ x1 + Cá kèo muối ớt x1
IF @Order6 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM OrderDetails WHERE OrderId = @Order6 AND DishId = @Dish1)
BEGIN
    INSERT INTO OrderDetails (OrderId, DishId, Quantity, UnitPrice, SubTotal, Notes)
    VALUES 
    (@Order6, @Dish1, 1, 158000, 158000, NULL),
    (@Order6, @Dish5, 1, 128000, 128000, N'Thêm muối ớt chanh');
END

-- =====================================================
-- 10. INSERT REVIEWS (Đánh giá) - Dựa trên Dishes và Customers
-- =====================================================

-- Review cho Bánh tráng cuốn ba chỉ
IF NOT EXISTS (SELECT 1 FROM Reviews WHERE CustomerId = @Customer1 AND DishId = @Dish1)
BEGIN
    INSERT INTO Reviews (DishId, CustomerId, CustomerName, Rating, Comment, IsApproved, CreatedDate)
    VALUES (@Dish1, @Customer1, N'Nguyễn Văn An', 5, 
            N'Rất ngon! Bánh tráng mềm, thịt ba chỉ luộc mềm vừa phải, rau sống tươi xanh. Nước mắm chấm đậm đà, vừa vị. Sẽ quay lại!', 
            1, DATEADD(DAY, -4, GETDATE()));
END

-- Review cho Bánh xèo
IF NOT EXISTS (SELECT 1 FROM Reviews WHERE CustomerId = @Customer2 AND DishId = @Dish3)
BEGIN
    INSERT INTO Reviews (DishId, CustomerId, CustomerName, Rating, Comment, IsApproved, CreatedDate)
    VALUES (@Dish3, @Customer2, N'Trần Thị Bình', 4, 
            N'Bánh xèo giòn, phần nhân tôm và thịt khá đa dạng. Nhưng hơi nhiều dầu một chút. Nước chấm chua ngọt vừa vặn.', 
            1, DATEADD(DAY, -1, GETDATE()));
END

-- Review cho Lẩu cá kèo
IF NOT EXISTS (SELECT 1 FROM Reviews WHERE CustomerId = @Customer4 AND DishId = @Dish11)
BEGIN
    INSERT INTO Reviews (DishId, CustomerId, CustomerName, Rating, Comment, IsApproved, CreatedDate)
    VALUES (@Dish11, @Customer4, N'Lê Thị Dung', 5, 
            N'Lẩu cá kèo rất ngon! Cá tươi, nước dùng thanh ngọt tự nhiên. Rau đồng quê đa dạng. Món lẩu đậm chất miền Tây!', 
            1, DATEADD(DAY, -6, GETDATE()));
END

-- Review cho Bò lá lốt
IF NOT EXISTS (SELECT 1 FROM Reviews WHERE CustomerId = @Customer3 AND DishId = @Dish4)
BEGIN
    INSERT INTO Reviews (DishId, CustomerId, CustomerName, Rating, Comment, IsApproved, CreatedDate)
    VALUES (@Dish4, @Customer3, N'Phạm Văn Cường', 5, 
            N'Bò lá lốt nướng thơm lừng, bánh hỏi mềm mịn. Món ăn rất bắt vị, đúng hương vị truyền thống. Recommend!', 
            1, DATEADD(HOUR, -1, GETDATE()));
END

-- Review cho Vịt nướng chao
IF NOT EXISTS (SELECT 1 FROM Reviews WHERE CustomerId = @Customer5 AND DishId = @Dish18)
BEGIN
    INSERT INTO Reviews (DishId, CustomerId, CustomerName, Rating, Comment, IsApproved, CreatedDate)
    VALUES (@Dish18, @Customer5, N'Hoàng Văn Em', 4, 
            N'Vịt nướng chao đậm đà, da giòn, thịt mềm. Nước chấm me chua ngọt hợp vị. Món ăn chất lượng!', 
            1, DATEADD(DAY, -7, GETDATE()));
END

-- Review cho Bánh tráng cuốn
IF NOT EXISTS (SELECT 1 FROM Reviews WHERE CustomerId = @Customer2 AND DishId = @Dish2)
BEGIN
    INSERT INTO Reviews (DishId, CustomerId, CustomerName, Rating, Comment, IsApproved, CreatedDate)
    VALUES (@Dish2, @Customer2, N'Trần Thị Bình', 5, 
            N'Bánh tráng cuốn quá tuyệt vời! Thịt luộc mềm, rau sống tươi, nước mắm chấm vừa vặn. Món ăn không thể bỏ qua!', 
            1, DATEADD(DAY, -3, GETDATE()));
END

-- Review cho Lẩu gà lá giang
IF NOT EXISTS (SELECT 1 FROM Reviews WHERE CustomerId = @Customer4 AND DishId = @Dish12)
BEGIN
    INSERT INTO Reviews (DishId, CustomerId, CustomerName, Rating, Comment, IsApproved, CreatedDate)
    VALUES (@Dish12, @Customer4, N'Lê Thị Dung', 5, 
            N'Lẩu gà lá giang vị chua thanh tự nhiên, gà ta dai ngọt. Nước lẩu thơm ngon, ăn rất đưa cơm. Rất thích!', 
            1, DATEADD(DAY, -6, GETDATE()));
END

-- =====================================================
-- VERIFICATION: Đếm số lượng dữ liệu đã thêm
-- =====================================================
DECLARE @EmpCount INT = (SELECT COUNT(*) FROM Employees);
DECLARE @CustCount INT = (SELECT COUNT(*) FROM Customers);
DECLARE @CatCount INT = (SELECT COUNT(*) FROM Categories);
DECLARE @DishCount INT = (SELECT COUNT(*) FROM Dishes);
DECLARE @AreaCount INT = (SELECT COUNT(*) FROM TableAreas);
DECLARE @TableCount INT = (SELECT COUNT(*) FROM [Tables]);
DECLARE @BookingCount INT = (SELECT COUNT(*) FROM Bookings);
DECLARE @OrderCount INT = (SELECT COUNT(*) FROM Orders);
DECLARE @OrderDetailCount INT = (SELECT COUNT(*) FROM OrderDetails);
DECLARE @ReviewCount INT = (SELECT COUNT(*) FROM Reviews);

PRINT '========================================';
PRINT 'TÓNG KẾT DỮ LIỆU ĐÃ THÊM';
PRINT '========================================';
PRINT '';
PRINT 'Nhân viên (Employees): ' + CAST(@EmpCount AS VARCHAR(10));
PRINT 'Khách hàng (Customers): ' + CAST(@CustCount AS VARCHAR(10));
PRINT 'Danh mục (Categories): ' + CAST(@CatCount AS VARCHAR(10));
PRINT 'Món ăn (Dishes): ' + CAST(@DishCount AS VARCHAR(10));
PRINT 'Khu vực bàn (TableAreas): ' + CAST(@AreaCount AS VARCHAR(10));
PRINT 'Bàn ăn (Tables): ' + CAST(@TableCount AS VARCHAR(10));
PRINT 'Đặt bàn (Bookings): ' + CAST(@BookingCount AS VARCHAR(10));
PRINT 'Đơn hàng (Orders): ' + CAST(@OrderCount AS VARCHAR(10));
PRINT 'Chi tiết đơn (OrderDetails): ' + CAST(@OrderDetailCount AS VARCHAR(10));
PRINT 'Đánh giá (Reviews): ' + CAST(@ReviewCount AS VARCHAR(10));
PRINT '';
PRINT '✅ Dữ liệu mẫu đã được thêm thành công!';
PRINT '========================================';
