SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		HH
-- Create date: 19.12.2022
-- Description:	Vrátí záznam o Itemu jako jeden objekt
-- =============================================
CREATE PROCEDURE [dbo].[GetItemDTO]
	@Offset bigint = 0,
	@Fetch bigint = 50
AS
BEGIN
	SET NOCOUNT ON;
    
	SELECT seq.ID, seq.Code, seq.ParentCode
		, MAX(CASE WHEN seq.StatusID = 1 THEN seq.CreatedAt END ) as ProductionDate
		, MAX(CASE WHEN seq.StatusID = 2 THEN seq.CreatedAt END) as TestsDate
		, MAX(CASE WHEN seq.StatusID = 3 THEN seq.CreatedAt END) as DispatchedDate
		, MAX(CASE WHEN seq.StatusID = 4 THEN seq.CreatedAt END) as ComplaintDate
		, MAX(CASE WHEN seq.StatusID = 5 THEN seq.CreatedAt END) as ComplaintDateToSupplier
	FROM (	
		select I.ID, I.Code, I.ParentCode, IH.CreatedAt, S.[Name], s.ID as StatusID from Item I
		left join ItemHistory IH on I.ID = IH.ItemID
		left join [Status] S on S.ID = IH.StatusID
	) as seq
	GROUP BY seq.ID, seq.Code, seq.ParentCode
	order by ID desc offset @Offset rows fetch next @Fetch rows only

END
GO
