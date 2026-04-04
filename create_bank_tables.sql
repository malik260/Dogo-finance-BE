-- Create TBL_BANK table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TBL_BANK]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TBL_BANK](
	[BankId] [int] IDENTITY(1,1) NOT NULL,
	[BankName] [varchar](100) NOT NULL,
	[BankCode] [varchar](10) NOT NULL,
	[LogoUrl] [varchar](200) NULL,
	[IsActive] [bit] NOT NULL DEFAULT (1),
	[CreatedAt] [datetime] NOT NULL DEFAULT (GETDATE()),
 CONSTRAINT [PK_TBL_BANK] PRIMARY KEY CLUSTERED 
(
	[BankId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

-- Create TBL_CUSTOMER_BANK table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TBL_CUSTOMER_BANK]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TBL_CUSTOMER_BANK](
	[CustomerBankId] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[BankId] [int] NOT NULL,
	[AccountNumber] [varchar](20) NOT NULL,
	[AccountName] [varchar](100) NOT NULL,
	[IsDefault] [bit] NOT NULL DEFAULT (0),
	[CreatedAt] [datetime] NOT NULL DEFAULT (GETDATE()),
	[IsDeleted] [bit] NOT NULL DEFAULT (0),
 CONSTRAINT [PK_TBL_CUSTOMER_BANK] PRIMARY KEY CLUSTERED 
(
	[CustomerBankId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

-- Add Foreign Keys
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CUSTOMER_BANK_BANK]') AND parent_object_id = OBJECT_ID(N'[dbo].[TBL_CUSTOMER_BANK]'))
ALTER TABLE [dbo].[TBL_CUSTOMER_BANK]  WITH CHECK ADD  CONSTRAINT [FK_CUSTOMER_BANK_BANK] FOREIGN KEY([BankId])
REFERENCES [dbo].[TBL_BANK] ([BankId])
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CUSTOMER_BANK_CUSTOMER]') AND parent_object_id = OBJECT_ID(N'[dbo].[TBL_CUSTOMER_BANK]'))
ALTER TABLE [dbo].[TBL_CUSTOMER_BANK]  WITH CHECK ADD  CONSTRAINT [FK_CUSTOMER_BANK_CUSTOMER] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[TBL_CUSTOMER] ([CustomerId])
GO

-- Seed some banks
INSERT INTO [dbo].[TBL_BANK] (BankName, BankCode, LogoUrl) VALUES ('Access Bank', '044', 'https://logos-world.net/wp-content/uploads/2021/02/Access-Bank-Logo.png')
INSERT INTO [dbo].[TBL_BANK] (BankName, BankCode, LogoUrl) VALUES ('Guaranty Trust Bank', '058', 'https://upload.wikimedia.org/wikipedia/commons/thumb/1/12/Guaranty_Trust_Bank_logo.svg/2048px-Guaranty_Trust_Bank_logo.svg.png')
INSERT INTO [dbo].[TBL_BANK] (BankName, BankCode, LogoUrl) VALUES ('First Bank of Nigeria', '011', 'https://upload.wikimedia.org/wikipedia/en/thumb/6/62/First_Bank_of_Nigeria_logo.svg/1200px-First_Bank_of_Nigeria_logo.svg.png')
INSERT INTO [dbo].[TBL_BANK] (BankName, BankCode, LogoUrl) VALUES ('Zenith Bank', '057', 'https://upload.wikimedia.org/wikipedia/commons/thumb/c/c5/Zenith_Bank_logo.svg/1200px-Zenith_Bank_logo.svg.png')
INSERT INTO [dbo].[TBL_BANK] (BankName, BankCode, LogoUrl) VALUES ('United Bank for Africa', '033', 'https://upload.wikimedia.org/wikipedia/commons/6/6f/United_Bank_for_Africa_Logo.png')
GO
