USE[AirStack]
go
    grant execute on [dbo].[GetItem] to public
    grant execute on[dbo].[GetItemCount] to public
    grant execute on[dbo].[GetItemDTO] to public
    grant execute on[dbo].[ProcessQueueItem] to public
    GRANT EXEC ON TYPE::[dbo].[StringList] TO public