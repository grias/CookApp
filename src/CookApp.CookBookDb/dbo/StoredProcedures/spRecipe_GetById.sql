CREATE PROCEDURE [dbo].[spRecipe_GetById]
	@RecipeId INT
AS
	SELECT * FROM [dbo].[Recipe] 
	WHERE [Id] = @RecipeId
