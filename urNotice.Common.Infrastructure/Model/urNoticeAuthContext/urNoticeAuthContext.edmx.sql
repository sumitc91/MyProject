
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/18/2016 13:53:31
-- Generated from EDMX file: C:\code\svn\urNoticeSvn\urNotice.Common.Infrastructure\Model\urNoticeAuthContext\urNoticeAuthContext.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [urnoticeAuth];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Admins]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Admins];
GO
IF OBJECT_ID(N'[dbo].[Companies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Companies];
GO
IF OBJECT_ID(N'[dbo].[CompanyDesignationInfoes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CompanyDesignationInfoes];
GO
IF OBJECT_ID(N'[dbo].[CompanySubParts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CompanySubParts];
GO
IF OBJECT_ID(N'[dbo].[CompanyTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CompanyTypes];
GO
IF OBJECT_ID(N'[dbo].[contactUs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[contactUs];
GO
IF OBJECT_ID(N'[dbo].[Designations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Designations];
GO
IF OBJECT_ID(N'[dbo].[FacebookAuths]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FacebookAuths];
GO
IF OBJECT_ID(N'[dbo].[ForgetPasswords]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ForgetPasswords];
GO
IF OBJECT_ID(N'[dbo].[linkedinAuths]', 'U') IS NOT NULL
    DROP TABLE [dbo].[linkedinAuths];
GO
IF OBJECT_ID(N'[dbo].[Locations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Locations];
GO
IF OBJECT_ID(N'[dbo].[RecommendedBies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RecommendedBies];
GO
IF OBJECT_ID(N'[dbo].[Subscriptions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Subscriptions];
GO
IF OBJECT_ID(N'[dbo].[UserAlerts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserAlerts];
GO
IF OBJECT_ID(N'[dbo].[UserCompanyInfoes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserCompanyInfoes];
GO
IF OBJECT_ID(N'[dbo].[UserEarningHistories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserEarningHistories];
GO
IF OBJECT_ID(N'[dbo].[UserEarnings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserEarnings];
GO
IF OBJECT_ID(N'[dbo].[UserMessages]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserMessages];
GO
IF OBJECT_ID(N'[dbo].[UserReputationMappings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserReputationMappings];
GO
IF OBJECT_ID(N'[dbo].[UserReputations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserReputations];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[ValidateUserKeys]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ValidateUserKeys];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Admins'
CREATE TABLE [dbo].[Admins] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [userId] nvarchar(max)  NOT NULL,
    [createdDate] datetime  NOT NULL,
    [priviledgeLevel] smallint  NOT NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'Companies'
CREATE TABLE [dbo].[Companies] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [companyName] nvarchar(max)  NOT NULL,
    [website] nvarchar(max)  NULL,
    [headquarters] nvarchar(max)  NULL,
    [size] bigint  NULL,
    [founded] nvarchar(max)  NULL,
    [type] nvarchar(max)  NULL,
    [industry] nvarchar(max)  NULL,
    [revenue] bigint  NULL,
    [descriptions] nvarchar(max)  NULL,
    [mission] nvarchar(max)  NULL,
    [founder] nvarchar(max)  NULL,
    [lastUpdated] datetime  NOT NULL,
    [createdBy] nvarchar(max)  NOT NULL,
    [lastUpdatedBy] nvarchar(max)  NOT NULL,
    [createdDate] datetime  NOT NULL,
    [universalName] nvarchar(max)  NOT NULL,
    [averageRating] float  NOT NULL,
    [totalNumberOfRatings] bigint  NOT NULL,
    [totalReviews] bigint  NOT NULL,
    [guid] nvarchar(max)  NOT NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'CompanyDesignationInfoes'
CREATE TABLE [dbo].[CompanyDesignationInfoes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [companyId] nvarchar(max)  NOT NULL,
    [locationId] nvarchar(max)  NOT NULL,
    [designationId] nvarchar(max)  NOT NULL,
    [lastUpdated] datetime  NOT NULL,
    [avgSalary] float  NOT NULL,
    [numOfSalary] int  NOT NULL,
    [isVerified] bit  NOT NULL,
    [verifiedBy] nvarchar(max)  NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'CompanySubParts'
CREATE TABLE [dbo].[CompanySubParts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [companyId] nvarchar(max)  NOT NULL,
    [locationId] nvarchar(max)  NULL,
    [website] nvarchar(max)  NULL,
    [size] bigint  NULL,
    [description] nvarchar(max)  NULL,
    [lastUpdated] datetime  NOT NULL,
    [lastUpdatedBy] nvarchar(max)  NOT NULL,
    [CompanyName] nvarchar(max)  NOT NULL,
    [averageRating] float  NOT NULL,
    [totalNumberOfRatings] bigint  NOT NULL,
    [totalReviews] bigint  NOT NULL,
    [isPrimary] bit  NOT NULL,
    [logoUrl] nvarchar(max)  NULL,
    [squareLogoUrl] nvarchar(max)  NULL,
    [specialities] nvarchar(max)  NULL,
    [guid] nvarchar(max)  NOT NULL,
    [avgNoticePeriod] float  NOT NULL,
    [buyoutPercentage] float  NOT NULL,
    [maxNoticePeriod] int  NOT NULL,
    [minNoticePeriod] int  NOT NULL,
    [avgHikePercentage] float  NOT NULL,
    [percLookingForChange] float  NOT NULL,
    [isSolrUpdated] bit  NULL,
    [telephone] nvarchar(max)  NOT NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'CompanyTypes'
CREATE TABLE [dbo].[CompanyTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [type] nvarchar(max)  NOT NULL,
    [createdBy] nvarchar(max)  NOT NULL,
    [verifiedBy] nvarchar(max)  NULL,
    [isVerified] bit  NOT NULL,
    [createdDate] datetime  NOT NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'contactUs'
CREATE TABLE [dbo].[contactUs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [username] nvarchar(max)  NOT NULL,
    [emailId] nvarchar(max)  NOT NULL,
    [heading] nvarchar(max)  NOT NULL,
    [message] nvarchar(max)  NOT NULL,
    [dateTime] datetime  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Phone] nvarchar(max)  NOT NULL,
    [Status] nvarchar(max)  NOT NULL,
    [ReplyMessage] nvarchar(max)  NOT NULL,
    [RepliedBy] nvarchar(max)  NOT NULL,
    [RepliedDateTime] nvarchar(max)  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'Designations'
CREATE TABLE [dbo].[Designations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [designationName] nvarchar(max)  NOT NULL,
    [displayName] nvarchar(max)  NULL,
    [isVerified] bit  NOT NULL,
    [verifiedBy] nvarchar(max)  NULL,
    [createdBy] nvarchar(max)  NOT NULL,
    [createdDate] datetime  NOT NULL,
    [guid] nvarchar(max)  NOT NULL,
    [isSolrUpdated] bit  NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'FacebookAuths'
CREATE TABLE [dbo].[FacebookAuths] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [username] nvarchar(max)  NOT NULL,
    [datetime] nvarchar(max)  NOT NULL,
    [facebookId] nvarchar(max)  NOT NULL,
    [facebookUsername] nvarchar(max)  NOT NULL,
    [AuthToken] nvarchar(max)  NOT NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'ForgetPasswords'
CREATE TABLE [dbo].[ForgetPasswords] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(max)  NOT NULL,
    [guid] nvarchar(max)  NOT NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'linkedinAuths'
CREATE TABLE [dbo].[linkedinAuths] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [oauth_Token] nvarchar(max)  NOT NULL,
    [oauth_TokenSecret] nvarchar(max)  NOT NULL,
    [oauth_verifier] nvarchar(max)  NOT NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'Locations'
CREATE TABLE [dbo].[Locations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [city] nvarchar(max)  NULL,
    [state] nvarchar(max)  NULL,
    [country] nvarchar(max)  NULL,
    [isVerified] bit  NOT NULL,
    [verifiedBy] nvarchar(max)  NULL,
    [createdBy] nvarchar(max)  NOT NULL,
    [createdDate] datetime  NOT NULL,
    [district] nvarchar(max)  NULL,
    [subLocality] nvarchar(max)  NULL,
    [postal_code] nvarchar(max)  NULL,
    [guid] nvarchar(max)  NOT NULL,
    [latitude] float  NULL,
    [longitude] float  NULL,
    [formatted_address] nvarchar(max)  NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'RecommendedBies'
CREATE TABLE [dbo].[RecommendedBies] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RecommendedTo] nvarchar(max)  NOT NULL,
    [RecommendedFrom] nvarchar(max)  NOT NULL,
    [DateTime] datetime  NULL,
    [isValid] nvarchar(max)  NULL,
    [RecommendedFromUsername] nvarchar(max)  NOT NULL,
    [UserId] int  NOT NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'Subscriptions'
CREATE TABLE [dbo].[Subscriptions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [subscriptionType] smallint  NOT NULL,
    [userId] nvarchar(max)  NULL,
    [emaiId] nvarchar(max)  NOT NULL,
    [isRegisteredUser] bit  NOT NULL,
    [isEmailAvailable] bit  NOT NULL,
    [createdDate] datetime  NOT NULL,
    [isActive] bit  NOT NULL,
    [guid] nvarchar(max)  NOT NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'UserAlerts'
CREATE TABLE [dbo].[UserAlerts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [userType] nvarchar(max)  NOT NULL,
    [username] nvarchar(max)  NOT NULL,
    [titleText] nvarchar(max)  NOT NULL,
    [dateTime] datetime  NOT NULL,
    [priority] nvarchar(max)  NOT NULL,
    [iconUrl] nvarchar(max)  NOT NULL,
    [messageFrom] nvarchar(max)  NOT NULL,
    [messageTo] nvarchar(max)  NOT NULL,
    [AlertSeen] nvarchar(max)  NOT NULL,
    [UserId] int  NOT NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'UserCompanyInfoes'
CREATE TABLE [dbo].[UserCompanyInfoes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [companyId] nvarchar(max)  NOT NULL,
    [startDate] datetime  NULL,
    [endDate] datetime  NULL,
    [userId] nvarchar(max)  NOT NULL,
    [designationId] nvarchar(max)  NULL,
    [salaryRange] int  NULL,
    [isHappy] bit  NULL,
    [reason] nvarchar(max)  NULL,
    [createdDate] datetime  NULL,
    [shareReasonPublic] bit  NULL,
    [shareAsAnonymous] bit  NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'UserEarningHistories'
CREATE TABLE [dbo].[UserEarningHistories] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [username] nvarchar(max)  NOT NULL,
    [type] nvarchar(max)  NOT NULL,
    [subtype] nvarchar(max)  NOT NULL,
    [paymentMode] nvarchar(max)  NOT NULL,
    [title] nvarchar(max)  NOT NULL,
    [amount] nvarchar(max)  NOT NULL,
    [dateTime] datetime  NOT NULL,
    [userType] nvarchar(max)  NOT NULL,
    [currency] nvarchar(max)  NOT NULL,
    [UserId] int  NOT NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'UserEarnings'
CREATE TABLE [dbo].[UserEarnings] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [username] nvarchar(max)  NOT NULL,
    [total] nvarchar(max)  NOT NULL,
    [approved] nvarchar(max)  NOT NULL,
    [pending] nvarchar(max)  NOT NULL,
    [currency] nvarchar(max)  NOT NULL,
    [LastUpdated] datetime  NULL,
    [userType] nvarchar(max)  NOT NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'UserMessages'
CREATE TABLE [dbo].[UserMessages] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [userType] nvarchar(max)  NOT NULL,
    [username] nvarchar(max)  NOT NULL,
    [titleText] nvarchar(max)  NOT NULL,
    [bodyText] nvarchar(max)  NOT NULL,
    [dateTime] datetime  NOT NULL,
    [priority] nvarchar(max)  NOT NULL,
    [iconUrl] nvarchar(max)  NOT NULL,
    [messageFrom] nvarchar(max)  NOT NULL,
    [messageTo] nvarchar(max)  NOT NULL,
    [messageSeen] nvarchar(max)  NOT NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'UserReputationMappings'
CREATE TABLE [dbo].[UserReputationMappings] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [username] nvarchar(max)  NOT NULL,
    [type] nvarchar(max)  NOT NULL,
    [subType] nvarchar(max)  NOT NULL,
    [description] nvarchar(max)  NOT NULL,
    [DateTime] datetime  NULL,
    [reputation] nvarchar(max)  NOT NULL,
    [UserId] int  NOT NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'UserReputations'
CREATE TABLE [dbo].[UserReputations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [username] nvarchar(max)  NOT NULL,
    [ReputationScore] nvarchar(max)  NOT NULL,
    [UserBadge] nvarchar(max)  NULL,
    [isDBSynced] bit  NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [username] nvarchar(max)  NOT NULL,
    [password] nvarchar(max)  NOT NULL,
    [isActive] nvarchar(max)  NOT NULL,
    [source] nvarchar(max)  NOT NULL,
    [guid] nvarchar(max)  NOT NULL,
    [firstName] nvarchar(max)  NOT NULL,
    [lastName] nvarchar(max)  NOT NULL,
    [imageUrl] nvarchar(max)  NULL,
    [gender] nvarchar(max)  NULL,
    [locked] nvarchar(max)  NULL,
    [keepMeSignedIn] nvarchar(max)  NULL,
    [registrationTime] nvarchar(max)  NULL,
    [lastUpdatedDate] datetime  NULL,
    [fixedGuid] nvarchar(max)  NULL,
    [isVerified] nvarchar(max)  NULL,
    [priviledgeLevel] smallint  NOT NULL,
    [timeZone] nvarchar(max)  NULL,
    [phone] bigint  NULL,
    [isPhoneActive] bit  NULL,
    [userCoverPic] nvarchar(max)  NULL,
    [locationId] nvarchar(max)  NULL,
    [isDBSynced] bit  NULL,
    [email] nvarchar(max)  NULL,
    [fid] nvarchar(max)  NULL
);
GO

-- Creating table 'ValidateUserKeys'
CREATE TABLE [dbo].[ValidateUserKeys] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(max)  NOT NULL,
    [guid] nvarchar(max)  NOT NULL,
    [isDBSynced] bit  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Admins'
ALTER TABLE [dbo].[Admins]
ADD CONSTRAINT [PK_Admins]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Companies'
ALTER TABLE [dbo].[Companies]
ADD CONSTRAINT [PK_Companies]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CompanyDesignationInfoes'
ALTER TABLE [dbo].[CompanyDesignationInfoes]
ADD CONSTRAINT [PK_CompanyDesignationInfoes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CompanySubParts'
ALTER TABLE [dbo].[CompanySubParts]
ADD CONSTRAINT [PK_CompanySubParts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CompanyTypes'
ALTER TABLE [dbo].[CompanyTypes]
ADD CONSTRAINT [PK_CompanyTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'contactUs'
ALTER TABLE [dbo].[contactUs]
ADD CONSTRAINT [PK_contactUs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Designations'
ALTER TABLE [dbo].[Designations]
ADD CONSTRAINT [PK_Designations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FacebookAuths'
ALTER TABLE [dbo].[FacebookAuths]
ADD CONSTRAINT [PK_FacebookAuths]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ForgetPasswords'
ALTER TABLE [dbo].[ForgetPasswords]
ADD CONSTRAINT [PK_ForgetPasswords]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'linkedinAuths'
ALTER TABLE [dbo].[linkedinAuths]
ADD CONSTRAINT [PK_linkedinAuths]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Locations'
ALTER TABLE [dbo].[Locations]
ADD CONSTRAINT [PK_Locations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RecommendedBies'
ALTER TABLE [dbo].[RecommendedBies]
ADD CONSTRAINT [PK_RecommendedBies]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Subscriptions'
ALTER TABLE [dbo].[Subscriptions]
ADD CONSTRAINT [PK_Subscriptions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserAlerts'
ALTER TABLE [dbo].[UserAlerts]
ADD CONSTRAINT [PK_UserAlerts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserCompanyInfoes'
ALTER TABLE [dbo].[UserCompanyInfoes]
ADD CONSTRAINT [PK_UserCompanyInfoes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserEarningHistories'
ALTER TABLE [dbo].[UserEarningHistories]
ADD CONSTRAINT [PK_UserEarningHistories]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserEarnings'
ALTER TABLE [dbo].[UserEarnings]
ADD CONSTRAINT [PK_UserEarnings]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserMessages'
ALTER TABLE [dbo].[UserMessages]
ADD CONSTRAINT [PK_UserMessages]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserReputationMappings'
ALTER TABLE [dbo].[UserReputationMappings]
ADD CONSTRAINT [PK_UserReputationMappings]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserReputations'
ALTER TABLE [dbo].[UserReputations]
ADD CONSTRAINT [PK_UserReputations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ValidateUserKeys'
ALTER TABLE [dbo].[ValidateUserKeys]
ADD CONSTRAINT [PK_ValidateUserKeys]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------