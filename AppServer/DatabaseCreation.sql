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

CREATE TABLE Users (
    UserId INT PRIMARY KEY Identity,     --מפתח ראשי--
    Mail NVARCHAR(100),      --מייל של משתמש--
    Username NVARCHAR(100),  --שם משתמש--
    [Password] NVARCHAR(100),    --סיסמה--
    [Name] NVARCHAR(100),   --שם קונה/קונדיטוריה--
    FOREIGN KEY (UserTypeId) REFERENCES UserTypes(UserTypeId),  --סוג משתמש--
    [Image] VARBINARY(MAX)  --תמונת פרופיל--
    );

    CREATE TABLE BAKERS (
    FOREIGN KEY (BakerId) REFERENCES Users(UserId),
    HighestPrice FLOAT,
    FOREIGN KEY (ConfectioneryTypeId) REFERENCES ConfectioneryTypes(ConectioneryTypeId)
    FOREIGN KEY (StatusCode) REFERENCES Statuses(StatusCode),
    Profits FLOAT
    );

    