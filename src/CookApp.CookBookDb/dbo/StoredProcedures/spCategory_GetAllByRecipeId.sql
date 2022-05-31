CREATE PROCEDURE [dbo].[spCategory_GetAllByRecipeId]
	@RecipeId int
AS
	SELECT [Id], [Name], [Description] FROM [dbo].[Category]
	INNER JOIN [dbo].[RecipeCategory] ON [dbo].[Category].[Id] = [dbo].[RecipeCategory].[CategoryId]
	WHERE [dbo].[RecipeCategory].[RecipeId] = @RecipeId