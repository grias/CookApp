CREATE PROCEDURE [dbo].[spCategory_GetById]
	@CategoryId INT
AS
	SELECT * FROM [dbo].[Category] WHERE [Id] = @CategoryId
