CREATE TABLE [fin].[GeneralLedger] (
    [Id]                INT                 IDENTITY (1, 1) NOT NULL,
    [CreatedOn]         DATETIME2 (7)       DEFAULT (GETUTCDATE()) NOT NULL,
    [CreatedBy]         NVARCHAR(128)       DEFAULT (USER_NAME()) NOT NULL,
    [LastModifiedOn]    DATETIME2 (7)       DEFAULT (GETUTCDATE()) NOT NULL,
    [LastModifiedBy]    NVARCHAR(128)       DEFAULT (USER_NAME())NOT NULL,
    [VendorId]          INT                 NOT NULL,
    [PaymentTypeId]     INT                 NOT NULL,
    [EntryDate]         DATETIME2 (7)       NOT NULL,
    [Amount]            DECIMAL (18, 4)     NOT NULL,
    [DiscountAmt]       DECIMAL (18, 4)     NOT NULL,
    [Debit]             BIT                 NOT NULL,
    [Description]       VARCHAR (100)       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_GeneralLedger_Vendor] FOREIGN KEY ([VendorId]) REFERENCES [fin].[Vendor]([Id]),
    CONSTRAINT [FK_GeneralLedger_PaymentType] FOREIGN KEY ([PaymentTypeId]) REFERENCES [fin].[PaymentType]([Id])
);
