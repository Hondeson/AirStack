USE [AirStack]
GO

/****** Object:  UserDefinedFunction [dbo].[itemFilterFunc]    Script Date: 17.01.2023 10:19:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[itemFilterFunc] (
    @Statuses dbo.StringList readonly,
	@ProductionFrom DateTime = null,
	@ProductionTo DateTime = null
)
RETURNS TABLE
AS
RETURN
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
GO