CREATE TABLE [Induction].[WingInductionPhase] (
    [Number]               INT              NOT NULL,
    [WingInductionId]      UNIQUEIDENTIFIER NOT NULL,
    [StartDate]            DATETIME2 (7)    NOT NULL,
    [CompletedDate]        DATETIME2 (7)    NULL,
    [AbandonJustification] NVARCHAR (1000)  NULL,
    [AbandonReason]        INT              NULL,
    [CompletedBy]          NVARCHAR (36)    NULL,
    [Status]               INT              DEFAULT ((0)) NOT NULL,
    [Id]                   UNIQUEIDENTIFIER DEFAULT ('00000000-0000-0000-0000-000000000000') NOT NULL
);
GO

ALTER TABLE [Induction].[WingInductionPhase]
    ADD CONSTRAINT [FK_WingInductionPhase_User_CompletedBy] FOREIGN KEY ([CompletedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Induction].[WingInductionPhase]
    ADD CONSTRAINT [FK_WingInductionPhase_WingInduction_WingInductionId] FOREIGN KEY ([WingInductionId]) REFERENCES [Induction].[WingInduction] ([Id]) ON DELETE CASCADE;
GO

CREATE NONCLUSTERED INDEX [IX_WingInductionPhase_CompletedBy]
    ON [Induction].[WingInductionPhase]([CompletedBy] ASC);
GO

ALTER TABLE [Induction].[WingInductionPhase]
    ADD CONSTRAINT [PK_WingInductionPhase] PRIMARY KEY CLUSTERED ([WingInductionId] ASC, [Number] ASC, [Id] ASC);
GO

