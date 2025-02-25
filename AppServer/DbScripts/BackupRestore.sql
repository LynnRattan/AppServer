-- REPLACE YOUR DATABASE LOGIN AND PASSWORD IN THE SCRIPT BELOW 

--drop database AppServer_DB
--Go
Use master
Go

-- Create a login for the admin user
CREATE LOGIN  [AdminLogin] WITH PASSWORD = '12345';
Go

-- Create a user in the DB database for the login
CREATE USER [AdminUser] FOR LOGIN [AdminLogin];
Go

--so user can restore the DB!
ALTER SERVER ROLE sysadmin ADD MEMBER [AdminLogin];
Go

Create Database AppServer_DB
go



Use master
Go


--USE master;
--ALTER DATABASE AppServer_DB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
--RESTORE DATABASE AppServer_DB FROM DISK = 'C:\Users\linra\source\repos\AppServer\AppServer\wwwroot\..\DbScripts\backup.bak' WITH REPLACE,
--    MOVE 'AppServer_DB' TO 'C:\Users\linra\AppServer_DB.mdf',   
--    MOVE 'AppServer_DB_log' TO 'C:\Users\linra\AppServer_DB_log.ldf';  
--ALTER DATABASE AppServer_DB SET MULTI_USER;