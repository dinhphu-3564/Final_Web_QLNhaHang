-- Script thêm các cột MoMo vào bảng Orders
-- Chạy script này trong SQL Server Management Studio

-- Thêm cột MoMoTransId
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'MoMoTransId')
BEGIN
    ALTER TABLE [dbo].[Orders]
    ADD [MoMoTransId] BIGINT NULL;
    PRINT 'Đã thêm cột MoMoTransId';
END
ELSE
BEGIN
    PRINT 'Cột MoMoTransId đã tồn tại';
END
GO

-- Thêm cột MoMoResultCode
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'MoMoResultCode')
BEGIN
    ALTER TABLE [dbo].[Orders]
    ADD [MoMoResultCode] INT NULL;
    PRINT 'Đã thêm cột MoMoResultCode';
END
ELSE
BEGIN
    PRINT 'Cột MoMoResultCode đã tồn tại';
END
GO

PRINT 'Hoàn thành cập nhật bảng Orders cho MoMo!';
