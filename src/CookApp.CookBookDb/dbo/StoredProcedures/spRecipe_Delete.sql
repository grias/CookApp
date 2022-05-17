CREATE PROCEDURE [dbo].[spRecipe_Delete]
	@RecipeId INT
AS
BEGIN
	DELETE FROM [dbo].[Recipe] 
	WHERE [Id] = @RecipeId
END
