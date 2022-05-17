CREATE PROCEDURE [dbo].[spCategory_Insert]
	@Name NVARCHAR(MAX),
	@Description NVARCHAR(MAX) = NULL,
	@Id INT OUTPUT
AS
BEGIN
	INSERT INTO [dbo].[Category] ([Name], [Description])
	VALUES (@Name, @Description);
	SET @Id = SCOPE_IDENTITY()
END
