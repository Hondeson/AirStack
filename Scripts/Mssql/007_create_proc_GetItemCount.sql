SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		HH
-- Create date: 12.01.2023
-- Description:	Vrátí celkový počet vyfiltrovaných itemů
-- =============================================
CREATE PROCEDURE [dbo].[GetItemCount]
	@ProductionFrom DateTime = null,
	@ProductionTo DateTime = null
AS
BEGIN
	SET NOCOUNT ON;
    
	SELECT count(1)
	FROM (	
		select I.ID, I.Code, I.ParentCode, IH.CreatedAt, S.[Name], s.ID as StatusID from Item I
		left join ItemHistory IH on I.ID = IH.ItemID
		left join [Status] S on S.ID = IH.StatusID
		where 
		--filtr na vstup do produkce
			(@ProductionFrom IS NULL OR (IH.CreatedAt >= @ProductionFrom and S.ID = 1)) and
			(@ProductionTo IS NULL OR (IH.CreatedAt <= @ProductionTo and S.ID = 1))
	) as seq
END
GO
