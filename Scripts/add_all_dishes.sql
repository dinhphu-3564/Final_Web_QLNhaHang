-- Script to ensure all dishes from images folder are in database
-- Run this once to add missing dishes

-- First, ensure the category exists
IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = N'Món đặc trưng')
BEGIN
    INSERT INTO Categories (CategoryName, Description, DisplayOrder, IsActive, CreatedDate)
    VALUES (N'Món đặc trưng', N'Các món ăn đặc trưng của nhà hàng', 1, 1, GETDATE());
END

DECLARE @CategoryId INT = (SELECT TOP 1 CategoryId FROM Categories WHERE CategoryName = N'Món đặc trưng');

-- Delete all existing dishes and re-insert with correct data
DELETE FROM Dishes;

-- Insert all 18 dishes matching the images
INSERT INTO Dishes (DishName, Description, Price, ImageUrl, CategoryId, IsAvailable, IsFeatured, DisplayOrder, CreatedDate)
VALUES 
-- 1. Bánh tráng cuốn ba chỉ
(N'Bánh tráng cuốn ba chỉ', 
 N'Bánh tráng mỏng cuốn cùng thịt ba chỉ luộc mềm, rau sống tươi xanh, dưa leo và xoài bào sợi. Món ăn hấp dẫn nhờ sự hòa quyện giữa vị béo của thịt, vị tươi mát của rau và nước mắm chấm đậm đà pha tỏi ớt chuẩn vị miền Trung – Tây Nguyên.', 
 158000, '/images/dishes/banh-trang-ba-chi.jpg', @CategoryId, 1, 1, 1, GETDATE()),

-- 2. Bánh tráng cuốn
(N'Bánh tráng cuốn', 
 N'Bánh tráng cuốn truyền thống với thịt luộc, trứng, rau sống và bún. Khi cuốn chặt tay và chấm cùng mắm nêm hoặc mắm me, món ăn mang đến cảm giác vừa dân dã vừa cuốn hút, rất thích hợp cho những bữa ăn nhẹ.', 
 128000, '/images/dishes/banh-trang-cuon.png', @CategoryId, 1, 1, 2, GETDATE()),

-- 3. Bánh xèo
(N'Bánh xèo', 
 N'Bánh xèo vàng giòn, thơm mùi bột gạo và nước cốt dừa, bên trong là tôm tươi, thịt ba chỉ và giá đỗ. Ăn kèm rau sống đa dạng, cuốn bánh tráng và chấm nước mắm chua ngọt, tạo nên hương vị giòn – béo – tươi mát đặc trưng.', 
 138000, '/images/dishes/banh-xeo.png', @CategoryId, 1, 1, 3, GETDATE()),

-- 4. Bò lá lốt cuốn bánh hỏi
(N'Bò lá lốt cuốn bánh hỏi', 
 N'Thịt bò xay ướp gia vị đậm đà, cuốn lá lốt rồi nướng thơm lừng, ăn kèm bánh hỏi mềm mịn, rau sống và nước mắm pha. Món ăn vừa thơm, vừa béo, rất bắt vị và giàu hương vị truyền thống.', 
 148000, '/images/dishes/bo_la_lot_cuon_banh_hoi.jpg', @CategoryId, 1, 1, 4, GETDATE()),

-- 5. Cá kèo muối ớt
(N'Cá kèo muối ớt', 
 N'Cá kèo tươi được nướng hoặc chiên muối ớt, dậy mùi cay nồng, thịt cá ngọt và săn chắc. Thường ăn kèm rau sống, bánh tráng và muối ớt chanh, rất thích hợp cho những bữa ăn lai rai.', 
 128000, '/images/dishes/ca-keo-muoi-ot.jpg', @CategoryId, 1, 1, 5, GETDATE()),

-- 6. Cá lóc hấp bầu
(N'Cá lóc hấp bầu', 
 N'Cá lóc tươi hấp cùng bầu non, giữ trọn vị ngọt tự nhiên. Món ăn thanh mát, bổ dưỡng, ăn kèm bún, rau sống và nước mắm me hoặc mắm gừng, phù hợp cho bữa cơm gia đình.', 
 448000, '/images/dishes/ca-loc-hap-bau.jpg', @CategoryId, 1, 0, 6, GETDATE()),

-- 7. Cháo bò bằm
(N'Cháo bò bằm', 
 N'Cháo nấu nhuyễn, nóng hổi, kết hợp thịt bò bằm mềm ngọt, rắc thêm hành lá, tiêu và gừng. Món ăn giàu dinh dưỡng, dễ ăn, thích hợp dùng vào buổi sáng hoặc khi cần bồi bổ sức khỏe.', 
 58000, '/images/dishes/chao_bo_bam.jpg', @CategoryId, 1, 0, 7, GETDATE()),

-- 8. Cháo chim bồ câu
(N'Cháo chim bồ câu', 
 N'Cháo nấu từ gạo rang thơm, kết hợp thịt chim bồ câu hầm mềm, giàu dinh dưỡng. Vị cháo ngọt tự nhiên, thường ăn kèm rau răm, tiêu và gừng, rất tốt cho người cần hồi phục thể lực.', 
 88000, '/images/dishes/chao_chim_bo_cau.jpg', @CategoryId, 1, 0, 8, GETDATE()),

-- 9. Cơm chiên cá mặn
(N'Cơm chiên cá mặn', 
 N'Cơm chiên tơi hạt với cá mặn đậm đà, trứng và hành lá, tạo nên hương vị mặn mà, thơm béo. Món ăn đơn giản nhưng cực kỳ đưa cơm, thích hợp cho bữa ăn nhanh.', 
 78000, '/images/dishes/com_chien_ca_man.jpg', @CategoryId, 1, 0, 9, GETDATE()),

-- 10. Cơm chiên hải sản
(N'Cơm chiên hải sản', 
 N'Cơm chiên vàng đều, kết hợp tôm, mực tươi, trứng và rau củ. Hạt cơm săn chắc, đậm vị hải sản, thơm ngon và đầy đủ dinh dưỡng, phù hợp cho mọi lứa tuổi.', 
 98000, '/images/dishes/com_chien_hai_san.jpg', @CategoryId, 1, 0, 10, GETDATE()),

-- 11. Lẩu cá kèo
(N'Lẩu cá kèo', 
 N'Cá kèo tươi bơi trong nồi lẩu nước dùng thanh ngọt, kết hợp rau đồng quê và bún tươi. Món lẩu đặc trưng của vùng sông nước Nam Bộ, thơm ngon và bổ dưỡng.', 
 498000, '/images/dishes/lau-ca-keo.png', @CategoryId, 1, 1, 11, GETDATE()),

-- 12. Lẩu gà lá giang
(N'Lẩu gà lá giang', 
 N'Gà ta thả vườn nấu cùng lá giang tươi, tạo nên vị chua thanh tự nhiên. Nước lẩu thơm ngon, thịt gà dai ngọt, ăn kèm bún và rau sống đa dạng.', 
 398000, '/images/dishes/lau-ga-la-giang.png', @CategoryId, 1, 1, 12, GETDATE()),

-- 13. Lẩu cua sông Cửu Long
(N'Lẩu cua sông Cửu Long', 
 N'Lẩu cua đồng tươi ngon từ sông Cửu Long, nước dùng đậm đà từ gạch cua, rau sống phong phú. Món ăn đặc sản miền Tây mang đậm hương vị quê hương.', 
 458000, '/images/dishes/lau_cua_song_cuu_long.jpg', @CategoryId, 1, 1, 13, GETDATE()),

-- 14. Lẩu mắm U Minh Hạ
(N'Lẩu mắm U Minh Hạ', 
 N'Lẩu mắm đặc sản U Minh Hạ, hương vị đậm đà khó quên của miền Tây sông nước. Nước lẩu thơm nồng, ăn kèm cá lóc, tôm, mực và rau đồng quê.', 
 398000, '/images/dishes/lau_mam_u_minh_ha.jpg', @CategoryId, 1, 1, 14, GETDATE()),

-- 15. Lẩu vịt nấu chao
(N'Lẩu vịt nấu chao', 
 N'Lẩu vịt nấu chao thơm ngon, vịt ta mềm ngọt trong nước lẩu chao đậm đà. Ăn kèm bún, rau muống và các loại rau sống, tạo nên bữa ăn ấm áp.', 
 428000, '/images/dishes/lau_vit_nau_chao.jpg', @CategoryId, 1, 0, 15, GETDATE()),

-- 16. Nem nướng cuốn bánh hỏi
(N'Nem nướng cuốn bánh hỏi', 
 N'Nem nướng thơm phức cuốn bánh hỏi mềm mịn, ăn kèm rau sống và nước chấm đặc biệt. Món ăn vừa thơm vừa béo, mang đậm hương vị Nha Trang.', 
 128000, '/images/dishes/nem_nuong_cuon_banh_hoi.jpg', @CategoryId, 1, 0, 16, GETDATE()),

-- 17. Nộm củ hũ dừa
(N'Nộm củ hũ dừa', 
 N'Củ hũ dừa tươi giòn trộn cùng tôm, thịt và rau thơm, nước sốt chua ngọt đặc biệt. Món nộm thanh mát, giải nhiệt tuyệt vời cho mùa hè.', 
 138000, '/images/dishes/nom-cu-hu-dua.png', @CategoryId, 1, 0, 17, GETDATE()),

-- 18. Vịt nướng chao
(N'Vịt nướng chao', 
 N'Vịt ta nướng giòn thơm, ướp chao đậm đà, thịt mềm ngọt bên trong. Ăn kèm bánh tráng, rau sống và nước chấm me, món ăn hấp dẫn không thể bỏ qua.', 
 368000, '/images/dishes/vit-nuong-chao.png', @CategoryId, 1, 1, 18, GETDATE());

-- Show all dishes after insert
SELECT DishId, DishName, Price, ImageUrl, DisplayOrder, IsFeatured FROM Dishes ORDER BY DisplayOrder;
