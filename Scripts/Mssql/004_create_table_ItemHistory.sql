USE[AirStack]
GO

/****** Object:  Table [dbo].[ItemHistory]    Script Date: 09.12.2022 12:06:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ItemHistory] (

    [ID][bigint] IDENTITY(1, 1) NOT NULL,

    [ItemID] [bigint] NOT NULL,

    [StatusID] [bigint] NOT NULL,

    [CreatedAt] [datetime] NOT NULL,
 CONSTRAINT[PK_ItemHistory] PRIMARY KEY CLUSTERED 
(

    [ID] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
) ON[PRIMARY]
GO

ALTER TABLE [dbo].[ItemHistory] WITH CHECK ADD  CONSTRAINT [FK_ItemHistory_Item] FOREIGN KEY([ItemID])
REFERENCES[dbo].[Item]([ID])
GO

ALTER TABLE [dbo].[ItemHistory] CHECK CONSTRAINT[FK_ItemHistory_Item]
GO

ALTER TABLE [dbo].[ItemHistory] WITH CHECK ADD  CONSTRAINT [FK_ItemHistory_Status] FOREIGN KEY([StatusID])
REFERENCES[dbo].[Status]([ID])
GO

ALTER TABLE [dbo].[ItemHistory] CHECK CONSTRAINT[FK_ItemHistory_Status]
GO


