using Discord;
using Discord.Commands;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Json;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("streamer")]
    public class Streamer : ModuleBase
    {
        IYouTubeManager youtubeManager;

        public Streamer()
        {
            youtubeManager = new YouTubeManager();
        }

        [Command("list"), Summary("List server streamers")]
        public async Task List()
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var guildObject = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json"));
            string twitch = "";
            string youtube = "";
            string beam = "";
            string hitbox = "";

            int count = 0;

            if (guildObject.ServerTwitchChannels != null && guildObject.ServerTwitchChannels.Count > 0)
            {
                foreach (var streamer in guildObject.ServerTwitchChannels)
                {
                    if (count == 0)
                    {
                        twitch += streamer;
                    }
                    else
                    {
                        twitch += ", " + streamer;
                    }

                    count++;
                }
            }

            count = 0;

            if (guildObject.ServerYouTubeChannelIds != null && guildObject.ServerYouTubeChannelIds.Count > 0)
            {
                foreach (var streamer in guildObject.ServerYouTubeChannelIds)
                {
                    var channel = await youtubeManager.GetYouTubeChannelSnippetById(streamer);

                    if (count == 0)
                    {
                        youtube += (channel.items.Count > 0 ? channel.items[0].snippet.title + " (" +  streamer + ")" : streamer);
                    }
                    else
                    {
                        youtube += ", " + (channel.items.Count > 0 ? channel.items[0].snippet.title + " (" + streamer + ")" : streamer);
                    }

                    count++;
                }
            }

            count = 0;

            if (guildObject.ServerBeamChannels != null && guildObject.ServerBeamChannels.Count > 0)
            {
                foreach (var streamer in guildObject.ServerBeamChannels)
                {
                    if (count == 0)
                    {
                        beam += streamer;
                    }
                    else
                    {
                        beam += ", " + streamer;
                    }

                    count++;
                }
            }

            if (guildObject.ServerHitboxChannels != null && guildObject.ServerHitboxChannels.Count > 0)
            {
                foreach (var streamer in guildObject.ServerHitboxChannels)
                {
                    if (count == 0)
                    {
                        hitbox += streamer;
                    }
                    else
                    {
                        hitbox += ", " + streamer;
                    }

                    count++;
                }
            }

            string info = "```Markdown\r\n" +
              "# Server Configured Channels\r\n" +
              "- YouTube Gaming: " + youtube + "\r\n" +
              "- Twitch: " + twitch + "\r\n" +
              "- Beam: " + beam + "\r\n" +
              "- Hitbox: " + hitbox + "\r\n" +
              "```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }
    }
}
