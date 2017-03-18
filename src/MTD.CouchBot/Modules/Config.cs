using System.IO;
using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;
using MTD.CouchBot.Json;
using MTD.CouchBot.Domain;
using System;

namespace MTD.CouchBot.Modules
{     
    [Group("config")]
    public class Config : ModuleBase
    {
        [Command("timezoneoffset"), Summary("Sets servers time zone offset.")]
        public async Task TimeZoneOffset(float offset)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.TimeZoneOffset = offset;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Your Server Time Zone Offset has been set.");
        }

        [Command("textannouncements")]
        public async Task TextAnnouncements(string trueFalse)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            trueFalse = trueFalse.ToLower();
            if (!trueFalse.Equals("true") && !trueFalse.Equals("false"))
            {
                await Context.Channel.SendMessageAsync("Pass true or false when configuring Text Announcements. (ie: !cb config textannouncements true)");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.UseTextAnnouncements = bool.Parse(trueFalse);
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Text announcements has been set to: " + trueFalse);
        }

        [Command("list")]
        public async Task List()
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server != null)
            {
                string whitelist = "";
                
                if(server.BroadcasterWhitelist == null || server.BroadcasterWhitelist.Count < 1)
                {
                    whitelist = "Empty";
                }
                else
                {
                    foreach(var broadcaster in server.BroadcasterWhitelist)
                    {
                        var u = await guild.GetUserAsync(ulong.Parse(broadcaster));

                        whitelist += u.Username + ",";
                    }

                    whitelist = whitelist.TrimEnd(',');
                }

                var announce = await guild.GetChannelAsync(server.AnnouncementsChannel);
                var announceChannel = announce != null ? announce.Name : "Not Set";

                var golive = await guild.GetChannelAsync(server.GoLiveChannel);
                var goliveChannel = golive != null ? golive.Name : "Not Set";

                var greetings = await guild.GetChannelAsync(server.GreetingsChannel);
                var greetingsChannel = greetings != null ? greetings.Name : "Not Set";

                var vod = await guild.GetChannelAsync(server.PublishedChannel);
                var vodChannel = vod != null ? vod.Name : "Not Set";

                string info = "```Markdown\r\n" +
                              "# " + server.Name + " Configuration Settings\r\n" +
                              "- Announcements Channel: " + announceChannel + "\r\n" +
                              "- Go Live Channel: " + goliveChannel + "\r\n" +
                              "- Greetings Channel: " + greetingsChannel + "\r\n" +
                              "- Published/VOD Channel: " + vodChannel + "\r\n" +
                              "- Allow @ Everyone: " + server.AllowEveryone + "\r\n" +
                              "- Allow Thumbnails: " + server.AllowThumbnails + "\r\n" +
                              "- Allow Broadcast Others: " + server.BroadcastOthers + "\r\n" +
                              "- Allow Greetings: " + server.Greetings + "\r\n" +
                              "- Allow Goodbyes: "  + server.Goodbyes + "\r\n" +
                              "- Allow VOD Content: " + server.AllowPublished + "\r\n" +
                              "- Allow Others VOD Content: " + server.AllowPublishedOthers + "\r\n" +
                              "- Use Text Announcements: " + server.UseTextAnnouncements + "\r\n" +
                              "- Use YTG URLS For VOD Content: " + server.UseYouTubeGamingPublished + "\r\n" +
                              "- Use Whitelist: " + server.UseWhitelist + "\r\n" +
                              "- Current Whitelist: " + whitelist + "\r\n" +
                              "- Live Message: " + (string.IsNullOrEmpty(server.LiveMessage) ? "Default" : server.LiveMessage) + "\r\n" +
                              "- Published Message: " + (string.IsNullOrEmpty(server.PublishedMessage) ? "Default" : server.PublishedMessage) + "\r\n" +
                              "- Greeting Message: " + (string.IsNullOrEmpty(server.GreetingMessage) ? "Default" : server.GreetingMessage) + "\r\n" +
                              "- Goodbye Message: " + (string.IsNullOrEmpty(server.GoodbyeMessage) ? "Default" : server.GoodbyeMessage) + "\r\n" +
                              "---- [Unused At The Moment] ----\r\n" +
                              "- Broadcast Sub Goals: " + server.BroadcastSubGoals + "\r\n" +
                              "- Time Zone Offset: " + server.TimeZoneOffset + "\r\n" +
                              "```\r\n";

                await Context.Channel.SendMessageAsync(info);
            }
        }

        [Command("publishedytg"), Summary("Sets www vs gaming in published content urls.")]
        public async Task PublishedYtg(string trueFalse)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            trueFalse = trueFalse.ToLower();
            if (!trueFalse.Equals("true") && !trueFalse.Equals("false"))
            {
                await Context.Channel.SendMessageAsync("Pass true or false when configuring PublishedYTG. (ie: !cb config publishedytg true)");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.UseYouTubeGamingPublished = bool.Parse(trueFalse);
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Publised YTG has been set to: " + trueFalse);
        }

        [Command("deleteoffline"), Summary("Do you want items to be deleted when you go offline?")]
        public async Task DeleteWhenOffline(string trueFalse)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            trueFalse = trueFalse.ToLower();
            if (!trueFalse.Equals("true") && !trueFalse.Equals("false"))
            {
                await Context.Channel.SendMessageAsync("Pass true or false when configuring DeleteOffline. (ie: !cb config deleteoffline true)");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.DeleteWhenOffline = !bool.Parse(trueFalse);
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Delete Offline has been set to: " + trueFalse);
        }

        [Command("mentionrole"), Summary("Set the role to mention instead of Everyone.")]
        public async Task MentionRole(IRole role)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.MentionRole = role.Id;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Mention Role has been set to: " + role.Name);
        }

        [Command("broadcastsubgoals"), Summary("Sets broadcasting of sub goals being met.")]
        public async Task BroadcastSubGoals(string trueFalse)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            trueFalse = trueFalse.ToLower();
            if (!trueFalse.Equals("true") && !trueFalse.Equals("false"))
            {
                await Context.Channel.SendMessageAsync("Pass true or false when configuring BroadcastSubGoals. (ie: !cb config BroadcastSubGoals true)");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.BroadcastSubGoals = bool.Parse(trueFalse);
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Broadcast sub goals has been set to: " + trueFalse);
        }

        #region : Obsolete/Moved : 

        [Obsolete("Moving to Channel Group", false)]
        [Command("announcechannel"), Summary("Sets announcement channel.")]
        public async Task AnnounceChannel(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                //await Context.Channel.SendMessageAsync("You do not have access to this command. Only the server owner does.");

                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guildChannel.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.AnnouncementsChannel = guildChannel.Id;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("The Announce Channel has been set.");
        }

        [Obsolete("Moving to Channel Group", false)]
        [Command("golivechannel"), Summary("Sets go live channel.")]
        public async Task GoLiveChannel(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guildChannel.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.GoLiveChannel = guildChannel.Id;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("The Go Live Channel has been set.");
        }

        [Obsolete("Moving to Channel Group", false)]
        [Command("greetingschannel"), Summary("Sets greetings channel.")]
        public async Task GreetingsChannel(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guildChannel.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.GreetingsChannel = guildChannel.Id;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("The Greetings Channel has been set.");
        }

        [Obsolete("Moving to Channel Group", false)]
        [Command("publishedchannel"), Summary("Sets published video channel.")]
        public async Task PublishedChannel(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guildChannel.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.PublishedChannel = guildChannel.Id;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("The Published Channel has been set.");
        }

        [Obsolete("Moving to Channel Group", false)]
        [Command("clear"), Summary("Clears configuration settings for a guild.")]
        public async Task Clear(string option)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (File.Exists(file))
            {
                option = option.ToLower();
                string label = "";

                switch (option)
                {
                    case "golivechannel":
                        server.GoLiveChannel = 0;
                        label = "Go Live Channel";
                        break;
                    case "announcechannel":
                        server.AnnouncementsChannel = 0;
                        label = "Announcements";
                        break;
                    case "greetingschannel":
                        server.GreetingsChannel = 0;
                        label = "Greetings";
                        break;
                    case "publishedchannel":
                        server.PublishedChannel = 0;
                        label = "Published";
                        break;
                    case "all":
                        server.AnnouncementsChannel = 0;
                        server.GoLiveChannel = 0;
                        server.GreetingsChannel = 0;
                        server.PublishedChannel = 0;
                        label = "All";
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(label))
                {
                    File.WriteAllText(file, JsonConvert.SerializeObject(server));
                    await Context.Channel.SendMessageAsync(label + " settings have been reset.");
                }
            }
        }

        [Obsolete("Deprecating, moving to Allow group.", false)]
        [Command("alloweveryone"), Summary("Sets use of everyone tag")]
        public async Task AllowEveryone(string trueFalse)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;

            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                //await Context.Channel.SendMessageAsync("You do not have access to this command. Only the server owner does.");

                return;
            }

            trueFalse = trueFalse.ToLower();
            if (!trueFalse.Equals("true") && !trueFalse.Equals("false"))
            {
                await Context.Channel.SendMessageAsync("Pass true or false when configuring AllowEveryone. (ie: !cb config AllowEveryone true)");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.AllowEveryone = bool.Parse(trueFalse);
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Allow everyone has been set to: " + trueFalse);
        }

        [Obsolete("Deprecating, moving to Allow group.", false)]
        [Command("allowthumbnails"), Summary("Sets use of thumbnails in announcements.")]
        public async Task AllowThumbnails(string trueFalse)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            trueFalse = trueFalse.ToLower();
            if (!trueFalse.Equals("true") && !trueFalse.Equals("false"))
            {
                await Context.Channel.SendMessageAsync("Pass true or false when configuring AllowThumbnails. (ie: !cb config AllowThumbnails true)");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.AllowThumbnails = bool.Parse(trueFalse);
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Allow thumbnails has been set to: " + trueFalse);
        }

        [Obsolete("Deprecating, moving to Allow group.", false)]
        [Command("allowpublished")]
        public async Task AllowPublished(string trueFalse)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            trueFalse = trueFalse.ToLower();
            if (!trueFalse.Equals("true") && !trueFalse.Equals("false"))
            {
                await Context.Channel.SendMessageAsync("Pass true or false when configuring AllowPublished. (ie: !cb config AllowPublished true)");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.AllowPublished = bool.Parse(trueFalse);
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Allow published has been set to: " + trueFalse);
        }

        [Obsolete("Deprecating, moving to Allow group.", false)]
        [Command("allowpublishedothers")]
        public async Task AllowPublishedOthers(string trueFalse)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            trueFalse = trueFalse.ToLower();
            if (!trueFalse.Equals("true") && !trueFalse.Equals("false"))
            {
                await Context.Channel.SendMessageAsync("Pass true or false when configuring AllowPublishedOthers. (ie: !cb config AllowPublishedOthers true)");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.AllowPublishedOthers = bool.Parse(trueFalse);
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Allow published others has been set to: " + trueFalse);
        }

        [Obsolete("Deprecating, moving to Allow group.", false)]
        [Command("broadcastothers"), Summary("Sets use of everyone tag")]
        public async Task BroadcastOthers(string trueFalse)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            trueFalse = trueFalse.ToLower();
            if (!trueFalse.Equals("true") && !trueFalse.Equals("false"))
            {
                await Context.Channel.SendMessageAsync("Pass true or false when configuring BroadcastOthers. (ie: !cb config BroadcastOthers true)");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.BroadcastOthers = bool.Parse(trueFalse);
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Broadcast others has been set to: " + trueFalse);
        }

        [Obsolete("Moved to Message group.", false)]
        [Command("livemessage")]
        public async Task LiveMessage(string message)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;

            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.LiveMessage = message;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Live Message has been set.");
        }

        [Obsolete("Moved to Message group.", false)]
        [Command("publishedmessage")]
        public async Task PublishedMessage(string message)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;

            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.PublishedMessage = message;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Live Message has been set.");
        }
        #endregion
    }

}
