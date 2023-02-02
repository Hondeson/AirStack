USE[AirStack]
GO

/****** Object:  Table [dbo].[ItemHistoryQueue]    Script Date: 25.01.2023 19:16:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ItemHistoryQueue] (

    [ID][bigint] IDENTITY(1, 1) NOT NULL,

    [ItemCode] [nvarchar] (300) NOT NULL,

    [ParentCode] [nvarchar] (300) NOT NULL,

    [StatusID] [bigint] NOT NULL,

    [CreatedAt] [datetime] NOT NULL,
 CONSTRAINT[PK_ItemStatusQueue] PRIMARY KEY CLUSTERED 
(

    [ID] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
) ON[PRIMARY]
GO
