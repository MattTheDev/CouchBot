CREATE TABLE [Live].[Platform]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Name] varchar(64) NOT NULL,
	[BaseUrl] varchar(128) NOT NULL,
	[AlertImageUrl] varchar(128) NOT NULL
)
