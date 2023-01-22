USE [AirStack]
GO

/****** Object:  UserDefinedFunction [dbo].[itemFilterFunc]    Script Date: 20.01.2023 9:37:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[itemFilterFunc] (
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
)
RETURNS TABLE
AS
RETURN
	select *
	from (select aDTO.*, 
	-- Doplní ActualStatus do DTO
		CASE 
			WHEN aDTO.ProductionDate = COALESCE(aDTO.ComplaintToSupplierDate,aDTO.TestsDate, aDTO.DispatchedDate, aDTO.ComplaintDate, aDTO.ProductionDate) THEN 'Production' 
            WHEN aDTO.TestsDate = COALESCE(aDTO.ComplaintToSupplierDate,aDTO.TestsDate, aDTO.DispatchedDate, aDTO.ComplaintDate,aDTO.ProductionDate) THEN 'Tests' 
            WHEN aDTO.DispatchedDate = COALESCE(aDTO.ComplaintToSupplierDate,aDTO.TestsDate, aDTO.ComplaintDate,aDTO.DispatchedDate, aDTO.ProductionDate) THEN 'Dispatched' 
			WHEN aDTO.ComplaintDate = COALESCE(aDTO.ComplaintToSupplierDate,aDTO.ComplaintDate,aDTO.TestsDate, aDTO.DispatchedDate, aDTO.ProductionDate) THEN 'Complaint' 
			WHEN aDTO.ComplaintToSupplierDate = COALESCE(aDTO.ComplaintToSupplierDate,aDTO.ComplaintDate,aDTO.TestsDate, aDTO.DispatchedDate, aDTO.ProductionDate) THEN 'ComplaintToSupplier' 
			END AS ActualStatus
	from
	(SELECT seq.ID, seq.Code, seq.ParentCode
	-- Naformátuju na formu DTO, chybí ale ActualStatus
		, MAX(CASE WHEN seq.StatusID = 1 THEN seq.CreatedAt END ) as ProductionDate
		, MAX(CASE WHEN seq.StatusID = 2 THEN seq.CreatedAt END) as TestsDate
		, MAX(CASE WHEN seq.StatusID = 3 THEN seq.CreatedAt END) as DispatchedDate
		, MAX(CASE WHEN seq.StatusID = 4 THEN seq.CreatedAt END) as ComplaintDate
		, MAX(CASE WHEN seq.StatusID = 5 THEN seq.CreatedAt END) as ComplaintToSupplierDate
	FROM (
	-- udělám joint abulek
		select I.ID, I.Code, I.ParentCode, IH.CreatedAt, S.[Name], s.ID as StatusID 
			from Item I
			left join ItemHistory IH on I.ID = IH.ItemID
			left join [Status] S on S.ID = IH.StatusID) as seq
			GROUP BY seq.ID, seq.Code, seq.ParentCode
	) as aDTO
	) as res
	where 
	-- filtr na vstup do produkce
		(@ProductionFrom IS NULL OR res.ProductionDate >= @ProductionFrom) and
		(@ProductionTo IS NULL OR res.ProductionDate <= @ProductionTo) and
	-- filtr na testy
		(@TestsFrom IS NULL OR res.TestsDate >= @TestsFrom) and
		(@TestsTo IS NULL OR res.TestsDate <= @TestsTo) and
	-- filtr na expedici
		(@DispatchedFrom IS NULL OR res.DispatchedDate >= @DispatchedFrom) and
		(@DispatchedTo IS NULL OR res.DispatchedDate <= @DispatchedTo) and
	-- filtr na reklamaci zákazníka
		(@ComplaintFrom IS NULL OR res.ComplaintDate >= @ComplaintFrom) and
		(@ComplaintTo IS NULL OR res.ComplaintDate <= @ComplaintTo) and
	-- filtr na reklamaci dodavateli
		(@ComplaintSuplFrom IS NULL OR res.ComplaintToSupplierDate >= @ComplaintSuplFrom) and
		(@ComplaintSuplTo IS NULL OR res.ComplaintToSupplierDate <= @ComplaintSuplTo) and
	-- filtr na kód dílu (airbagu)
		(@CodeLike IS NULL OR res.Code like CONCAT('%', @CodeLike, '%')) and
	-- filtr na parent kód dílu
		(@ParentCodeLike IS NULL OR res.ParentCode like CONCAT('%', @ParentCodeLike, '%')) and
	-- filtr na status
		(not exists (select top 1 value from @Statuses) or exists (select top 1 value from @Statuses where value = res.ActualStatus))
GO