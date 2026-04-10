CREATE TABLE [Dms].[LocationMapping] (
    [Code]           NVARCHAR (4)   NOT NULL,
    [CodeType]       NVARCHAR (9)   NOT NULL,
    [Description]    NVARCHAR (200) NOT NULL,
    [DeliveryRegion] NVARCHAR (200) NULL,
    [LocationId]     INT            NULL
);
GO

ALTER TABLE [Dms].[LocationMapping]
    ADD CONSTRAINT [FK_LocationMapping_Location_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Configuration].[Location] ([Id]);
GO

ALTER TABLE [Dms].[LocationMapping]
    ADD CONSTRAINT [PK_LocationMapping] PRIMARY KEY CLUSTERED ([Code] ASC, [CodeType] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_LocationMapping_LocationId]
    ON [Dms].[LocationMapping]([LocationId] ASC);
GO

