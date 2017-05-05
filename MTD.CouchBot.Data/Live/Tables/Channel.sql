CREATE TABLE [Live].[Channel]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Name] varchar(64) NOT NULL,
	[PlatformId] int NOT NULL,
	[NotificationStatusId] int NOT NULL,
	[CreateDate] datetime NOT NULL
	CONSTRAINT [FK_ChannelNotificationStatus] FOREIGN KEY (NotificationStatusId) REFERENCES [Live].[NotificationStatus] ([Id])
	CONSTRAINT [FK_ChannelPlatform] FOREIGN KEY (PlatformId) REFERENCES [Live].[Platform] ([Id])
)
