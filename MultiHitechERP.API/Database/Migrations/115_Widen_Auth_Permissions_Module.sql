-- Migration 115: Widen Auth_Permissions.Module to hold submenu permission keys
-- Submenu-level permissions store the menu route (e.g. '/masters/customers') as the
-- Module key, so the column must be wider than the original NVARCHAR(50).
-- The UNIQUE (RoleId, Module, Action) constraint references Module, so drop it,
-- alter the column, then re-create it.

IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UQ_Auth_Permissions')
    ALTER TABLE Auth_Permissions DROP CONSTRAINT UQ_Auth_Permissions;

ALTER TABLE Auth_Permissions ALTER COLUMN Module NVARCHAR(100) NOT NULL;

IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UQ_Auth_Permissions')
    ALTER TABLE Auth_Permissions ADD CONSTRAINT UQ_Auth_Permissions UNIQUE (RoleId, Module, Action);

PRINT 'Auth_Permissions.Module widened to NVARCHAR(100).';
