-- Script to update dishes in existing database
-- Run this once to update dish data

UPDATE Dishes SET 
    DishName = N'Bánh tráng cuốn ba chỉ', 
    Description = N'Bánh tráng mỏng cuốn cùng thịt ba chỉ luộc mềm, rau sống tươi xanh, dưa leo và xoài bào sợi. Món ăn hấp dẫn nhờ sự hòa quyện giữa vị béo của thịt, vị tươi mát của rau và nước mắm chấm đậm đà pha tỏi ớt chuẩn vị miền Trung – Tây Nguyên.', 
    ImageUrl = '/images/dishes/banh-trang-ba-chi.jpg', 
    Price = 158000, 
    DisplayOrder = 1,
    IsFeatured = 1
WHERE DishId = 1;

UPDATE Dishes SET 
    DishName = N'Bánh tráng cuốn', 
    Description = N'Bánh tráng cuốn truyền thống với thịt luộc, trứng, rau sống và bún. Khi cuốn chặt tay và chấm cùng mắm nêm hoặc mắm me, món ăn mang đến cảm giác vừa dân dã vừa cuốn hút, rất thích hợp cho những bữa ăn nhẹ.', 
    ImageUrl = '/images/dishes/banh-trang-cuon.png', 
    Price = 128000, 
    DisplayOrder = 2,
    IsFeatured = 1
WHERE DishId = 2;

UPDATE Dishes SET 
    DishName = N'Bánh xèo', 
    Description = N'Bánh xèo vàng giòn, thơm mùi bột gạo và nước cốt dừa, bên trong là tôm tươi, thịt ba chỉ và giá đỗ. Ăn kèm rau sống đa dạng, cuốn bánh tráng và chấm nước mắm chua ngọt, tạo nên hương vị giòn – béo – tươi mát đặc trưng.', 
    ImageUrl = '/images/dishes/banh-xeo.png', 
    Price = 138000, 
    DisplayOrder = 3
WHERE DishId = 3;

UPDATE Dishes SET 
    DishName = N'Bò lá lốt cuốn bánh hỏi', 
    Description = N'Thịt bò xay ướp gia vị đậm đà, cuốn lá lốt rồi nướng thơm lừng, ăn kèm bánh hỏi mềm mịn, rau sống và nước mắm pha. Món ăn vừa thơm, vừa béo, rất bắt vị và giàu hương vị truyền thống.', 
    ImageUrl = '/images/dishes/bo_la_lot_cuon_banh_hoi.jpg', 
    Price = 148000, 
    DisplayOrder = 4,
    IsFeatured = 1
WHERE DishId = 4;

UPDATE Dishes SET 
    DishName = N'Cá kèo muối ớt', 
    Description = N'Cá kèo tươi được nướng hoặc chiên muối ớt, dậy mùi cay nồng, thịt cá ngọt và săn chắc. Thường ăn kèm rau sống, bánh tráng và muối ớt chanh, rất thích hợp cho những bữa ăn lai rai.', 
    ImageUrl = '/images/dishes/ca-keo-muoi-ot.jpg', 
    Price = 128000, 
    DisplayOrder = 5,
    IsFeatured = 1
WHERE DishId = 5;

UPDATE Dishes SET 
    DishName = N'Cá lóc hấp bầu', 
    Description = N'Cá lóc tươi hấp cùng bầu non, giữ trọn vị ngọt tự nhiên. Món ăn thanh mát, bổ dưỡng, ăn kèm bún, rau sống và nước mắm me hoặc mắm gừng, phù hợp cho bữa cơm gia đình.', 
    ImageUrl = '/images/dishes/ca-loc-hap-bau.jpg', 
    Price = 448000, 
    DisplayOrder = 6
WHERE DishId = 6;

UPDATE Dishes SET 
    DishName = N'Cháo bò bằm', 
    Description = N'Cháo nấu nhuyễn, nóng hổi, kết hợp thịt bò bằm mềm ngọt, rắc thêm hành lá, tiêu và gừng. Món ăn giàu dinh dưỡng, dễ ăn, thích hợp dùng vào buổi sáng hoặc khi cần bồi bổ sức khỏe.', 
    ImageUrl = '/images/dishes/chao_bo_bam.jpg', 
    Price = 58000, 
    DisplayOrder = 7
WHERE DishId = 7;

UPDATE Dishes SET 
    DishName = N'Cháo chim bồ câu', 
    Description = N'Cháo nấu từ gạo rang thơm, kết hợp thịt chim bồ câu hầm mềm, giàu dinh dưỡng. Vị cháo ngọt tự nhiên, thường ăn kèm rau răm, tiêu và gừng, rất tốt cho người cần hồi phục thể lực.', 
    ImageUrl = '/images/dishes/chao_chim_bo_cau.jpg', 
    Price = 88000, 
    DisplayOrder = 8
WHERE DishId = 8;

UPDATE Dishes SET 
    DishName = N'Cơm chiên cá mặn', 
    Description = N'Cơm chiên tơi hạt với cá mặn đậm đà, trứng và hành lá, tạo nên hương vị mặn mà, thơm béo. Món ăn đơn giản nhưng cực kỳ đưa cơm, thích hợp cho bữa ăn nhanh.', 
    ImageUrl = '/images/dishes/com_chien_ca_man.jpg', 
    Price = 78000, 
    DisplayOrder = 9
WHERE DishId = 9;

UPDATE Dishes SET 
    DishName = N'Cơm chiên hải sản', 
    Description = N'Cơm chiên vàng đều, kết hợp tôm, mực tươi, trứng và rau củ. Hạt cơm săn chắc, đậm vị hải sản, thơm ngon và đầy đủ dinh dưỡng, phù hợp cho mọi lứa tuổi.', 
    ImageUrl = '/images/dishes/com_chien_hai_san.jpg', 
    Price = 98000, 
    DisplayOrder = 10
WHERE DishId = 10;

-- Show updated dishes
SELECT DishId, DishName, Price, DisplayOrder, IsFeatured FROM Dishes ORDER BY DisplayOrder;
