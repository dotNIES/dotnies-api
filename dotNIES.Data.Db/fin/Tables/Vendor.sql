CREATE TABLE [fin].[Vendor]
(
	[Id]                INT             IDENTITY (1, 1) NOT NULL  PRIMARY KEY,
    [CreatedOn]         DATETIME2 (7)   DEFAULT (GETUTCDATE()) NOT NULL,
    [CreatedBy]         INT             DEFAULT (USER_NAME()) NOT NULL,
    [LastModifiedOn]    DATETIME2 (7)   DEFAULT (GETUTCDATE()) NOT NULL,
    [LastModifiedBy]    INT             DEFAULT (USER_NAME())NOT NULL,
    [IsActive]          BIT             DEFAULT ((1)) NOT NULL,
    [VendorTypeId]      INT             NOT NULL,
    [CategoryId]        INT             NOT NULL,
    [Name]              NVARCHAR(100)   NOT NULL,
    [Address]           NVARCHAR(500)   NULL,
    [City]              NVARCHAR(100)   NULL,
    [ZipCode]           NVARCHAR(20)    NULL,
    [Phone]             NVARCHAR(20)    NULL,
    [Email]             NVARCHAR(100)   NULL,
    [BankAccount]       NVARCHAR(50)    NULL,
    [Note]              NVARCHAR(500)   NULL,
)
