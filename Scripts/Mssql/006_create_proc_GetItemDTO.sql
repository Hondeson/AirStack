USE [AirStack]
GO

/****** Object:  StoredProcedure [dbo].[GetItemDTO]    Script Date: 20.01.2023 9:38:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		HH
-- Create date: 19.12.2022
-- Description:	Vrátí záznam o Itemu jako jeden DTO
-- =============================================
CREATE PROCEDURE [dbo].[GetItemDTO]
	@Offset bigint = 0,
	@Fetch bigint = 50,

	@Statuses dbo.StringList readonly,
	@CodeLike nvarchar(300) = null,
	@ParentCodeLike nvarchar(300) = null,

	@ProductionFrom DateTime = null,
	@ProductionTo DateTime = null,
	
	@DispatchedFrom DateTime = null,
	@DispatchedTo DateTime = null,

	@TestsFrom DateTime = null,
	@TestsTo DateTime = null,

	@ComplaintFrom DateTime = null,
	@ComplaintTo DateTime = null,

	@ComplaintSuplFrom DateTime = null,
	@ComplaintSuplTo DateTime = null
AS
BEGIN
	SET NOCOUNT ON;
    
	select *
	FROM itemFilterFunc(
		@Statuses, 
		@CodeLike, @ParentCodeLike, 
		@ProductionFrom, @ProductionTo, 
		@DispatchedFrom, @DispatchedTo,
		@TestsFrom, @TestsTo,
		@ComplaintFrom, @ComplaintTo,
		@ComplaintSuplFrom, @ComplaintSuplTo
	)
	-- vrací od nejnovějšího zápisu
	order by ID desc
	-- pagination, když -1 vrať vše
	OFFSET 
	CASE 
		WHEN @Offset = -1 THEN 0
		ELSE @Offset
	END ROWS
	FETCH NEXT 
	CASE
		WHEN @Fetch = -1 THEN (SELECT COUNT(*) FROM Item)
		ELSE @Fetch
	END ROWS ONLY
END
GO


