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
    StatusName NVARCHAR(100) --שם סטטוס--
    );

    --טבלת קונדיטוריות--
CREATE TABLE ConfectioneryTypes (
    ConfectioneryTypeId INT PRIMARY KEY, --מפתח ראשי--
    ConfectioneryTypeName NVARCHAR(100) --שם סוג של קודניטוריה--
    );

    --טבלת סוגי קינוחים--
CREATE TABLE DessertTypes (
    DessertTypeId INT PRIMARY KEY, --מפתח ראשי--
    DessertTypeName NVARCHAR(100) --שם סוג קינוח--
    );

    --טבלת סוגי משתמש--
    CREATE TABLE UserTypes (
    UserTypeId INT PRIMARY KEY, --מפתח ראשי--
    UserTypeName NVARCHAR(100) --שם סוג משתמש--
    );

    --טבלת משתמשים--
CREATE TABLE Users (
    UserId INT PRIMARY KEY Identity,     --מפתח ראשי--
    Mail NVARCHAR(100) Unique,      --מייל של משתמש--
    Username NVARCHAR(100),  --שם משתמש--
    [Password] NVARCHAR(100),    --סיסמה--
    [ProfileName] NVARCHAR(100),   --שם קונה/קונדיטוריה--
     UserTypeId INT,  --מפתח זר לטבלת סוגי משתמש--
    FOREIGN KEY (UserTypeId) REFERENCES UserTypes(UserTypeId),  --סוג משתמש--
    ProfileImage VARBINARY(MAX)  --תמונת פרופיל--
    );

    --טבלת קודניטורים--
CREATE TABLE Bakers (
    BakerId INT PRIMARY KEY,
    FOREIGN KEY (BakerId) REFERENCES Users(UserId), --מפתח ראשי--
    HighestPrice FLOAT, --טווח מחירים--
    ConfectioneryTypeId INT, --מפתח זר לטבלת סוגי קונדיטוריה--
    FOREIGN KEY (ConfectioneryTypeId) REFERENCES ConfectioneryTypes(ConfectioneryTypeId), --סוג קונדיטוריה--
    StatusCode INT, --מפתח זר לטבלת סטטוסים--
    FOREIGN KEY (StatusCode) REFERENCES Statuses(StatusCode), --סטטוס קונדיטוריה--
    Profits FLOAT --רווח--
    );

    --טבלת קינוחים--
CREATE TABLE Desserts (
    DessertId INT PRIMARY KEY Identity, --מפתח ראשי--
    DessertName NVARCHAR(100), --שם קניוח--
    BakerId INT --מפתח זר לטבלת קונדיטורים--
    FOREIGN KEY (BakerId) REFERENCES Bakers(BakerId), --מספר קונדיטור--
    DessertTypeId INT --מפתח זר לטבלת סוגי קינוח--
    FOREIGN KEY (DessertTypeId) REFERENCES DessertTypes(DessertTypeId), --סוג קינוח--
     StatusCode INT --מפתח זר לטבלת סטטוסים--
     FOREIGN KEY (StatusCode) REFERENCES Statuses(StatusCode), --סטטוס קינוח--
    Price FLOAT, --מחיר--
    DessertImage VARBINARY(MAX) --תמונה של קינוח--
    );

    --טבלת הזמנות--
CREATE TABLE Orders (
    OrderId INT PRIMARY KEY Identity, --מפתח ראשי--
    StatusCode INT --מפתח זר לטבלת סטטוסים--
     FOREIGN KEY (StatusCode) REFERENCES Statuses(StatusCode), --סטטוס הזמנה--
     CustomerId INT --מפתח זר לטבלת משתמשים--
    FOREIGN KEY (CustomerId) REFERENCES Users(UserId), --מספר משתמש--
    BakerId INT --מפתח זר לטבלת קונדיטורים--
    FOREIGN KEY (BakerId) REFERENCES Bakers(BakerId), --מספר קונדיטור--
    OrderDate Date, --תאריך הזמנה--
    ArrivalDate Date,  --תאריך הגעה--
    Adress NVARCHAR(100), --כתובת--
    TotalPrice FLOAT --מחיר כל ההזמנה--
    );

    --טבלת קינוחים שהוזמנו--
CREATE TABLE OrderedDesserts (
    OrderId INT --מפתח זר לטבלת הזמנות--
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId), --מספר הזמנה--
    DessertId INT --מפתח זר לטבלת קינוחים--
    FOREIGN KEY (DessertId) REFERENCES Desserts(DessertId), --מספר קינוח--
    CONSTRAINT PK_Orders_Desserts PRIMARY KEY (OrderId,DessertId), --קישור מפתחות זרים--
    StatusCode INT --מפתח זר לטבלת סטטוסים--
    FOREIGN KEY (StatusCode) REFERENCES Statuses(StatusCode), --סטטוס קינוח שהוזמן--
    Quantity INT, --כמות--
    Price FLOAT --מחיר--
    );
    insert into UserTypes values(1,'User')
    insert into UserTypes values(2,'Confectioner')
    insert into UserTypes values(3,'Admin')
    insert into Users (Username,Mail,[Password],UserTypeId,ProfileName) values('Admin','linrattan11@gmail.com','1234',3,'TheAdmin')

    insert into Statuses values(1,'Pending')
    insert into Statuses values(2,'Approved')
    insert into Statuses values(3,'Declined')

    insert into ConfectioneryTypes values(1, 'Bakery')
    insert into ConfectioneryTypes values(2, 'Patisserie')
    insert into ConfectioneryTypes values(3, 'Homemade')
    insert into ConfectioneryTypes values(4, 'Everything')

    insert into DessertTypes values(1,'Cake')
    insert into DessertTypes values(2,'Cupcake')
    insert into DessertTypes values(3,'Cookie')
    insert into DessertTypes values(4,'Pastry')
    

    -- Create a login for the admin user
CREATE LOGIN [AdminLogin] WITH PASSWORD = '12345';
Go

-- Create a user in the DB database for the login
CREATE USER [AdminUser] FOR LOGIN [AdminLogin];
Go

-- Add the user to the db_owner role to grant admin privileges
ALTER ROLE db_owner ADD MEMBER [AdminUser];
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
