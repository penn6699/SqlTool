USE [OnlineOffice]
GO
/****** Object:  Table [dbo].[Files]    Script Date: 2020/8/28 11:14:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Files](
	[FileId] [varchar](50) NOT NULL,
	[IsFolder] [bit] NOT NULL,
	[FolderId] [varchar](50) NULL,
	[FileName] [varchar](500) NULL,
	[FileSize] [float] NULL,
	[FileExt] [varchar](50) NULL,
	[FileContentType] [varchar](50) NULL,
	[FileUpdateTime] [datetime] NULL,
	[FilePath] [varchar](500) NULL,
	[FileUrl] [varchar](500) NULL,
	[CreateTime] [datetime] NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED 
(
	[FileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[test]    Script Date: 2020/8/28 11:14:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[test](
	[a] [bigint] NOT NULL,
	[a1] [binary](50) NULL,
	[a2] [bit] NULL,
	[a3] [char](10) NULL,
	[a4] [date] NULL,
	[a5] [datetime] NULL,
	[a6] [datetime2](7) NULL,
	[a7] [datetimeoffset](7) NULL,
	[a8] [decimal](18, 0) NULL,
	[a9] [float] NULL,
	[a10] [geography] NULL,
	[b1] [geometry] NULL,
	[b2] [hierarchyid] NULL,
	[b21] [image] NULL,
	[b3] [int] NULL,
	[b4] [money] NULL,
	[b5] [nchar](10) NULL,
	[b6] [ntext] NULL,
	[b7] [numeric](18, 0) NULL,
	[b8] [nvarchar](50) NULL,
	[b9] [nvarchar](max) NULL,
	[b10] [real] NULL,
	[c1] [smalldatetime] NULL,
	[c2] [smallint] NULL,
	[c3] [smallmoney] NULL,
	[c4] [sql_variant] NULL,
	[c5] [text] NULL,
	[c6] [time](7) NULL,
	[c7] [timestamp] NULL,
	[c8] [tinyint] NULL,
	[c9] [uniqueidentifier] NULL,
	[c10] [varbinary](50) NULL,
	[d1] [varbinary](max) NULL,
	[d2] [varchar](50) NULL,
	[d3] [varchar](max) NULL,
	[d4] [xml] NULL,
 CONSTRAINT [PK_test] PRIMARY KEY CLUSTERED 
(
	[a] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users]    Script Date: 2020/8/28 11:14:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [varchar](50) NOT NULL,
	[UserName] [nvarchar](50) NULL,
	[UserRole] [varchar](50) NOT NULL,
	[LoginId] [varchar](50) NOT NULL,
	[Password] [varchar](50) NOT NULL,
	[Disabled] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateTime] [datetime] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Files] ADD  CONSTRAINT [DF_Files_IsFolder]  DEFAULT ((0)) FOR [IsFolder]
GO
ALTER TABLE [dbo].[Files] ADD  CONSTRAINT [DF_Files_FileSize]  DEFAULT ((0)) FOR [FileSize]
GO
ALTER TABLE [dbo].[Files] ADD  CONSTRAINT [DF_Files_FileCreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[Files] ADD  CONSTRAINT [DF_Files_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_Disabled]  DEFAULT ((0)) FOR [Disabled]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO
