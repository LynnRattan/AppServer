-- REPLACE YOUR DATABASE LOGIN AND PASSWORD IN THE SCRIPT BELOW 

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

