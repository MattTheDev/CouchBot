using Discord;
using Discord.Commands;
using MTD.CouchBot.Bot;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

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
            await BotFiles.SaveDiscordServer(server, Context.Guild);
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
            await BotFiles.SaveDiscordServer(server, Context.Guild);
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
                var announce = await guild.GetChannelAsync(server.AnnouncementsChannel);
                var announceChannel = announce != null ? announce.Name : "Not Set";

                var golive = await guild.GetChannelAsync(server.GoLiveChannel);
                var goliveChannel = golive != null ? golive.Name : "Not Set";

                var ownerGolive = await guild.GetChannelAsync(server.OwnerLiveChannel);
                var ownerGoliveChannel = ownerGolive != null ? ownerGolive.Name : "Not Set";

                var greetings = await guild.GetChannelAsync(server.GreetingsChannel);
                var greetingsChannel = greetings != null ? greetings.Name : "Not Set";

                var vod = await guild.GetChannelAsync(server.PublishedChannel);
                var vodChannel = vod != null ? vod.Name : "Not Set";

                var ownerVod = await guild.GetChannelAsync(server.OwnerPublishedChannel);
                var ownerVodChannel = ownerVod != null ? ownerVod.Name : "Not Set";

                var ownerTwitchFeed = await guild.GetChannelAsync(server.OwnerTwitchFeedChannel);
                var ownerTwitchFeedChannel = ownerTwitchFeed != null ? ownerTwitchFeed.Name : "Not Set";

                var role = await DiscordHelper.GetRoleByGuildAndId(server.Id, server.MentionRole);
                var roleName = "";

                if (role == null)
                {
                    server.MentionRole = 0;
                }

                if(server.MentionRole == 0)
                {
                    roleName = "Everyone";
                }
                else if(server.MentionRole == 1)
                {
                    roleName = "Here";
                }
                else
                {
                    roleName = role.Name.Replace("@", "");
                }

                string info = "```Markdown\r\n" +
                              "# " + server.Name + " Configuration Settings\r\n" +
                              "- Owner Go Live Channel: " + ownerGoliveChannel + "\r\n" +
                              "- Owner Published Channel: " + ownerVodChannel + "\r\n" +
                              "- Owner Twitch Channel Feed Channel: " + ownerTwitchFeedChannel + "\r\n" +
                              "- Go Live Channel: " + goliveChannel + "\r\n" +
                              "- Published Channel: " + vodChannel + "\r\n" +
                              "- Greetings Channel: " + greetingsChannel + "\r\n" +
                              "- Allow @ Role: " + server.AllowEveryone + "\r\n" +
                              "- Allow Thumbnails: " + server.AllowThumbnails + "\r\n" +
                              "- Allow Greetings: " + server.Greetings + "\r\n" +
                              "- Allow Goodbyes: "  + server.Goodbyes + "\r\n" +
                              "- Allow Live Content: " + server.AllowLive + "\r\n" +
                              "- Allow Published Content: " + server.AllowPublished + "\r\n" +
                              "- Allow Owner Twitch Channel Feed: " + server.AllowOwnerChannelFeed + "\r\n" +
                              "- Use Text Announcements: " + server.UseTextAnnouncements + "\r\n" +
                              "- Use YTG URLS For VOD Content: " + server.UseYouTubeGamingPublished + "\r\n" +
                              "- Live Message: " + (string.IsNullOrEmpty(server.LiveMessage) ? "Default" : server.LiveMessage) + "\r\n" +
                              "- Published Message: " + (string.IsNullOrEmpty(server.PublishedMessage) ? "Default" : server.PublishedMessage) + "\r\n" +
                              "- Greeting Message: " + (string.IsNullOrEmpty(server.GreetingMessage) ? "Default" : server.GreetingMessage) + "\r\n" +
                              "- Goodbye Message: " + (string.IsNullOrEmpty(server.GoodbyeMessage) ? "Default" : server.GoodbyeMessage) + "\r\n" +
                              "- Stream Offline Message: " + (string.IsNullOrEmpty(server.StreamOfflineMessage) ? "Default" : server.StreamOfflineMessage) + "\r\n" +
                              "- Mention Role: " + roleName + "\r\n" +
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
            await BotFiles.SaveDiscordServer(server, Context.Guild);
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

            server.DeleteWhenOffline = bool.Parse(trueFalse);
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Delete Offline has been set to: " + trueFalse);
        }

        [Command("mentionrole"), Summary("Set the role to mention instead of Everyone.")]
        public async Task MentionRole(string role)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            if(!role.ToLower().Contains("here"))
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.MentionRole = 1;

            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Mention Role has been set to: " + role);
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

            if (role.Name.ToLower().Contains("everyone"))
            {
                server.MentionRole = 0;
            }
            else
            {
                server.MentionRole = role.Id;
            }

            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Mention Role has been set to: " + role.Name);
        }
    }

}
