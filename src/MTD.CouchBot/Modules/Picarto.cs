using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("picarto")]
    public class Picarto : BaseModule
    {
        private readonly IPicartoManager _picartoManager;
        private readonly BotSettings _botSettings;
        private readonly FileService _fileService;
        private readonly StringService _stringService;

        public Picarto(IPicartoManager picartoManager, IOptions<BotSettings> botSettings, FileService fileService, StringService stringService) 
            : base(botSettings, fileService)
        {
            _picartoManager = picartoManager;
            _botSettings = botSettings.Value;
            _fileService = fileService;
            _stringService = stringService;
        }
        
        [Command("add")]
        public async Task Add(string channelName)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var channel = await _picartoManager.GetChannelByName(channelName);

            if (channel == null)
            {
                await Context.Channel.SendMessageAsync("The Picarto channel, " + channelName + ", does not exist.");
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.PicartoChannels == null)
                server.PicartoChannels = new List<string>();

            if (!string.IsNullOrEmpty(server.OwnerPicartoChannel) && server.OwnerPicartoChannel.ToLower().Equals(channelName.ToLower()))
            {
                await Context.Channel.SendMessageAsync("The channel " + channelName + " is configured as the Owner Picarto channel. " +
                    "Please remove it with the '!cb picarto resetowner' command and then try re-adding it.");

                return;
            }

            if (!server.PicartoChannels.Contains(channelName.ToLower()))
            {
                server.PicartoChannels.Add(channelName.ToLower());
                await _fileService.SaveDiscordServer(server, Context.Guild);
                await Context.Channel.SendMessageAsync("Added " + channelName + " to the server Picarto streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channelName + " is already on the server Picarto streamer list.");
            }
        }

        [Command("remove")]
        public async Task Remove(string channel)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.PicartoChannels == null)
                return;

            if (server.PicartoChannels.Contains(channel.ToLower()))
            {
                server.PicartoChannels.Remove(channel.ToLower());
                await _fileService.SaveDiscordServer(server, Context.Guild);
                await Context.Channel.SendMessageAsync("Removed " + channel + " from the server Picarto streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel + " wasn't on the server Picarto streamer list.");
            }
        }

        [Command("owner")]
        public async Task Owner(string channelName)
        {
            if (!IsAdmin)
            {
                return;
            }

            var channel = await _picartoManager.GetChannelByName(channelName);

            if (channel == null)
            {
                await Context.Channel.SendMessageAsync("The Picarto channel, " + channelName + ", does not exist.");
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.PicartoChannels != null && server.PicartoChannels.Contains(channelName.ToLower()))
            {
                await Context.Channel.SendMessageAsync("The channel " + channelName + " is in the list of server Picarto Channels. " +
                    "Please remove it with '!cb picarto remove " + channelName + "' and then retry setting your owner channel.");

                return;
            }

            server.OwnerPicartoChannel = channelName;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Owner Picarto Channel has been set to " + channelName + ".");
        }

        [Command("resetowner")]
        public async Task ResetOwner()
        {
            if (!IsAdmin)
            {
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.OwnerPicartoChannel = null;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Owner Picarto Channel has been reset.");
        }

        [Command("announce")]
        public async Task Announce(string channelName)
        {
            if (!IsAdmin)
            {
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.LiveMessage == null)
            {
                server.LiveMessage = "%CHANNEL% just went live - %TITLE% - %URL%";
            }

            var stream = await _picartoManager.GetChannelByName(channelName);

            if (stream == null)
            {
                await Context.Channel.SendMessageAsync(channelName + " doesn't exist on Picarto.");

                return;
            }

            if (stream.Online)
            {
                var embedBuilder = new EmbedBuilder();
                var author = new EmbedAuthorBuilder();
                var footer = new EmbedFooterBuilder();

                author.IconUrl = "https://picarto.tv/user_data/usrimg/" + stream.Name.ToLower() + "/dsdefault.jpg";
                author.Name = stream.Name;
                author.Url = "https://picarto.tv/" + stream.Name;
                embedBuilder.Author = author;

                footer.IconUrl = "https://picarto.tv/images/Picarto_logo.png";
                footer.Text = "[Picarto] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                embedBuilder.Footer = footer;

                embedBuilder.Title = stream.Name + " has gone live!";
                embedBuilder.Color = new Color(192, 192, 192);
                embedBuilder.ThumbnailUrl = "https://picarto.tv/user_data/usrimg/" + stream.Name.ToLower() + "/dsdefault.jpg";
                embedBuilder.ImageUrl = "https://thumb.picarto.tv/thumbnail/" + stream.Name + ".jpg";

                embedBuilder.Description = _stringService.AnnouncementText(server.LiveMessage, stream.Name, stream.Title, $"https://picarto.tv/{stream.Name}", "");

                embedBuilder.AddField(f =>
                {
                    f.Name = "Category";
                    f.Value = stream.Category;
                    f.IsInline = true;
                });

                embedBuilder.AddField(f =>
                {
                    f.Name = "Adult Stream?";
                    f.Value = stream.Adult ? "Yup!" : "Nope!";
                    f.IsInline = true;
                });

                embedBuilder.AddField(f =>
                {
                    f.Name = "Total Viewers";
                    f.Value = stream.ViewersTotal;
                    f.IsInline = true;
                });

                embedBuilder.AddField(f =>
                {
                    f.Name = "Total Followers";
                    f.Value = stream.Followers;
                    f.IsInline = true;
                });

                var tags = "";
                foreach(var t in stream.Tags)
                {
                    tags += t + ", ";
                }

                embedBuilder.AddField(f =>
                {
                    f.Name = "Stream Tags";
                    f.Value = tags.Trim().TrimEnd(',');
                    f.IsInline = false;
                });

                await Context.Channel.SendMessageAsync("", false, embedBuilder.Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync(channelName + " is offline.");
            }
        }
    }
}
