-- Migration 111: Create Auth Tables (Users, Roles, Permissions, Sessions)

-- Roles
CREATE TABLE Auth_Roles (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    RoleName    NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255) NULL,
    CreatedAt   DATETIME DEFAULT GETDATE()
);

-- Users
CREATE TABLE Auth_Users (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    FullName     NVARCHAR(100) NOT NULL,
    Username     NVARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    RoleId       INT NULL REFERENCES Auth_Roles(Id),
    IsAdmin      BIT DEFAULT 0,
    IsActive     BIT DEFAULT 1,
    CreatedAt    DATETIME DEFAULT GETDATE()
);

-- Permissions (one row per role + module + action)
CREATE TABLE Auth_Permissions (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    RoleId    INT NOT NULL REFERENCES Auth_Roles(Id) ON DELETE CASCADE,
    Module    NVARCHAR(50) NOT NULL,
    Action    NVARCHAR(50) NOT NULL,
    IsAllowed BIT DEFAULT 0,
    CONSTRAINT UQ_Auth_Permissions UNIQUE (RoleId, Module, Action)
);

-- Sessions
CREATE TABLE Auth_Sessions (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    UserId       INT NOT NULL REFERENCES Auth_Users(Id),
    SessionToken NVARCHAR(255) NOT NULL UNIQUE,
    CreatedAt    DATETIME DEFAULT GETDATE(),
    IsActive     BIT DEFAULT 1
);

-- Seed: Admin role
INSERT INTO Auth_Roles (RoleName, Description)
VALUES ('Admin', 'Full access to everything');

-- NOTE: Admin user is seeded at application startup (Program.cs)
-- so that BCrypt hashing is used properly.
-- Default credentials: admin / admin123
