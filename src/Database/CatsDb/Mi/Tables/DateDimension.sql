CREATE TABLE [Mi].[DateDimension] (
    [TheDate]             DATE          NOT NULL,
    [TheDay]              INT           NOT NULL,
    [TheDaySuffix]        NVARCHAR (2)  NOT NULL,
    [TheDayName]          NVARCHAR (20) NOT NULL,
    [TheDayOfWeek]        INT           NOT NULL,
    [TheDayOfWeekInMonth] INT           NOT NULL,
    [TheDayOfYear]        INT           NOT NULL,
    [IsWeekend]           BIT           NOT NULL,
    [TheWeek]             INT           NOT NULL,
    [TheISOweek]          INT           NOT NULL,
    [TheFirstOfWeek]      DATE          NOT NULL,
    [TheLastOfWeek]       DATE          NOT NULL,
    [TheWeekOfMonth]      INT           NOT NULL,
    [TheMonth]            INT           NOT NULL,
    [TheMonthName]        NVARCHAR (20) NOT NULL,
    [TheFirstOfMonth]     DATE          NOT NULL,
    [TheLastOfMonth]      DATE          NOT NULL,
    [TheFirstOfNextMonth] DATE          NOT NULL,
    [TheLastOfNextMonth]  DATE          NOT NULL,
    [TheQuarter]          INT           NOT NULL,
    [TheFirstOfQuarter]   DATE          NOT NULL,
    [TheLastOfQuarter]    DATE          NOT NULL,
    [TheYear]             INT           NOT NULL,
    [TheISOYear]          INT           NOT NULL,
    [TheFirstOfYear]      DATE          NOT NULL,
    [TheLastOfYear]       DATE          NOT NULL,
    [IsLeapYear]          BIT           NOT NULL,
    [Has53Weeks]          BIT           NOT NULL,
    [Has53ISOWeeks]       BIT           NOT NULL,
    [MMYYYY]              NVARCHAR (7)  NOT NULL,
    [Style101]            NVARCHAR (10) NOT NULL,
    [Style103]            NVARCHAR (10) NOT NULL,
    [Style112]            NVARCHAR (8)  NOT NULL,
    [Style120]            NVARCHAR (10) NOT NULL
);
GO

ALTER TABLE [Mi].[DateDimension]
    ADD CONSTRAINT [PK_DateDimension] PRIMARY KEY CLUSTERED ([TheDate] ASC);
GO

