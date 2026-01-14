-- =============================================
-- Database: QLNhaHang
-- Mô tả: Database cho hệ thống quản lý nhà hàng
-- =============================================

USE master;
GO

-- Tạo database nếu chưa tồn tại
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'QLNhaHang')
BEGIN
    CREATE DATABASE QLNhaHang;
END
GO

USE QLNhaHang;
GO

-- =============================================
-- Bảng: Danh mục món ăn
-- =============================================
CREATE TABLE Categories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    DisplayOrder INT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- =============================================
-- Bảng: Món ăn
-- =============================================
CREATE TABLE Dishes (
    DishId INT PRIMARY KEY IDENTITY(1,1),
    DishName NVARCHAR(200) NOT NULL,
    CategoryId INT NOT NULL,
    Description NVARCHAR(1000),
    Price DECIMAL(18,2) NOT NULL,
    ImageUrl NVARCHAR(500),
    IsAvailable BIT DEFAULT 1,
    IsFeatured BIT DEFAULT 0,
    DisplayOrder INT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
);
GO

-- =============================================
-- Bảng: Khu vực bàn
-- =============================================
CREATE TABLE TableAreas (
    AreaId INT PRIMARY KEY IDENTITY(1,1),
    AreaName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    IsActive BIT DEFAULT 1
);
GO

-- =============================================
-- Bảng: Bàn
-- =============================================
CREATE TABLE Tables (
    TableId INT PRIMARY KEY IDENTITY(1,1),
    TableNumber NVARCHAR(50) NOT NULL UNIQUE,
    AreaId INT NOT NULL,
    Capacity INT NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Trống', -- Trống, Đã đặt, Đang phục vụ, Bảo trì
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (AreaId) REFERENCES TableAreas(AreaId)
);
GO

-- =============================================
-- Bảng: Khách hàng
-- =============================================
CREATE TABLE Customers (
    CustomerId INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200),
    PhoneNumber NVARCHAR(20),
    Address NVARCHAR(500),
    LoyaltyPoints INT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- =============================================
-- Bảng: Nhân viên
-- =============================================
CREATE TABLE Employees (
    EmployeeId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200),
    PhoneNumber NVARCHAR(20),
    Role NVARCHAR(50) NOT NULL, -- Admin, User
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- =============================================
-- Bảng: Đặt bàn
-- =============================================
CREATE TABLE Bookings (
    BookingId INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT,
    CustomerName NVARCHAR(200) NOT NULL,
    CustomerPhone NVARCHAR(20) NOT NULL,
    CustomerEmail NVARCHAR(200),
    TableId INT,
    BookingDate DATETIME NOT NULL,
    NumberOfGuests INT NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Chờ', -- Chờ, Xác nhận, Hoàn tất, Hủy
    Notes NVARCHAR(1000),
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
    FOREIGN KEY (TableId) REFERENCES Tables(TableId)
);
GO

-- =============================================
-- Bảng: Đơn hàng
-- =============================================
CREATE TABLE Orders (
    OrderId INT PRIMARY KEY IDENTITY(1,1),
    OrderNumber NVARCHAR(50) NOT NULL UNIQUE,
    CustomerId INT,
    CustomerName NVARCHAR(200) NOT NULL,
    CustomerPhone NVARCHAR(20) NOT NULL,
    CustomerEmail NVARCHAR(200),
    CustomerAddress NVARCHAR(500),
    OrderDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) DEFAULT 'Chờ', -- Chờ, Chuẩn bị, Đang giao, Hoàn thành, Hủy
    PaymentStatus NVARCHAR(50) DEFAULT 'Chưa thanh toán', -- Đã thanh toán, Chưa thanh toán
    TotalAmount DECIMAL(18,2) NOT NULL,
    Notes NVARCHAR(1000),
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
);
GO

-- =============================================
-- Bảng: Chi tiết đơn hàng
-- =============================================
CREATE TABLE OrderDetails (
    OrderDetailId INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    DishId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    SubTotal DECIMAL(18,2) NOT NULL,
    Notes NVARCHAR(500), -- Ghi chú: ít cay, không hành...
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE,
    FOREIGN KEY (DishId) REFERENCES Dishes(DishId)
);
GO

-- =============================================
-- Bảng: Đánh giá/Bình luận
-- =============================================
CREATE TABLE Reviews (
    ReviewId INT PRIMARY KEY IDENTITY(1,1),
    DishId INT,
    CustomerId INT,
    CustomerName NVARCHAR(200),
    Rating INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
    Comment NVARCHAR(1000),
    IsApproved BIT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (DishId) REFERENCES Dishes(DishId),
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
);
GO

-- =============================================
-- Bảng: Banner/Khuyến mãi
-- =============================================
CREATE TABLE Promotions (
    PromotionId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000),
    ImageUrl NVARCHAR(500),
    StartDate DATETIME,
    EndDate DATETIME,
    IsActive BIT DEFAULT 1,
    DisplayOrder INT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- =============================================
-- Insert dữ liệu mẫu
-- =============================================

-- Danh mục món ăn
INSERT INTO Categories (CategoryName, Description, DisplayOrder) VALUES
(N'Khai vị', N'Các món khai vị, gỏi, nộm', 1),
(N'Cuốn', N'Bánh tráng cuốn các loại', 2),
(N'Lẩu', N'Các loại lẩu đặc sản', 3),
(N'Nướng', N'Các món nướng', 4),
(N'Món ăn nhẹ', N'Các món ăn vặt, món nhẹ', 5),
(N'Tráng miệng', N'Các món tráng miệng', 6);
GO

-- Khu vực bàn
INSERT INTO TableAreas (AreaName, Description) VALUES
(N'Trong nhà', N'Khu vực trong nhà có điều hòa'),
(N'Ngoài trời', N'Khu vực ngoài trời'),
(N'VIP', N'Phòng VIP riêng biệt');
GO

-- Bàn mẫu
INSERT INTO Tables (TableNumber, AreaId, Capacity, Status) VALUES
('T01', 1, 4, 'Trống'),
('T02', 1, 6, 'Trống'),
('T03', 1, 8, 'Trống'),
('T04', 2, 4, 'Trống'),
('T05', 2, 6, 'Trống'),
('V01', 3, 10, 'Trống'),
('V02', 3, 12, 'Trống');
GO

-- Món ăn mẫu
INSERT INTO Dishes (DishName, CategoryId, Description, Price, IsFeatured, DisplayOrder) VALUES
(N'Cá kèo muối ớt', 4, N'Cá kèo đi máy bay', 128000, 1, 1),
(N'Bánh tráng phơi sương cuốn ba chỉ quay', 2, N'Bánh tráng phơi sương Trảng Bàng cuốn thịt ba chỉ quay', 158000, 1, 2),
(N'Lẩu cá kèo đệ nhất Phương Nam 16 con', 3, N'Cá kèo đang bơi: 16 con, nước dùng đặc biệt', 498000, 1, 3),
(N'Lẩu bò lá giang', 3, N'Thịt bò 0.4kg, lá giang, rau ngổ', 398000, 0, 4),
(N'Bánh xèo cuốn lá cải', 2, N'Bột bánh xèo, nước cốt dừa, tép đồng', 138000, 0, 5);
GO

-- Nhân viên mẫu (password: admin123 - hash này cần được hash thật trong code)
INSERT INTO Employees (Username, PasswordHash, FullName, Email, Role) VALUES
('admin', 'AQAAAAIAAYagAAAAE...', N'Quản trị viên', 'admin@phuongnam.com', 'Admin'),
('user', 'AQAAAAIAAYagAAAAE...', N'Nhân viên', 'user@phuongnam.com', 'User');
GO

-- Khuyến mãi mẫu
INSERT INTO Promotions (Title, Description, IsActive, DisplayOrder, StartDate, EndDate) VALUES
(N'Khuyến mãi đặc biệt', N'Giảm 20% cho đơn hàng trên 500.000đ', 1, 1, GETDATE(), DATEADD(DAY, 30, GETDATE()));
GO

-- Tạo các Index để tối ưu hiệu suất
CREATE INDEX IX_Dishes_CategoryId ON Dishes(CategoryId);
CREATE INDEX IX_Dishes_IsFeatured ON Dishes(IsFeatured);
CREATE INDEX IX_Bookings_BookingDate ON Bookings(BookingDate);
CREATE INDEX IX_Bookings_Status ON Bookings(Status);
CREATE INDEX IX_Orders_OrderDate ON Orders(OrderDate);
CREATE INDEX IX_Orders_Status ON Orders(Status);
CREATE INDEX IX_OrderDetails_OrderId ON OrderDetails(OrderId);
CREATE INDEX IX_Reviews_DishId ON Reviews(DishId);
GO

