
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/22/2016 23:50:58
-- Generated from EDMX file: C:\code\svn\urNoticeSvn\urNotice.Common.Infrastructure\Model\urNoticeAnalyticsContext\urNoticeAnalyticsContext.edmx
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

IF OBJECT_ID(N'[dbo].[GoogleApiCheckLastSynceds]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GoogleApiCheckLastSynceds];
GO
IF OBJECT_ID(N'[dbo].[GoogleApiContacts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GoogleApiContacts];
GO
IF OBJECT_ID(N'[dbo].[GoogleApiContactsArchiveds]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GoogleApiContactsArchiveds];
GO
IF OBJECT_ID(N'[dbo].[VirtualFriendLists]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VirtualFriendLists];
GO
IF OBJECT_ID(N'[dbo].[VirtualFriendListJsons]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VirtualFriendListJsons];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'GoogleApiCheckLastSynceds'
CREATE TABLE [dbo].[GoogleApiCheckLastSynceds] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [emailId] nvarchar(max)  NOT NULL,
    [lastContactUpdated] nvarchar(max)  NOT NULL
);
GO

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

-- Creating table 'GoogleApiContactsArchiveds'
CREATE TABLE [dbo].[GoogleApiContactsArchiveds] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [emailId] nvarchar(max)  NOT NULL,
    [createdDate] datetime  NOT NULL,
    [startIndex] int  NOT NULL,
    [itemPerPage] int  NOT NULL,
    [lastContactUpdated] nvarchar(max)  NULL,
    [entryListString] nvarchar(max)  NOT NULL,
    [isSolrUpdated] bit  NOT NULL
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

-- Creating table 'VirtualFriendListJsons'
CREATE TABLE [dbo].[VirtualFriendListJsons] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [user1] nvarchar(max)  NOT NULL,
    [jsonData] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'GoogleApiCheckLastSynceds'
ALTER TABLE [dbo].[GoogleApiCheckLastSynceds]
ADD CONSTRAINT [PK_GoogleApiCheckLastSynceds]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GoogleApiContacts'
ALTER TABLE [dbo].[GoogleApiContacts]
ADD CONSTRAINT [PK_GoogleApiContacts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GoogleApiContactsArchiveds'
ALTER TABLE [dbo].[GoogleApiContactsArchiveds]
ADD CONSTRAINT [PK_GoogleApiContactsArchiveds]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VirtualFriendLists'
ALTER TABLE [dbo].[VirtualFriendLists]
ADD CONSTRAINT [PK_VirtualFriendLists]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VirtualFriendListJsons'
ALTER TABLE [dbo].[VirtualFriendListJsons]
ADD CONSTRAINT [PK_VirtualFriendListJsons]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------