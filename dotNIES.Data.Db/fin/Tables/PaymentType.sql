CREATE TABLE [fin].[PaymentType]
(
	[Id]                INT             IDENTITY (1, 1) NOT NULL  PRIMARY KEY,
    [CreatedOn]         DATETIME2 (7)   DEFAULT (GETUTCDATE()) NOT NULL,
    [CreatedBy]         INT             DEFAULT (USER_NAME()) NOT NULL,
    [LastModifiedOn]    DATETIME2 (7)   DEFAULT (GETUTCDATE()) NOT NULL,
    [LastModifiedBy]    INT             DEFAULT (USER_NAME())NOT NULL,
    [IsActive]          BIT             DEFAULT ((1)) NOT NULL,
    [Description]       NVARCHAR(100)   NOT NULL
)
