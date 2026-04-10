CREATE TABLE [Identity].[User] (
    [Id]                     NVARCHAR (36)      NOT NULL,
    [DisplayName]            NVARCHAR (100)     NOT NULL,
    [ProviderId]             NVARCHAR (50)      NOT NULL,
    [TenantId]               NVARCHAR (50)      NOT NULL,
    [TenantName]             NVARCHAR (50)      NOT NULL,
    [ProfilePictureDataUrl]  TEXT               NULL,
    [IsActive]               BIT                NOT NULL,
    [IsLive]                 BIT                NOT NULL,
    [MemorablePlace]         NVARCHAR (50)      NOT NULL,
    [MemorableDate]          NVARCHAR (50)      NOT NULL,
    [RefreshToken]           NVARCHAR (MAX)     NULL,
    [RequiresPasswordReset]  BIT                NOT NULL,
    [RefreshTokenExpiryTime] DATETIME2 (7)      NOT NULL,
    [SuperiorId]             NVARCHAR (36)      NULL,
    [Created]                DATETIME2 (7)      NULL,
    [CreatedBy]              NVARCHAR (36)      NOT NULL,
    [LastModified]           DATETIME2 (7)      NULL,
    [LastModifiedBy]         NVARCHAR (36)      NULL,
    [UserName]               NVARCHAR (256)     NULL,
    [NormalizedUserName]     NVARCHAR (256)     NULL,
    [Email]                  NVARCHAR (256)     NULL,
    [NormalizedEmail]        NVARCHAR (256)     NULL,
    [EmailConfirmed]         BIT                NOT NULL,
    [PasswordHash]           NVARCHAR (MAX)     NULL,
    [SecurityStamp]          NVARCHAR (MAX)     NULL,
    [ConcurrencyStamp]       NVARCHAR (MAX)     NULL,
    [PhoneNumber]            NVARCHAR (20)      NULL,
    [PhoneNumberConfirmed]   BIT                NOT NULL,
    [TwoFactorEnabled]       BIT                NOT NULL,
    [LockoutEnd]             DATETIMEOFFSET (7) NULL,
    [LockoutEnabled]         BIT                NOT NULL,
    [AccessFailedCount]      INT                NOT NULL,
    [LastLogin]              DATETIME2 (7)      NULL
);
GO

ALTER TABLE [Identity].[User]
    ADD CONSTRAINT [FK_User_User_SuperiorId] FOREIGN KEY ([SuperiorId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Identity].[User]
    ADD CONSTRAINT [FK_User_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Configuration].[Tenant] ([Id]) ON DELETE CASCADE;
GO

CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [Identity].[User]([NormalizedUserName] ASC) WHERE ([NormalizedUserName] IS NOT NULL);
GO

CREATE NONCLUSTERED INDEX [IX_User_SuperiorId]
    ON [Identity].[User]([SuperiorId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_User_TenantId]
    ON [Identity].[User]([TenantId] ASC);
GO

CREATE NONCLUSTERED INDEX [EmailIndex]
    ON [Identity].[User]([NormalizedEmail] ASC);
GO

ALTER TABLE [Identity].[User]
    ADD CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

