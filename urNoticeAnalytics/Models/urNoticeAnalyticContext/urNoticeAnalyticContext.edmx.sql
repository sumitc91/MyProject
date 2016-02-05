
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/19/2015 18:33:55
-- Generated from EDMX file: C:\code\svn\urNotice\urNoticeAnalytics\Models\urNoticeAnalyticContext\urNoticeAnalyticContext.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [urnoticeAnalytics];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[GoogleApiContacts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GoogleApiContacts];
GO
IF OBJECT_ID(N'[dbo].[GoogleApiCheckLastSynceds]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GoogleApiCheckLastSynceds];
GO
IF OBJECT_ID(N'[dbo].[VirtualFriendLists]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VirtualFriendLists];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'GoogleApiContacts'
CREATE TABLE [dbo].[GoogleApiContacts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [emailId] nvarchar(max)  NOT NULL,
    [createdDate] datetime  NOT NULL,
    [startIndex] int  NOT NULL,
    [itemPerPage] int  NOT NULL,
    [lastContactUpdated] nvarchar(max)  NULL,
    [entryListString] nvarchar(max)  NOT NULL,
    [isSolrUpdated] bit  NOT NULL,
    [isVirtualFriendListUpdated] bit  NOT NULL
);
GO

-- Creating table 'GoogleApiCheckLastSynceds'
CREATE TABLE [dbo].[GoogleApiCheckLastSynceds] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [emailId] nvarchar(max)  NOT NULL,
    [lastContactUpdated] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'VirtualFriendLists'
CREATE TABLE [dbo].[VirtualFriendLists] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [id1] nvarchar(max)  NOT NULL,
    [id2] nvarchar(max)  NOT NULL,
    [type] nvarchar(max)  NOT NULL,
    [source] nvarchar(max)  NOT NULL,
    [isSolrUpdated] bit  NOT NULL,
    [name1] nvarchar(max)  NULL,
    [name2] nvarchar(max)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'GoogleApiContacts'
ALTER TABLE [dbo].[GoogleApiContacts]
ADD CONSTRAINT [PK_GoogleApiContacts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GoogleApiCheckLastSynceds'
ALTER TABLE [dbo].[GoogleApiCheckLastSynceds]
ADD CONSTRAINT [PK_GoogleApiCheckLastSynceds]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VirtualFriendLists'
ALTER TABLE [dbo].[VirtualFriendLists]
ADD CONSTRAINT [PK_VirtualFriendLists]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------