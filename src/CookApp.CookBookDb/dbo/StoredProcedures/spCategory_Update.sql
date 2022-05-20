CREATE PROCEDURE [dbo].[spCategory_Update]
	@CategoryId INT,
	@Name NVARCHAR(MAX),
	@Description NVARCHAR(MAX)
AS
BEGIN
	UPDATE [dbo].[Category] 
	SET [Name] = @Name, [Description] = @Description
	WHERE [Id] = @CategoryId
END
