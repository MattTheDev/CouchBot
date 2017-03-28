using Discord;
using Discord.Commands;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Json;
using Newtonsoft.Json;
using System;
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

            server.DeleteWhenOffline = bool.Parse(trueFalse);
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
    }

}
