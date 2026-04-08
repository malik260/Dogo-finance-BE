-- 1. Create TBL_MODULE
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TBL_MODULE]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TBL_MODULE](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Icon] [varchar](50) NOT NULL,
	[Description] [varchar](250) NULL,
 CONSTRAINT [PK_TBL_MODULE] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
END
GO

-- 2. Create TBL_ACCESS_RIGHT
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TBL_ACCESS_RIGHT]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TBL_ACCESS_RIGHT](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ModuleId] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL, -- Logical name (e.g., ViewClients)
	[Label] [varchar](100) NOT NULL, -- UI Display Name (e.g., View Clients)
 CONSTRAINT [PK_TBL_ACCESS_RIGHT] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
END
GO

-- 3. Create TBL_ROLE_ACCESS_RIGHT (Mapping)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TBL_ROLE_ACCESS_RIGHT]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TBL_ROLE_ACCESS_RIGHT](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NOT NULL,
	[AccessRightId] [int] NOT NULL,
 CONSTRAINT [PK_TBL_ROLE_ACCESS_RIGHT] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
END
GO

-- Add Foreign Keys
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ACCESS_RIGHT_MODULE]'))
ALTER TABLE [dbo].[TBL_ACCESS_RIGHT] WITH CHECK ADD CONSTRAINT [FK_ACCESS_RIGHT_MODULE] FOREIGN KEY([ModuleId]) REFERENCES [dbo].[TBL_MODULE] ([Id])
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ROLE_ACCESS_RIGHT_ROLE]'))
ALTER TABLE [dbo].[TBL_ROLE_ACCESS_RIGHT] WITH CHECK ADD CONSTRAINT [FK_ROLE_ACCESS_RIGHT_ROLE] FOREIGN KEY([RoleId]) REFERENCES [dbo].[TBL_ROLE] ([Id])
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ROLE_ACCESS_RIGHT_ACCESS]'))
ALTER TABLE [dbo].[TBL_ROLE_ACCESS_RIGHT] WITH CHECK ADD CONSTRAINT [FK_ROLE_ACCESS_RIGHT_ACCESS] FOREIGN KEY([AccessRightId]) REFERENCES [dbo].[TBL_ACCESS_RIGHT] ([Id])
GO

-- Seed Data (Based on Frontend Mockup)
SET IDENTITY_INSERT [dbo].[TBL_MODULE] ON
INSERT INTO [dbo].[TBL_MODULE] (Id, Name, Icon, Description) VALUES (1, 'User Management', 'ri-group-line', 'Manage client accounts and verifications.')
INSERT INTO [dbo].[TBL_MODULE] (Id, Name, Icon, Description) VALUES (2, 'Investments', 'ri-building-line', 'Manage Shariah-compliant asset pools.')
INSERT INTO [dbo].[TBL_MODULE] (Id, Name, Icon, Description) VALUES (3, 'Transactions', 'ri-money-dollar-circle-line', 'Manage deposits and withdrawals.')
INSERT INTO [dbo].[TBL_MODULE] (Id, Name, Icon, Description) VALUES (4, 'Compliance', 'ri-scale-3-line', 'Audit logs and certifications.')
INSERT INTO [dbo].[TBL_MODULE] (Id, Name, Icon, Description) VALUES (5, 'Role & Access Matrix', 'ri-shield-keyhole-line', 'Manage roles and system privileges.')
SET IDENTITY_INSERT [dbo].[TBL_MODULE] OFF
GO

SET IDENTITY_INSERT [dbo].[TBL_ACCESS_RIGHT] ON
-- User Management
INSERT INTO [dbo].[TBL_ACCESS_RIGHT] (Id, ModuleId, Name, Label) VALUES (1, 1, 'ViewClients', 'View Clients')
INSERT INTO [dbo].[TBL_ACCESS_RIGHT] (Id, ModuleId, Name, Label) VALUES (2, 1, 'EditProfiles', 'Edit Profiles')
INSERT INTO [dbo].[TBL_ACCESS_RIGHT] (Id, ModuleId, Name, Label) VALUES (3, 1, 'SuspendAccounts', 'Suspend Accounts')
INSERT INTO [dbo].[TBL_ACCESS_RIGHT] (Id, ModuleId, Name, Label) VALUES (4, 1, 'ApproveVerification', 'Approve BVN/NIN')
-- Investments
INSERT INTO [dbo].[TBL_ACCESS_RIGHT] (Id, ModuleId, Name, Label) VALUES (5, 2, 'ViewPools', 'View Pools')
INSERT INTO [dbo].[TBL_ACCESS_RIGHT] (Id, ModuleId, Name, Label) VALUES (6, 2, 'CreatePool', 'Create Pool')
INSERT INTO [dbo].[TBL_ACCESS_RIGHT] (Id, ModuleId, Name, Label) VALUES (7, 2, 'CalculateYields', 'Calculate Yields')
-- Transactions
INSERT INTO [dbo].[TBL_ACCESS_RIGHT] (Id, ModuleId, Name, Label) VALUES (8, 3, 'ViewDeposits', 'View Deposits')
INSERT INTO [dbo].[TBL_ACCESS_RIGHT] (Id, ModuleId, Name, Label) VALUES (9, 3, 'ApproveWithdrawals', 'Approve Payouts')
-- Roles
INSERT INTO [dbo].[TBL_ACCESS_RIGHT] (Id, ModuleId, Name, Label) VALUES (10, 5, 'ViewRoles', 'View Roles')
INSERT INTO [dbo].[TBL_ACCESS_RIGHT] (Id, ModuleId, Name, Label) VALUES (11, 5, 'ManageAccessRights', 'Edit Access Matrix')
SET IDENTITY_INSERT [dbo].[TBL_ACCESS_RIGHT] OFF
GO

-- Grant SuperAdmin (ID 1) all access rights initially
INSERT INTO [dbo].[TBL_ROLE_ACCESS_RIGHT] (RoleId, AccessRightId)
SELECT 1, Id FROM [dbo].[TBL_ACCESS_RIGHT]
GO
