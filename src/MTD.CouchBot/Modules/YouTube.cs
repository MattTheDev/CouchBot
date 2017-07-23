using Discord;
using Discord.Commands;
using MTD.CouchBot.Bot;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using MTD.CouchBot.Models.Bot;
using MTD.CouchBot.Modules;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Modules
{
    [Group("youtube")]
    public class YouTube : BaseModule
    {
        IYouTubeManager _youTubeManager;

        public YouTube()
        {
            _youTubeManager = new YouTubeManager();
        }
        
        [Command("add")]
        public async Task Add(string channelId)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            if (!channelId.ToLower().StartsWith("uc") || channelId.Length != 24)
            {
                await Context.Channel.SendMessageAsync("Incorrect YouTube Channel ID Provided. Channel ID's start with UC and have 24 characters.");
                return;
            }

            var channel = await _youTubeManager.GetYouTubeChannelSnippetById(channelId);

            if(channel == null || channel.items == null || channel.items.Count == 0)
            {
                await Context.Channel.SendMessageAsync("No channel exists with the ID " + channelId + ". You can use the command '!cb ytidlookup <QUERY>' to find the correct ID.");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + Context.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.ServerYouTubeChannelIds == null)
                server.ServerYouTubeChannelIds = new List<string>();

            if (!string.IsNullOrEmpty(server.OwnerYouTubeChannelId) && server.OwnerYouTubeChannelId.Equals(channelId))
            {
                await Context.Channel.SendMessageAsync("The channel " + channelId + " is configured as the Owner YouTube channel. " +
                    "Please remove it with the '!cb youtube resetowner' command and then try re-adding it.");

                return;
            }

            if (!server.ServerYouTubeChannelIds.Contains(channelId))
            {
                server.ServerYouTubeChannelIds.Add(channelId);
                await BotFiles.SaveDiscordServer(server, Context.Guild);
                await Context.Channel.SendMessageAsync("Added " + channelId + " to the server YouTube streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channelId + " is already on the server YouTube streamer list.");
            }
        }

        [Command("remove")]
        public async Task Remove(string channel)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + Context.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.ServerYouTubeChannelIds == null)
                return;

            if (server.ServerYouTubeChannelIds.Contains(channel))
            {
                server.ServerYouTubeChannelIds.Remove(channel);
                await BotFiles.SaveDiscordServer(server, Context.Guild);
                await Context.Channel.SendMessageAsync("Removed " + channel + " from the server YouTube streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel + " wasn't on the server YouTube streamer list.");
            }
        }

        [Command("owner")]
        public async Task Owner(string channelId)
        {
            if (!IsAdmin)
            {
                return;
            }

            if (!channelId.ToLower().StartsWith("uc") || channelId.Length != 24)
            {
                await Context.Channel.SendMessageAsync("Incorrect YouTube Channel ID Provided. Channel ID's start with UC and have 24 characters.");
                return;
            }

            var channel = await _youTubeManager.GetYouTubeChannelSnippetById(channelId);

            if (channel == null || channel.items == null || channel.items.Count == 0)
            {
                await Context.Channel.SendMessageAsync("No channel exists with the ID " + channelId + ". You can use the command '!cb ytidlookup <QUERY>' to find the correct ID.");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + Context.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.ServerYouTubeChannelIds != null && server.ServerYouTubeChannelIds.Contains(channelId.ToLower()))
            {
                await Context.Channel.SendMessageAsync("The channel " + channel + " is in the list of server YouTube Channels. " +
                    "Please remove it with '!cb youtube remove " + channel + "' and then retry setting your owner channel.");

                return;
            }

            server.OwnerYouTubeChannelId = channelId;
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Owner YouTube Channel ID has been set to " + channelId + ".");
        }

        [Command("resetowner")]
        public async Task ResetOwner()
        {
            if (!IsAdmin)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + Context.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.OwnerYouTubeChannelId = null;
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Owner YouTube Channel ID has been reset.");
        }

        [Command("announce")]
        public async Task Announce(string videoId)
        {
            if (!IsAdmin)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + Context.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            var videoResponse = await _youTubeManager.GetVideoById(videoId);

            if (videoResponse == null || videoResponse.items == null || videoResponse.items.Count == 0)
            {
                await Context.Channel.SendMessageAsync("A video with the ID " + videoId + " doesn't exist on YouTube.");

                return;
            }

            var video = videoResponse.items[0];
            var channelData = await _youTubeManager.GetYouTubeChannelSnippetById(video.snippet.channelId);

            string url = "http://" + (server.UseYouTubeGamingPublished ? "gaming" : "www") + ".youtube.com/watch?v=" + videoId;
            string channelTitle = video.snippet.channelTitle;
            string avatarUrl = channelData.items.Count > 0 ? channelData.items[0].snippet.thumbnails.high.url : "";
            string thumbnailUrl = video.snippet.thumbnails.high.url;

            var message = await MessagingHelper.BuildMessage(channelTitle, "a game", video.snippet.title, url, avatarUrl, thumbnailUrl,
                Constants.YouTubeGaming, video.snippet.channelId, server, server.GoLiveChannel, null);
            await MessagingHelper.SendMessages(Constants.YouTube, new List<BroadcastMessage>() { message });
        }

        [Command("preview")]
        public async Task Preview(string videoId)
        {
            await Context.Channel.SendMessageAsync(await _youTubeManager.GetPreviewUrl(videoId));
        }

        [Command("embedpreview")]
        public async Task EmbedPreview(string videoId)
        {
            EmbedBuilder builder = new EmbedBuilder();
            var url = await _youTubeManager.GetPreviewUrl(videoId);
            builder.ImageUrl = url;
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}
