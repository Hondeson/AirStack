USE[AirStack]
GO

/****** Object:  Table [dbo].[Status]    Script Date: 02.02.2023 10:47:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Status] (

    [ID][bigint] NOT NULL,

    [Name] [nvarchar] (100) NOT NULL,
 CONSTRAINT[PK_Status] PRIMARY KEY CLUSTERED 
(

    [ID] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY]
) ON[PRIMARY]
GO

insert into [Status] ([ID],[Name]) values
(1, 'Production'), (2, 'Tests'), (3, 'Dispatched'), (4, 'Complaint'), (5, 'ComplaintToSupplier')