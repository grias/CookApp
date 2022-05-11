CREATE PROCEDURE [dbo].[spRecipe_Update]
	@RecipeId int,
	@Name NVARCHAR(MAX),
	@Description NVARCHAR(MAX) = NULL,
	@Process NVARCHAR(MAX) = NULL
AS
BEGIN
	UPDATE [dbo].[Recipe] 
	SET [Name] = @Name, [Description] = @Description, [Process] = @Process 
	WHERE [Id] = @RecipeId
END
