CREATE PROCEDURE [dbo].[spRecipe_Insert]
	@Name NVARCHAR(MAX),
	@Description NVARCHAR(MAX) = NULL,
	@Process NVARCHAR(MAX) = NULL,
	@Id INT OUTPUT
AS
BEGIN
	INSERT INTO [dbo].[Recipe] ([Name], [Description], [Process])
	VALUES (@Name, @Description, @Process);
	SET @Id = SCOPE_IDENTITY();
END
