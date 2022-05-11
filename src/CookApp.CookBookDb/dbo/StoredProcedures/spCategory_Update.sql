CREATE PROCEDURE [dbo].[spCategory_Update]
	@CategoryId INT,
	@Name NVARCHAR(MAX)
AS
BEGIN
	UPDATE [dbo].[Recipe] 
	SET [Name] = @Name
	WHERE [Id] = @CategoryId
END
