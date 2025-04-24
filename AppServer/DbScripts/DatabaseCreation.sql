Use master
Go
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'AppServer_DB')
BEGIN 
    DROP DATABASE AppServer_DB;    
END
Go
Create Database AppServer_DB
Go
Use AppServer_DB
Go

    --טבלת סטטוסים--
CREATE TABLE Statuses (
    StatusCode INT PRIMARY KEY, --מפתח ראשי--
    StatusName NVARCHAR(100) NOT NULL --שם סטטוס--
    );

    --טבלת קונדיטוריות--
CREATE TABLE ConfectioneryTypes (
    ConfectioneryTypeId INT PRIMARY KEY, --מפתח ראשי--
    ConfectioneryTypeName NVARCHAR(100) NOT NULL --שם סוג של קודניטוריה--
    );

    --טבלת סוגי קינוחים--
CREATE TABLE DessertTypes (
    DessertTypeId INT PRIMARY KEY, --מפתח ראשי--
    DessertTypeName NVARCHAR(100) NOT NULL --שם סוג קינוח--
    );

    --טבלת סוגי משתמש--
    CREATE TABLE UserTypes (
    UserTypeId INT PRIMARY KEY, --מפתח ראשי--
    UserTypeName NVARCHAR(100) NOT NULL --שם סוג משתמש--
    );

    --טבלת משתמשים--
CREATE TABLE Users (
    UserId INT PRIMARY KEY Identity,     --מפתח ראשי--
    Mail NVARCHAR(100) Unique NOT NULL,      --מייל של משתמש--
    Username NVARCHAR(100) NOT NULL,  --שם משתמש--
    [Password] NVARCHAR(100) NOT NULL,    --סיסמה--
    [ProfileName] NVARCHAR(100) NOT NULL,   --שם פרופיל--
     UserTypeId INT NOT NULL,  --מפתח זר לטבלת סוגי משתמש--
    FOREIGN KEY (UserTypeId) REFERENCES UserTypes(UserTypeId),  --סוג משתמש--
    ProfileImage NVARCHAR(100),  --תמונת פרופיל--
    PhoneNumber NVARCHAR(100) --מספר טלפון--
    );

    --טבלת קודניטורים--
CREATE TABLE Bakers (
    BakerId INT PRIMARY KEY,
    FOREIGN KEY (BakerId) REFERENCES Users(UserId), --מפתח ראשי--
    ConfectioneryName NVARCHAR(100) NOT NULL,
    HighestPrice FLOAT NOT NULL, --טווח מחירים--
    ConfectioneryTypeId INT NOT NULL, --מפתח זר לטבלת סוגי קונדיטוריה--
    FOREIGN KEY (ConfectioneryTypeId) REFERENCES ConfectioneryTypes(ConfectioneryTypeId), --סוג קונדיטוריה--
    StatusCode INT NOT NULL, --מפתח זר לטבלת סטטוסים--
    FOREIGN KEY (StatusCode) REFERENCES Statuses(StatusCode), --סטטוס קונדיטוריה--
    Profits FLOAT --רווח--
    );

    --טבלת קינוחים--
CREATE TABLE Desserts (
    DessertId INT PRIMARY KEY Identity, --מפתח ראשי--
    DessertName NVARCHAR(100) NOT NULL, --שם קניוח--
    BakerId INT --מפתח זר לטבלת קונדיטורים--
    FOREIGN KEY (BakerId) REFERENCES Bakers(BakerId), --מספר קונדיטור--
    DessertTypeId INT --מפתח זר לטבלת סוגי קינוח--
    FOREIGN KEY (DessertTypeId) REFERENCES DessertTypes(DessertTypeId), --סוג קינוח--
     StatusCode INT --מפתח זר לטבלת סטטוסים--
     FOREIGN KEY (StatusCode) REFERENCES Statuses(StatusCode), --סטטוס קינוח--
    Price FLOAT NOT NULL, --מחיר--
    DessertImage NVARCHAR(100) NOT NULL --תמונה של קינוח--
    );

    --טבלת הזמנות--
CREATE TABLE Orders (
    OrderId INT PRIMARY KEY Identity, --מפתח ראשי--
    StatusCode INT --מפתח זר לטבלת סטטוסים--
    FOREIGN KEY (StatusCode) REFERENCES Statuses(StatusCode), --סטטוס הזמנה--
    UserId INT --מפתח זר לטבלת משתמשים--
    FOREIGN KEY (UserId) REFERENCES Users(UserId), --מספר משתמש--
    BakerId INT --מפתח זר לטבלת קונדיטורים--
    FOREIGN KEY (BakerId) REFERENCES Bakers(BakerId), --מספר קונדיטור--
    OrderDate Date, --תאריך הזמנה--
    DispatchDate Date,  --תאריך אישור ושליחה--
    Adress NVARCHAR(100) NOT NULL, --כתובת--
    TotalPrice FLOAT NOT NULL --מחיר כל ההזמנה--
    );

    --טבלת קינוחים שהוזמנו--
CREATE TABLE OrderedDesserts (
    OrderedDessertId INT PRIMARY KEY Identity, --מפתח ראשי מספר הקינוח שהוזמן--
    OrderId INT NULL --מפתח זר לטבלת הזמנות--
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId), --מספר הזמנה--
    DessertId INT --מפתח זר לטבלת קינוחים--
    FOREIGN KEY (DessertId) REFERENCES Desserts(DessertId), --מספר קינוח--
    --CONSTRAINT PK_Orders_Desserts PRIMARY KEY (OrderId,DessertId), --קישור מפתחות זרים--
    StatusCode INT --מפתח זר לטבלת סטטוסים--
    FOREIGN KEY (StatusCode) REFERENCES Statuses(StatusCode), --סטטוס קינוח שהוזמן--
    UserId INT --מפתח זר לטבלת משתמשים--
    FOREIGN KEY (UserId) REFERENCES Users(UserId), --מספר משתמש-- 
    BakerId INT --מפתח זר לטבלת קונדיטורים--
    FOREIGN KEY (BakerId) REFERENCES Bakers(BakerId), --מספר קונדיטור--
    Quantity INT NOT NULL, --כמות--
    Price FLOAT NOT NULL, --מחיר--
    OrderedDessertImage NVARCHAR(100) NOT NULL --תמונה של הקינוח שהוזמן--
    );

    insert into ConfectioneryTypes values(1, 'Bakery')
    insert into ConfectioneryTypes values(2, 'Patisserie')
    insert into ConfectioneryTypes values(3, 'Homemade')
    insert into ConfectioneryTypes values(4, 'Everything')

    insert into UserTypes values(1,'User')
    insert into UserTypes values(2,'Confectioner')
    insert into UserTypes values(3,'Admin')
  

    insert into Statuses values(1,'Pending')
    insert into Statuses values(2,'Approved')
    insert into Statuses values(3,'Declined')

    

    insert into DessertTypes values(1,'Cake')
    insert into DessertTypes values(2,'Cupcake')
    insert into DessertTypes values(3,'Cookie')
    insert into DessertTypes values(4,'Pastry')
    
      insert into Users (Username,Mail,[Password],UserTypeId,ProfileName,PhoneNumber) values('Admin','linrattan11@gmail.com','1234',3,'TheAdmin','0545454540')
    insert into Users (Username,Mail,[Password],UserTypeId,ProfileName,PhoneNumber) values('check','check@gmail.com','check1',2,'check','0526789120')
    insert into Users (Username,Mail,[Password],UserTypeId,ProfileName,PhoneNumber) values('check2','check2@gmail.com','check2',2,'check2','0531234567')
          insert into Users (Username,Mail,[Password],UserTypeId,ProfileName,PhoneNumber) values('test','test@gmail.com','test1',1,'test','0998765430')
          insert into Users (Username,Mail,[Password],UserTypeId,ProfileName,PhoneNumber) values('lol','lol@gmail.com','lol1',1,'lol','0547339924')

    insert into Bakers (BakerId,ConfectioneryName,HighestPrice, ConfectioneryTypeId,StatusCode,Profits) values(2,'Sweet',10,1,2,0)
    insert into Bakers (BakerId,ConfectioneryName,HighestPrice, ConfectioneryTypeId,StatusCode,Profits) values(3,'Sweet2',10,2,2,0)

   

    -- Create a login for the admin user
CREATE LOGIN [AdminLogin] WITH PASSWORD = '12345';
Go

-- Create a user in the DB database for the login
CREATE USER [AdminUser] FOR LOGIN [AdminLogin];
Go

-- Add the user to the db_owner role to grant admin privileges
ALTER ROLE db_owner ADD MEMBER [AdminUser];
Go

--so user can restore the DB!
ALTER SERVER ROLE sysadmin ADD MEMBER [AdminUser];
Go

    --EF Code
/*
scaffold-DbContext "Server = (localdb)\MSSQLLocalDB;Initial Catalog=AppServer_DB;User ID=AdminLogin;Password=12345;" Microsoft.EntityFrameworkCore.SqlServer -OutPutDir Models -Context DBContext -DataAnnotations -force
*/


SELECT * FROM Users
SELECT * FROM Bakers
SELECT * FROM UserTypes
SELECT * FROM ConfectioneryTypes
SELECT * FROM DessertTypes
SELECT * FROM Statuses
SELECT * FROM Desserts
select * from OrderedDesserts
select * from Orders


