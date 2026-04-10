CREATE TABLE [Activities].[Activity] (
    [Id]                           UNIQUEIDENTIFIER NOT NULL,
    [Definition]                   INT              NOT NULL,
    [Category]                     INT              NOT NULL,
    [Type]                         INT              NOT NULL,
    [ParticipantId]                NVARCHAR (9)     NOT NULL,
    [TaskId]                       UNIQUEIDENTIFIER NOT NULL,
    [ObjectiveId]                  UNIQUEIDENTIFIER NOT NULL,
    [TookPlaceAtLocationId]        INT              NOT NULL,
    [TookPlaceAtContractId]        NVARCHAR (12)    NOT NULL,
    [ParticipantCurrentLocationId] INT              NOT NULL,
    [ParticipantCurrentContractId] NVARCHAR (12)    NULL,
    [ParticipantStatus]            INT              NOT NULL,
    [AdditionalInformation]        NVARCHAR (1000)  NULL,
    [CommencedOn]                  DATETIME2 (7)    NOT NULL,
    [TenantId]                     NVARCHAR (50)    NOT NULL,
    [Status]                       INT              NOT NULL,
    [Created]                      DATETIME2 (7)    NULL,
    [CreatedBy]                    NVARCHAR (36)    NULL,
    [LastModified]                 DATETIME2 (7)    NULL,
    [LastModifiedBy]               NVARCHAR (36)    NULL,
    [OwnerId]                      NVARCHAR (36)    NULL,
    [EditorId]                     NVARCHAR (36)    NULL,
    [CompletedOn]                  DATETIME2 (7)    NULL,
    [CompletedBy]                  NVARCHAR (36)    NULL,
    [AbandonJustification]         NVARCHAR (1000)  NULL,
    [AbandonReason]                INT              NULL
);
GO

ALTER TABLE [Activities].[Activity]
    ADD CONSTRAINT [FK_Activity_Location_TookPlaceAtLocationId] FOREIGN KEY ([TookPlaceAtLocationId]) REFERENCES [Configuration].[Location] ([Id]);
GO

ALTER TABLE [Activities].[Activity]
    ADD CONSTRAINT [FK_Activity_Location_ParticipantCurrentLocationId] FOREIGN KEY ([ParticipantCurrentLocationId]) REFERENCES [Configuration].[Location] ([Id]);
GO

ALTER TABLE [Activities].[Activity]
    ADD CONSTRAINT [FK_Activity_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Configuration].[Tenant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Activities].[Activity]
    ADD CONSTRAINT [FK_Activity_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Activities].[Activity]
    ADD CONSTRAINT [FK_Activity_Contract_TookPlaceAtContractId] FOREIGN KEY ([TookPlaceAtContractId]) REFERENCES [Configuration].[Contract] ([Id]);
GO

ALTER TABLE [Activities].[Activity]
    ADD CONSTRAINT [FK_Activity_Contract_ParticipantCurrentContractId] FOREIGN KEY ([ParticipantCurrentContractId]) REFERENCES [Configuration].[Contract] ([Id]);
GO

ALTER TABLE [Activities].[Activity]
    ADD CONSTRAINT [FK_Activity_User_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[Activity]
    ADD CONSTRAINT [FK_Activity_User_EditorId] FOREIGN KEY ([EditorId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[Activity]
    ADD CONSTRAINT [FK_Activity_User_CompletedBy] FOREIGN KEY ([CompletedBy]) REFERENCES [Identity].[User] ([Id]);
GO

CREATE NONCLUSTERED INDEX [IX_Activity_CompletedBy]
    ON [Activities].[Activity]([CompletedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Activity_ParticipantCurrentLocationId]
    ON [Activities].[Activity]([ParticipantCurrentLocationId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Activity_ParticipantCurrentContractId]
    ON [Activities].[Activity]([ParticipantCurrentContractId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Activity_TookPlaceAtLocationId]
    ON [Activities].[Activity]([TookPlaceAtLocationId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Activity_EditorId]
    ON [Activities].[Activity]([EditorId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Activity_OwnerId]
    ON [Activities].[Activity]([OwnerId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Activity_TookPlaceAtContractId]
    ON [Activities].[Activity]([TookPlaceAtContractId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Activity_ParticipantId]
    ON [Activities].[Activity]([ParticipantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Activity_TenantId]
    ON [Activities].[Activity]([TenantId] ASC);
GO

ALTER TABLE [Activities].[Activity]
    ADD CONSTRAINT [PK_Activity] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

