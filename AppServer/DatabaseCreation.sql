﻿Use master
Go
IF EXISTS (SELECT * FROM sys.databases WHERE name = N'AppServer_DB')
BEGIN
    DROP DATABASE AppServer_DB;
END
Go
Create Database AppServer_DB
Go
Use AppServer_DB
Go

CREATE TABLE Statuses (
    StatusCode INT PRIMARY KEY, --קוד סטטוס--
    StatusName NVARCHAR(100) --שם סטטוס--
    );


CREATE TABLE ConfectioneryTypes (
    ConfectioneryTypeId INT PRIMARY KEY, --מספר סוג של קודניטוריה--
    ConfectioneryTypeName NVARCHAR(100) --שם סוג של קודניטוריה--
    );


CREATE TABLE DessertTypes (
    DessertTypeId INT PRIMARY KEY, --מספר סוג קינוח--
    DessertTypeName NVARCHAR(100) --שם סוג קינוח--
    );


    CREATE TABLE UserTypes (
    UserTypeId INT PRIMARY KEY, --מספר סוג משתמש--
    UserTypeName NVARCHAR(100) --שם סוג משתמש--
    );


CREATE TABLE Users (
    UserId INT PRIMARY KEY Identity,     --מפתח ראשי--
    Mail NVARCHAR(100),      --מייל של משתמש--
    Username NVARCHAR(100),  --שם משתמש--
    [Password] NVARCHAR(100),    --סיסמה--
    [Name] NVARCHAR(100),   --שם קונה/קונדיטוריה--
     UserTypeId INT,  --מפתח זר לטבלת סוגי משתמש--
    FOREIGN KEY (UserTypeId) REFERENCES UserTypes(UserTypeId),  --סוג משתמש--
    ProfileImage VARBINARY(MAX)  --תמונת פרופיל--
    );


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


CREATE TABLE Orders (
    OrderId INT PRIMARY KEY Identity, --מפתח ראשי--
    StatusCode INT --מפתח זר לטבלת סטטוסים--
     FOREIGN KEY (StatusCode) REFERENCES Statuses(StatusCode), --סטטוס הזמנה--
     CustomerId INT --מפתח זר לטבלת משתמשים--
    FOREIGN KEY (CustomerId) REFERENCES Users(UserId), --מספר משתמש--
    BakerId INT --מפתח זר לטבלת קונדיטורים--
    FOREIGN KEY (BakerId) REFERENCES Bakers(BakerId), --מספר קונדיטור--
    OrderDate Date,
    ArrivalDate Date,
    Adress NVARCHAR(100),
    TotalPrice FLOAT
    );


CREATE TABLE OrderedDesserts (
    OrderId INT
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId), --מספר הזמנה--
    DessertId INT --מפתח זר לטבלת קינוחים--
    FOREIGN KEY (DessertId) REFERENCES Desserts(DessertId), --מספר קינוח--
    CONSTRAINT PK_Orders_Desserts PRIMARY KEY (OrderId,DessertId), --קישור מפתחות זרים--
    StatusCode INT --מפתח זר לטבלת סטטוסים--
    FOREIGN KEY (StatusCode) REFERENCES Statuses(StatusCode), --סטטוס קינוח שהוזמן--
    Quantity INT, --כמות--
    Price FLOAT --מחיר--
    );

    




    


    