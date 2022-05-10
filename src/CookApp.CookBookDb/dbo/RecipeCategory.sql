CREATE TABLE [dbo].[RecipeCategory]
(
	[RecipeId] INT NOT NULL,
	[CategoryId] INT NOT NULL,
	CONSTRAINT [FK_RecipeCategory_Recipe] FOREIGN KEY ([RecipeId]) REFERENCES [dbo].[Recipe] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_RecipeCategory_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [PK_RecipeCategory] PRIMARY KEY ([RecipeId], [CategoryId])
)
