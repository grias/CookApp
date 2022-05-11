CREATE PROCEDURE [dbo].[spCategory_Insert]
	@Name NVARCHAR(MAX)
AS
BEGIN
	INSERT INTO [dbo].[Category] ([Name])
	VALUES (@Name)
END
