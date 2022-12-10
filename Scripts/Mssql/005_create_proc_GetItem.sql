SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		HH
-- Create date: 8.12.2022
-- Description:	Vrátí item podle parametrů
-- =============================================
CREATE PROCEDURE [dbo].[GetItem]
	@ID bigint = null,
	@Code nvarchar(300) = null,
	@ParentCode nvarchar(300) = null,
	@CodeLike nvarchar(300) = null
AS
BEGIN
	SET NOCOUNT ON;

	if(@ID is not null and @ID > 0)
	begin
		select top 1 * from Item where ID = @ID
	end

	else if(isnull(@Code, '') <> '')
	begin
		select top 1 * from Item where Code = @Code
	end

	else if(isnull(@ParentCode, '') <> '')
	begin
		select top 1 * from Item where ParentCode = @ParentCode
	end

	else if(isnull(@CodeLike, '') <> '')
	begin
		select * from Item 
		where Code like CONCAT('%',@CodeLike,'%')
		or @ParentCode like CONCAT('%',@CodeLike,'%')
	end

END
GO
