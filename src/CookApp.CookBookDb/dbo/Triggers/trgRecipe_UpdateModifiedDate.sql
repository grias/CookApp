CREATE TRIGGER [trgRecipe_UpdateModifiedDate]
ON [dbo].[Recipe]
AFTER UPDATE
AS
BEGIN
	UPDATE [dbo].[Recipe]
	SET [ModifiedDate] = CURRENT_TIMESTAMP
	WHERE [Id] = (SELECT Id FROM inserted)
END
