/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

INSERT INTO [Live].[Platform] VALUES ('Beam', 'https://beam.pro', 'http://couchbot.io/img/alerts/beam.gif')
INSERT INTO [Live].[Platform] VALUES ('YouTube Gaming', 'https://gaming.youtube.com', 'http://couchbot.io/img/alerts/ytg.gif')
INSERT INTO [Live].[Platform] VALUES ('Twitch', 'https://twitch.tv', 'http://couchbot.io/img/alerts/twitch.gif')
INSERT INTO [Live].[Platform] VALUES ('Hitbox', 'https://hitbox.tv', 'http://couchbot.io/img/alerts/hitbox.gif')

INSERT INTO [Live].[NotificationStatus] VALUES ('Pending')
INSERT INTO [Live].[NotificationStatus] VALUES ('Complete')
INSERT INTO [Live].[NotificationStatus] VALUES ('Failed')

