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

-- Thêm Bàn
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

-- Thêm FoodCategory
INSERT INTO FoodCategory(name) VALUES (N'Hải sản')
INSERT INTO FoodCategory(name) VALUES (N'Nông sản')
INSERT INTO FoodCategory(name) VALUES (N'Lâm sản')
INSERT INTO FoodCategory(name) VALUES (N'Sản sản')
INSERT INTO FoodCategory(name) VALUES (N'Nước')
-- Thêm món ăn
INSERT INTO Food(name,idCategory,price) VALUES (N'Mực nước sa tế','1','120000')
INSERT INTO Food(name,idCategory,price) VALUES (N'Nghêu hấp xả','1','50000')
INSERT INTO Food(name,idCategory,price) VALUES (N'Dú dê nướng sữa','2','60000')
INSERT INTO Food(name,idCategory,price) VALUES (N'Heo rừng nướng muối ớt','3','76000')
INSERT INTO Food(name,idCategory,price) VALUES (N'Cơm chiên sushi','4','99999')
INSERT INTO Food(name,idCategory,price) VALUES (N'7Up','5','15000')
INSERT INTO Food(name,idCategory,price) VALUES (N'Cafe','5','12000')
--Thêm Bill
INSERT INTO Bill(dateCheckIn,dateCheckOut,idTable,status) VALUES (GETDATE(),NULL,1,0)
INSERT INTO Bill(dateCheckIn,dateCheckOut,idTable,status) VALUES (GETDATE(),NULL,2,0)
INSERT INTO Bill(dateCheckIn,dateCheckOut,idTable,status) VALUES (GETDATE(),GETDATE(),2,1)
--Thêm BillInfo
INSERT INTO BillInfo(idBill,idFood,count) VALUES (1,1,2)
INSERT INTO BillInfo(idBill,idFood,count) VALUES (1,3,4)
INSERT INTO BillInfo(idBill,idFood,count) VALUES (1,5,1)
INSERT INTO BillInfo(idBill,idFood,count) VALUES (2,1,2)
INSERT INTO BillInfo(idBill,idFood,count) VALUES (2,6,2)
INSERT INTO BillInfo(idBill,idFood,count) VALUES (3,5,2)
GO

Create Proc USP_InsertBill
@idTable int
AS
BEGIN
	INSERT INTO Bill(dateCheckIn,dateCheckOut,idTable,status,discount)
	VALUES (GETDATE(), NULL, @idTable, 0,0)
END
GO

Create Proc USP_InsertBillInfo
@idBill int, @idFood int, @count int
AS
BEGIN
	Declare @isExistsBillInfo int = -1;
	Declare @foodCount int = 1;

	Select @isExistsBillInfo=id, @foodCount=count From BillInfo Where idBill=@idBill AND idFood=@idFood

	If(@isExistsBillInfo>0)
	BEGIN
		Declare @newCount int = @foodCount + @count;
		if (@newCount>0)
			UPDATE BillInfo SET count = @newCount Where idBill=@idBill AND idFood=@idFood
		else
			Delete BillInfo Where idBill=@idBill AND idFood=@idFood
	END
	Else 
	Begin
		INSERT INTO BillInfo(idBill,idFood,count)
		VALUES (@idBill,@idFood, @count)
	End	
END
GO

Create trigger UTG_UpdateBillInfo
on BillInfo for Insert,Update
AS
BEGIN
	Declare @idBill int
	Select @idBill=idBill From Inserted

	Declare @idTable int
	Select @idTable=idTable From Bill Where id=@idBill AND status=0

	Declare @count int
	Select @count=COUNT(*) From BillInfo Where idBill=@idBill
	
	If(@count>0)
		Update TableFood Set status=N'Có người' Where id=@idTable
	else 
		Update TableFood Set status=N'Trống' Where id=@idTable
END
GO

Create trigger UTG_UpdateBill
On Bill for Update
AS
BEGIN
	Declare @idBill int
	Select @idBill=id From Inserted

	Declare @idTable int
	Select @idTable=idTable From Bill Where id=@idBill

	Declare @count int = 0
	Select @count=COUNT(*) From Bill Where idTable=@idTable AND status = 0

	If(@count = 0)
		UPDATE TableFood Set status =N'Trống' Where id=@idTable
END
GO

Delete BillInfo
GO
Delete Bill
GO

Alter Table Bill
Add discount int
GO
Update Bill Set discount=0
GO

Create proc USP_SwitchTable
@idTable1 int, @idTable2 int
AS
BEGIN
	Declare @idFirstBill int
	Declare @idSecondBill int
	Declare @isFirstTableEmpty int = 1
	Declare @isSecondTableEmpty int = 1

	Select @idSecondBill=id From Bill Where idTable=@idTable2 AND status=0
	Select @idFirstBill=id From Bill Where idTable=@idTable1 AND status=0

	If(@idFirstBill IS NULL)
	begin
		Insert into Bill(dateCheckIn,dateCheckOut,idTable,status)
		Values(GETDATE(),NULL,@idTable1,0)
		
		Select @idFirstBill=MAX(id) From Bill Where idTable=@idTable1 AND status=0
	end
	Select @isFirstTableEmpty=COUNT(*) From BillInfo Where idBill=@idFirstBill

	If(@idSecondBill IS NULL)
	begin
		Insert into Bill(dateCheckIn,dateCheckOut,idTable,status)
		Values(GETDATE(),NULL,@idTable2,0)
		
		Select @idSecondBill=MAX(id) From Bill Where idTable=@idTable2 AND status=0
	end
	Select @isSecondTableEmpty=COUNT(*) From BillInfo Where idBill=@idSecondBill

	Select id Into IDBillInfoTable From BillInfo Where idBill = @idSecondBill

	Update BillInfo Set idBill=@idSecondBill Where idBill=@idFirstBill

	Update BillInfo Set idBill=@idFirstBill Where id In(Select * from IDBillInfoTable)

	Drop Table IDBillInfoTable
	If(@isFirstTableEmpty = 0)
		UPDATE TableFood Set status=N'Trống' Where id=@idTable2
	If(@isSecondTableEmpty = 0)
		UPDATE TableFood Set status=N'Trống' Where id=@idTable1
END
GO

Alter Table Bill Add totalPrice float
GO

Delete BillInfo
GO
Delete Bill
GO

Create proc USP_GetListBillByDate
@checkIn date, @checkOut date
AS
BEGIN
	Select t.name AS 'Tên bàn',b.totalPrice AS 'Tổng tiền',dateCheckIn AS 'Ngày vào',dateCheckOut AS 'Ngày ra',discount AS 'Giảm giá'
	From Bill AS b, TableFood AS t
	Where dateCheckIn >= @checkIn AND dateCheckOut <= @checkOut AND b.status = 1
		AND t.id = b.idTable
END
GO

Create proc USP_UpdateAccount
@userName nvarchar(100), @displayName nvarchar(100), @password nvarchar(100), @newPassword nvarchar(100)
AS
BEGIN
	Declare @isRightPass int = 0
	Select @isRightPass = COUNT(*) From Account Where @userName=userName AND @password=passWord

	If (@isRightPass = 1)
	begin
		If(@newPassword = null OR @newPassword= '')
			UPDATE Account Set displayName=@displayName Where @userName=userName
		Else
			UPDATE Account Set displayName=@displayName, passWord=@newPassword Where @userName=userName
	end
END
GO

Create trigger UTG_DeleteBillInfo
on BillInfo for delete
AS
BEGIN
	Declare @idBillInfo int
	Declare @idBill int
	Select @idBillInfo=id, @idBill=idBill From Deleted

	Declare @idTable int
	Select @idTable=idTable From Bill Where @idBill=id 
	
	Declare @count int=0
	Select @count=COUNT(*) From BillInfo,Bill Where Bill.id=BillInfo.idBill AND Bill.id=@idBill AND status=0 

	If(@count=0)
		Update TableFood Set status=N'Trống' Where id=@idTable
END
GO

--Function đưa chuỗi có dấu vào, chuyển thành chuỗi không dấu
CREATE FUNCTION [dbo].[fuConvertToUnsign1] ( @strInput NVARCHAR(4000) ) RETURNS NVARCHAR(4000) AS BEGIN IF @strInput IS NULL RETURN @strInput IF @strInput = '' RETURN @strInput DECLARE @RT NVARCHAR(4000) DECLARE @SIGN_CHARS NCHAR(136) DECLARE @UNSIGN_CHARS NCHAR (136) SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệế ìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵý ĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍ ÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ' +NCHAR(272)+ NCHAR(208) SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeee iiiiiooooooooooooooouuuuuuuuuuyyyyy AADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIII OOOOOOOOOOOOOOOUUUUUUUUUUYYYYYDD' DECLARE @COUNTER int DECLARE @COUNTER1 int SET @COUNTER = 1 WHILE (@COUNTER <=LEN(@strInput)) BEGIN SET @COUNTER1 = 1 WHILE (@COUNTER1 <=LEN(@SIGN_CHARS)+1) BEGIN IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1)) = UNICODE(SUBSTRING(@strInput,@COUNTER ,1) ) BEGIN IF @COUNTER=1 SET @strInput = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)-1) ELSE SET @strInput = SUBSTRING(@strInput, 1, @COUNTER-1) +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)- @COUNTER) BREAK END SET @COUNTER1 = @COUNTER1 +1 END SET @COUNTER = @COUNTER +1 END SET @strInput = replace(@strInput,' ','-') RETURN @strInput END
--Select * From Food Where dbo.fuConvertToUnsign1(name) Like N'%'+dbo.fuConvertToUnsign1(N'muc')+N'%'
GO

--1 page 2 dòng
-- pageCount = 2
-- pageNum = 2
Create proc USP_GetListBillByDateAndPage
@checkIn date, @checkOut date, @page int
AS
BEGIN
	Declare @pageRows int = 10
	Declare @selectRows int = @pageRows
	Declare @exceptRows int = (@page-1)*@pageRows

	;With BillShow AS(Select b.id, t.name AS 'Tên bàn',b.totalPrice AS 'Tổng tiền',dateCheckIn AS 'Ngày vào',dateCheckOut AS 'Ngày ra',discount AS 'Giảm giá'
	From Bill AS b, TableFood AS t
	Where dateCheckIn >= @checkIn AND dateCheckOut <= @checkOut AND b.status = 1
		AND t.id = b.idTable)

	Select TOP (@selectRows) * From BillShow Where id NOT IN (Select TOP (@exceptRows) id from BillShow)
END
GO

Create proc USP_GetNumBillByDate
@checkIn date, @checkOut date
AS
BEGIN
	Select COUNT(*)
	From Bill AS b, TableFood AS t
	Where dateCheckIn >= @checkIn AND dateCheckOut <= @checkOut AND b.status = 1
		AND t.id = b.idTable
END
GO

	