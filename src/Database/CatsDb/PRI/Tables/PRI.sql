CREATE TABLE [PRI].[PRI] (
    [Id]                                     UNIQUEIDENTIFIER NOT NULL,
    [ParticipantId]                          NVARCHAR (9)     NOT NULL,
    [ExpectedReleaseDate]                    DATE             NOT NULL,
    [ActualReleaseDate]                      DATE             NULL,
    [AcceptedOn]                             DATETIME2 (7)    NULL,
    [ExpectedReleaseRegionId]                INT              NOT NULL,
    [AssignedTo]                             NVARCHAR (36)    NULL,
    [MeetingAttendedOn]                      DATE             NOT NULL,
    [ReasonParticipantDidNotAttendInPerson]  NVARCHAR (1000)  NULL,
    [Created]                                DATETIME2 (7)    NULL,
    [CreatedBy]                              NVARCHAR (36)    NULL,
    [LastModified]                           DATETIME2 (7)    NULL,
    [LastModifiedBy]                         NVARCHAR (36)    NULL,
    [ReasonCommunityDidNotAttendInPerson]    NVARCHAR (1000)  NULL,
    [ReasonCustodyDidNotAttendInPerson]      NVARCHAR (1000)  NULL,
    [CustodyLocationId]                      INT              DEFAULT ((0)) NOT NULL,
    [ObjectiveId]                            UNIQUEIDENTIFIER DEFAULT ('00000000-0000-0000-0000-000000000000') NOT NULL,
    [PostReleaseCommunitySupportInformation] NVARCHAR (1000)  NULL,
    [CompletedBy]                            NVARCHAR (36)    NULL,
    [AbandonJustification]                   NVARCHAR (1000)  NULL,
    [Status]                                 INT              DEFAULT ((0)) NOT NULL,
    [AbandonReason]                          INT              NULL,
    [CompletedOn]                            DATETIME2 (7)    NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_PRI_CompletedBy]
    ON [PRI].[PRI]([CompletedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_PRI_ParticipantId]
    ON [PRI].[PRI]([ParticipantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_PRI_ExpectedReleaseRegionId]
    ON [PRI].[PRI]([ExpectedReleaseRegionId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_PRI_CustodyLocationId]
    ON [PRI].[PRI]([CustodyLocationId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_PRI_AssignedTo]
    ON [PRI].[PRI]([AssignedTo] ASC);
GO

ALTER TABLE [PRI].[PRI]
    ADD CONSTRAINT [FK_PRI_User_AssignedTo] FOREIGN KEY ([AssignedTo]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [PRI].[PRI]
    ADD CONSTRAINT [FK_PRI_Location_ExpectedReleaseRegionId] FOREIGN KEY ([ExpectedReleaseRegionId]) REFERENCES [Configuration].[Location] ([Id]);
GO

ALTER TABLE [PRI].[PRI]
    ADD CONSTRAINT [FK_PRI_Location_CustodyLocationId] FOREIGN KEY ([CustodyLocationId]) REFERENCES [Configuration].[Location] ([Id]);
GO

ALTER TABLE [PRI].[PRI]
    ADD CONSTRAINT [FK_PRI_User_CompletedBy] FOREIGN KEY ([CompletedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [PRI].[PRI]
    ADD CONSTRAINT [FK_PRI_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]);
GO

ALTER TABLE [PRI].[PRI]
    ADD CONSTRAINT [PK_PRI] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

