CREATE PROCEDURE [dbo].[spCategory_Delete]
	@CategoryId INT
AS
BEGIN
	DELETE FROM [dbo].[Category] 
	WHERE [Id] = @CategoryId
END
