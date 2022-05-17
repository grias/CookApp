CREATE PROCEDURE [dbo].[spRecipe_RemoveCategory]
	@RecipeId INT,
	@CategoryId INT
AS
BEGIN
	DELETE FROM [dbo].[RecipeCategory]
	WHERE [RecipeId] = @RecipeId
	AND [CategoryId] = @CategoryId
END
