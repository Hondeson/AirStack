USE [AirStack]
GO

/****** Object:  StoredProcedure [dbo].[ProcessQueueItem]    Script Date: 25.01.2023 18:06:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		HH
-- Create date: 25.01.2022
-- Description:	Zpracuje queue item po 100 itemech
-- =============================================
CREATE PROCEDURE [dbo].[ProcessQueueItem]
AS
BEGIN
	SET NOCOUNT ON;

	declare 
		@i int = 100,
		@Count int;

	-- zjistím počet v queue
	select @Count = count(*) from ItemHistoryQueue

	-- pokud je počet menší jak 100, tak nastavím na danou hodnotu
	if @Count <= @i
	begin
		set @i = @Count
	end

	while @i > 0
	begin
		-- fronta je empty tak return
		if not exists (select top 1 * from ItemHistoryQueue)
		begin
			return;
		end

		declare 
			@ID bigint,
			@Code nvarchar(300),
			@ParentCode nvarchar(300),
			@StatusID bigint,
			@CreatedAt datetime

		-- vezmu 1. item z fronty
		select top 1 @ID = ID, 
			@Code = ItemCode, @ParentCode = ParentCode, 
			@StatusID = StatusID, @CreatedAt = CreatedAt
		from ItemHistoryQueue

		-- podle kódu si najdu ID itemu
		declare @ItemID bigint = 0;
		select @ItemID = ID from Item where Code = @Code

		-- našel jsem item, tak updatuju jeho parent kód
		if (@ItemID > 0)
		begin
			update Item set ParentCode = @ParentCode
			where ID = @ItemID
		end

		-- vytvoření záznamu do histore
		insert into ItemHistory 
		(ItemID, StatusID, CreatedAt) values
		(@ItemID, @StatusID, @CreatedAt)

		-- oddělám item z fronty
		delete from ItemHistoryQueue
		where ID = @ID 

		set @i = @i - 1;
	end -- while

END
GO