CREATE PROCEDURE [dbo].[spCategory_GetPageByRecipeId]
	@RecipeId int,
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

	SELECT * FROM [Category]
	INNER JOIN [RecipeCategory] ON [Category].[Id] = [RecipeCategory].[CategoryId]
	WHERE [RecipeCategory].[RecipeId] = @RecipeId
	ORDER BY [Name] DESC
	OFFSET @SkipRows ROWS FETCH NEXT @PageSize ROWS ONLY;
END
