CREATE PROCEDURE [dbo].[spRecipe_AddCategory]
	@RecipeId INT,
	@CategoryId INT
AS
BEGIN
	INSERT INTO RecipeCategory (RecipeId, CategoryId)
	VALUES (@RecipeId, @CategoryId)
END
