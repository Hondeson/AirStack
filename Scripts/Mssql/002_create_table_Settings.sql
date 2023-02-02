USE[AirStack]
GO

/****** Object:  Table [dbo].[Settings]    Script Date: 09.12.2022 11:48:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Settings] (

    [ID][bigint] IDENTITY(1, 1) NOT NULL,

    [Name] [nvarchar] (300) NOT NULL,

    [Value] [nvarchar] (500) NOT NULL,
 CONSTRAINT[PK_Settings] PRIMARY KEY CLUSTERED 
(

    [ID] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
) ON[PRIMARY]
GO

insert into Settings ([Name], [Value]) values
('CodeRegex_1', '^.{0,1}FM.*$')