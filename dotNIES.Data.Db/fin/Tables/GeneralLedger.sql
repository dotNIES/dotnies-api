CREATE TABLE [fin].[GeneralLedger] (
	[Id]                INT                 IDENTITY (1, 1) NOT NULL  PRIMARY KEY,
    [CreatedOn]         DATETIME2 (7)       DEFAULT (GETUTCDATE()) NOT NULL,
    [CreatedBy]         INT                 DEFAULT (USER_NAME()) NOT NULL,
    [LastModifiedOn]    DATETIME2 (7)       DEFAULT (GETUTCDATE()) NOT NULL,
    [LastModifiedBy]    INT                 DEFAULT (USER_NAME())NOT NULL,
    [PurchaseTypeId]    INT                 NOT NULL,
    [CategoryId]        INT                 NOT NULL,
    [EntryDate]         DATETIME2 (7)       DEFAULT (GETUTCDATE()) NOT NULL,
    [Amount]            DECIMAL (18, 4)     NOT NULL,
    [Debit]             BIT                 DEFAULT ((0)) NOT NULL,
    [Description]       VARCHAR (100)       NULL,
    CONSTRAINT [FK_GeneralLedger_PurchaseType] FOREIGN KEY ([PurchaseTypeId]) REFERENCES [fin].[PurchaseType]([Id]),
    CONSTRAINT [FK_GeneralLedger_Category] FOREIGN KEY ([CategoryId]) REFERENCES [common].[Category]([Id])
);

