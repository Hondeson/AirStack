USE [AirStack]
GO

/****** Object:  StoredProcedure [dbo].[GetItemDTO]    Script Date: 16.01.2023 15:46:24 ******/
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
	@Fetch bigint = 50,
	@Statuses dbo.StringList readonly,
	@ProductionFrom DateTime = null,
	@ProductionTo DateTime = null
AS
BEGIN
	SET NOCOUNT ON;
    
	SELECT seq.ID, seq.Code, seq.ParentCode
	-- Přeloží ID statusu na proměnnou s příslušným datem
		, MAX(CASE WHEN seq.StatusID = 1 THEN seq.CreatedAt END ) as ProductionDate
		, MAX(CASE WHEN seq.StatusID = 2 THEN seq.CreatedAt END) as TestsDate
		, MAX(CASE WHEN seq.StatusID = 3 THEN seq.CreatedAt END) as DispatchedDate
		, MAX(CASE WHEN seq.StatusID = 4 THEN seq.CreatedAt END) as ComplaintDate
		, MAX(CASE WHEN seq.StatusID = 5 THEN seq.CreatedAt END) as ComplaintToSupplierDate
	FROM (	
		select I.ID, I.Code, I.ParentCode, IH.CreatedAt, S.[Name], s.ID as StatusID from Item I
		left join ItemHistory IH on I.ID = IH.ItemID
		left join [Status] S on S.ID = IH.StatusID
		left join @Statuses SFilter on SFilter.value = S.[Name]
		where
		-- filtr na statusy (pokud nějaký je jinak vše)
			SFilter.value is not null or not exists (select top 1 value from @Statuses) and
		-- filtr na vstup do produkce
			(@ProductionFrom IS NULL OR (IH.CreatedAt >= @ProductionFrom and S.ID = 1)) and
			(@ProductionTo IS NULL OR (IH.CreatedAt <= @ProductionTo and S.ID = 1))
	) as seq
	GROUP BY seq.ID, seq.Code, seq.ParentCode
	order by ID desc 
	OFFSET 
	CASE 
		WHEN @Offset = -1 THEN 0
		ELSE @Offset
	END ROWS
	FETCH NEXT 
	CASE
		WHEN @Fetch = -1 THEN (SELECT COUNT(*) FROM Item)
		ELSE @Fetch
	END ROWS ONLY;
	
END
GO


