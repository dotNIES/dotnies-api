CREATE TABLE [fin].[GeneralLedgerDetail]
(
	[Id]                INT                 IDENTITY (1, 1) NOT NULL,
    [CreatedOn]         DATETIME2 (7)       NOT NULL,
    [CreatedBy]         INT                 NOT NULL,
    [LastModifiedOn]    DATETIME2 (7)       NOT NULL,
    [LastModifiedBy]    INT                 NOT NULL,
    [PurchaseTypeId]    INT                 NOT NULL,
    [CategoryId]        INT                 NOT NULL,
    [EntryDate]         DATETIME2 (7)       NOT NULL,
    [Amount]            DECIMAL (18, 4)     NOT NULL,
    [DiscountAmt]       DECIMAL (18, 4)     NOT NULL,
    [TaxRate]           DECIMAL (8, 4)      NOT NULL DEFAULT(0.21),
    [TaxAmt]            DECIMAL (18, 4)     NOT NULL,
    [TotalAmt]          DECIMAL (18, 4)     NOT NULL,
    [Debit]             BIT                 NOT NULL,
    [Description]       VARCHAR (100)       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_GeneralLedgerDetail_PurchaseType] FOREIGN KEY ([PurchaseTypeId]) REFERENCES [fin].[PurchaseType]([Id]),
    CONSTRAINT [FK_GeneralLedgerDetail_Category] FOREIGN KEY ([CategoryId]) REFERENCES [common].[Category]([Id])
)
