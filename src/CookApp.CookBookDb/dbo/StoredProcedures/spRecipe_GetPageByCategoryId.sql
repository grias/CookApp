CREATE PROCEDURE [dbo].[spRecipe_GetPageByCategoryId]
	@CategoryId int,
	@PageNumber int = 0,
	@PageSize int = 10
AS
BEGIN
    IF (@PageNumber <= 0)
	BEGIN
		SET @PageNumber = 1;
	END

	IF (@PageSize <= 0)
	BEGIN
		SET @PageSize = 10;
	END
	
	DECLARE @SkipRows INT = (@PageNumber - 1) * @PageSize;

	SELECT Id, Name, Description, Process FROM Recipe
	INNER JOIN RecipeCategory ON Recipe.Id = RecipeCategory.RecipeId
	WHERE RecipeCategory.CategoryId = @CategoryId
	ORDER BY ModifiedDate DESC
	OFFSET @SkipRows ROWS FETCH NEXT @PageSize ROWS ONLY;
END
