USE[AirStack]
GO

/****** Object:  StoredProcedure [dbo].[GetItemCount]    Script Date: 20.01.2023 9:38:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		HH
-- Create date: 12.01.2023
-- Description: Vrátí celkový počet vyfiltrovaných itemů
-- =============================================
CREATE PROCEDURE [dbo].[GetItemCount]
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

SELECT count(1)

    FROM itemFilterFunc(
        @Statuses,
        @CodeLike, @ParentCodeLike,
        @ProductionFrom, @ProductionTo,
        @DispatchedFrom, @DispatchedTo,
        @TestsFrom, @TestsTo,
        @ComplaintFrom, @ComplaintTo,
        @ComplaintSuplFrom, @ComplaintSuplTo

    )
END
GO