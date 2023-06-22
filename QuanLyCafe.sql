CREATE DATABASE QuanLyQuanCafe
GO

USE QuanLyQuanCafe
GO

-- Food
-- Table
-- FoodCategory
-- Account
-- Bill
-- BillInfo

CREATE TABLE TableFood
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Bàn chưa có tên',
	status NVARCHAR(100) NOT NULL DEFAULT N'Trống'	-- NULL || available People 
)
GO

CREATE TABLE Account
(
	userName NVARCHAR(100) PRIMARY KEY,
	displayName NVARCHAR(100) NOT NULL,
	passWord NVARCHAR(1000) NOT NULL DEFAULT 0,	-- thêm dài dài để có gì còn mã hóa password
	type int NOT NULL DEFAULT 0	-- 	1 là admin, 2 là staff 
)
GO

CREATE TABLE FoodCategory
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên',
)
GO

CREATE TABLE Food
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên',
	idCategory INT NOT NULL,
	price FLOAT NOT NULL DEFAULT 0

	FOREIGN KEY (idCategory) REFERENCES dbo.FoodCategory(id)
)
GO

CREATE TABLE Bill
(
	id INT IDENTITY PRIMARY KEY,
	dateCheckIn DATE NOT NULL DEFAULT GETDATE(), 
	dateCheckOut DATE,
	idTable INT NOT NULL,
	status INT NOT NULL DEFAULT 0	-- 1 là đã thanh toán, 0 là chưa thanh toán

	FOREIGN KEY (idTable) REFERENCES dbo.TableFood(id)
)
GO

CREATE TABLE BillInfo
(
	id INT IDENTITY PRIMARY KEY,
	idBill INT NOT NULL,
	idFood INT NOT NULL,
	count INT NOT NULL DEFAULT 0	-- số lượng  
	
	FOREIGN KEY (idBill) REFERENCES dbo.Bill(id),
	FOREIGN KEY (idFood) REFERENCES dbo.Food(id)
)
GO

INSERT INTO Account(userName,displayName,passWord,type)
VALUES (N'admin',N'admin','1','0')
INSERT INTO Account(userName,displayName,passWord,type)
VALUES (N'khanh',N'Hoang Khanh','1','1')
GO

CREATE PROC USP_GetAccountByUserName
@username nvarchar(100)
AS
BEGIN
	Select *
	From Account
	Where @username=userName
END
GO

EXEC USP_GetAccountByUserName @username='admin'
GO

CREATE PROC USP_Login
@userName nvarchar(100), @passWord nvarchar(100)
AS
BEGIN
	Select *
	From Account
	Where userName=@userName AND passWord=@passWord
END
GO

EXEC USP_Login @userName='admin',@passWord='1'
GO

Declare @i int = 0
While @i<=10
BEGIN
	INSERT TableFood(name) VALUES (N'Bàn' + CAST(@i AS nvarchar(100)))
	Set @i = @i + 1
END
GO

CREATE PROC USP_GetTableList
AS
SELECT * FROM TableFood
GO

EXEC USP_GetTableList
GO


Select *from Bill
GO
Select  *from BillInfo

Select *from Food
Select *from FoodCategory