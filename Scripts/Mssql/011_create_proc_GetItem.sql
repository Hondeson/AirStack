USE[AirStack]
GO

/****** Object:  StoredProcedure [dbo].[GetItem]    Script Date: 23.01.2023 21:30:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		HH
-- Create date: 8.12.2022
/* Description:	Vrátí item podle parametrů, pokud díl neexistuje v systému, tak kouká do View PartSource
do externího systému*/
-- =============================================
CREATE PROCEDURE[dbo].[GetItem]


	@ID bigint = null,
	@Code nvarchar(300) = null,
	@ParentCode nvarchar(300) = null,
	@CodeLike nvarchar(300) = null
AS
BEGIN
	SET NOCOUNT ON;

if (@ID is not null and @ID > 0)
	begin
		select top 1 * from Item where ID = @ID


	end



	else if (isnull(@Code, '') <> '')
	begin
		select top 1 * from Item where Code = @Code


	end



	else if (isnull(@ParentCode, '') <> '')
	begin
		declare


			@ItemRow table(Code nvarchar(300), ParentCode nvarchar(300))
		
		--zda existuje ParentCode v systému
		insert into @ItemRow select top 1 Code, ParentCode 
			from Item where ParentCode = @ParentCode

		--když neexistuje, tak ho najdu z pohledu do MES
		if not exists(select top 1 * from @ItemRow)
		begin
			declare
				@ItemID bigint

			insert into @ItemRow select top 1 AirbackCode as Code, PartCode as ParentCode
				from PartSource where PartCode = @ParentCode

			--Nenalezen ani v MES
			if not exists(select top 1 * from @ItemRow)
			begin
				return 1;
end

select top 1 @ItemID = ID from Item as I
				where Code in (select top 1 Code from @ItemRow)

			--Zrovna updatnu ParentCode do systému
			update Item set ParentCode = (select top 1 ParentCode from @ItemRow)
				where ID = @ItemID

			select top 1 * from Item where ID = @ItemID
		end
		else
		begin
			select top 1 * from Item
				where Code in (select top 1 Code from @ItemRow)
		end
	end

	else if(isnull(@CodeLike, '') <> '')
	begin
		select * from Item 
		where Code like CONCAT('%', @CodeLike,'%')
		or @ParentCode like CONCAT('%', @CodeLike,'%')
	end

END
GO


