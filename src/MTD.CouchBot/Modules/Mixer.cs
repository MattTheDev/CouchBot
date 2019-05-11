using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Models.Mixer;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Models.Bot;
using MTD.CouchBot.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("mixer")]
    public class Mixer : BaseModule
    {
        private readonly IMixerManager _mixerManager;
        private readonly DiscordShardedClient _discord;
        private readonly MixerConstellationService _mixerService;
        private readonly MessagingService _messagingService;
        private readonly BotSettings _botSettings;
        private readonly FileService _fileService;

        public Mixer(IMixerManager mixerManager, MessagingService messagingService, DiscordShardedClient discord, MixerConstellationService mixerService,
            IOptions<BotSettings> botSettings, FileService fileService) : base (botSettings, fileService)
        {
            _mixerManager = mixerManager;
            _messagingService = messagingService;
            _discord = discord;
            _mixerService = mixerService;
            _botSettings = botSettings.Value;
            _fileService = fileService;
        }

        [Command("status")]
        public async Task Status()
        {
            await Context.Channel.SendMessageAsync("Current " + _discord.CurrentUser.Username + " Mixer Constellation Connection Status: " + _mixerService.Status());
        }

        [Command("add")]
        public async Task Add(string name)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var channel = await _mixerManager.GetChannelById(name);

            if (channel == null)
            {
                await Context.Channel.SendMessageAsync("The Mixer channel, " + name + ", does not exist.");
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            DiscordServer server = null;

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server == null)
            {
                return;
            }

            if (server.ServerBeamChannelIds == null)
            {
                server.ServerBeamChannelIds = new List<string>();
            }

            if (!string.IsNullOrEmpty(server.OwnerBeamChannelId) && server.OwnerBeamChannelId.Equals(channel.id.ToString()))
            {
                await Context.Channel.SendMessageAsync("The channel " + channel.user.username + " is configured as the Owner Mixer channel. " +
                    "Please remove it with the '!cb mixer resetowner' command and then try re-adding it.");

                return;
            }

            if (!server.ServerBeamChannelIds.Contains(channel.id.ToString()))
            {
                server.ServerBeamChannelIds.Add(channel.id?.ToString());

                if (_botSettings.PlatformSettings.EnableMixer)
                {
                    await _mixerService.SubscribeToLiveAnnouncements(channel.id?.ToString());

                    if(channel.online)
                    {
                        await Announce(channel);
                    }
                }

                await _fileService.SaveDiscordServer(server, Context.Guild);
                await Context.Channel.SendMessageAsync("Added " + channel.user.username + " to the server Mixer streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel.user.username + " is already on the server Mixer streamer list.");
            }
        }

        [Command("remove")]
        public async Task Remove(string name)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var beamChannel = await _mixerManager.GetChannelByName(name);

            if (beamChannel == null)
            {
                await Context.Channel.SendMessageAsync("Mixer Channel " + name + " does not exist.");

                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server.ServerBeamChannelIds == null)
            {
                return;
            }

            if (server.ServerBeamChannelIds.Contains(beamChannel.id?.ToString()))
            {
                server.ServerBeamChannelIds.Remove(beamChannel.id?.ToString());
                await _fileService.SaveDiscordServer(server, Context.Guild);
                await Context.Channel.SendMessageAsync("Removed " + beamChannel.user.username + " from the server Mixer streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(name + " wasn't on the server Mixer streamer list.");
            }
        }

        [Command("owner")]
        public async Task Owner(string channel)
        {
            if (!IsAdmin)
            {
                return;
            }

            var beamChannel = await _mixerManager.GetChannelByName(channel);

            if (beamChannel == null)
            {
                await Context.Channel.SendMessageAsync("Mixer Channel " + channel + " does not exist.");

                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server.ServerBeamChannelIds != null && server.ServerBeamChannelIds.Contains(beamChannel.id?.ToString()))
            {
                await Context.Channel.SendMessageAsync("The channel " + beamChannel.user.username + " is in the list of server Mixer Channels. " +
                    "Please remove it with '!cb mixer remove " + beamChannel.user.username + "' and then retry setting your owner channel.");

                return;
            }

            server.OwnerBeamChannelId = beamChannel.id?.ToString();
            await _mixerService.SubscribeToLiveAnnouncements(beamChannel.id?.ToString());
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Owner Mixer Channel has been set to " + beamChannel.user.username + ".");
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
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            server.OwnerBeamChannelId = null;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Owner Mixer Channel has been reset.");
        }

        public async Task Announce(MixerChannel channel, bool isTeam = false, string team = null)
        {
            if (!IsAdmin)
            {
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (channel.online)
            {
                var gameName = channel.type == null ? "A game" : channel.type.name;
                var url = "http://mixer.com/" + channel.token;
                var avatarUrl = channel.user.avatarUrl != null ? channel.user.avatarUrl : "https://mixer.com/_latest/assets/images/main/avatars/default.jpg";
                var thumbnailUrl = "https://thumbs.mixer.com/channel/" + channel.id + ".small.jpg";
                var channelId = channel.id?.ToString();

                var mixerChannel = await _mixerManager.GetChannelById(channelId);

                var message = _messagingService.BuildMessage(channel.token, gameName, channel.name, url,
                    avatarUrl, thumbnailUrl, Constants.Mixer, channelId, server, server.GoLiveChannel, isTeam ? team : null, false, channel.viewersCurrent, channel.viewersTotal, mixerChannel.numFollowers);
                var sentMessage = await _messagingService.SendMessages(Constants.Mixer, new List<BroadcastMessage>() { message });

                var liveChannel = new LiveChannel()
                {
                    Name = channel.id.ToString(),
                    Servers = new List<ulong>(),
                    ChannelMessages = sentMessage
                };

                File.WriteAllText(
                    _botSettings.DirectorySettings.ConfigRootDirectory +
                    _botSettings.DirectorySettings.LiveDirectory +
                    _botSettings.DirectorySettings.MixerDirectory +
                    channel.id + ".json",
                    JsonConvert.SerializeObject(liveChannel));
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel.token + " is offline.");
            }
        }

        [Command("team")]
        public async Task Team(string token)
        {
            if (!IsApprovedAdmin) return;

            var team = await _mixerManager.GetTeamByToken(token);

            if (team == null)
            {
                await Context.Channel.SendMessageAsync($"Sorry, {token} doesn't appear to be a valid team token. Try again.");
                return;
            }

            var teamUsers = await _mixerManager.GetTeamUsersByTeamId(team.id);

            var builder = new EmbedBuilder();
            var authorBuilder = new EmbedAuthorBuilder();
            var footerBuilder = new EmbedFooterBuilder();

            if (!string.IsNullOrEmpty(team.description))
            {
                builder.Description = Regex.Replace(team.description, "<.*?>", String.Empty);
            }

            builder.ThumbnailUrl = $"{team.logoUrl}?_={Guid.NewGuid().ToString().Replace("-", "")}";

            var amountOfTeams = Math.Ceiling(teamUsers.Count / 75.0);

            if (amountOfTeams > 1)
            {
                for (var i = 0; i < amountOfTeams; i++)
                {
                    var start = 75 * i;
                    var length = 75;

                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if ((i + 1) == amountOfTeams)
                    {
                        length = teamUsers.Count - (i * 75);
                    }

                    builder.Fields.Add(new EmbedFieldBuilder
                    {
                        IsInline = false,
                        Name = "Members",
                        Value = string.Join(", ", teamUsers.GetRange(start, length).Select(x => x.username))
                    });
                }
            }
            else
            {
                builder.Fields.Add(new EmbedFieldBuilder
                {
                    IsInline = false,
                    Name = "Members",
                    Value = string.Join(", ", teamUsers.Select(x => x.username))
                });
            }

            authorBuilder.IconUrl = $"{_discord.CurrentUser.GetAvatarUrl()}?_={Guid.NewGuid().ToString().Replace("-", "")}";
            authorBuilder.Name = _discord.CurrentUser.Username;

            footerBuilder.IconUrl = "http://mattthedev.codes/img/mixer2.png";
            footerBuilder.Text = $"Mixer Team Information for {team.name} ({token})";
            builder.Author = authorBuilder;
            builder.Footer = footerBuilder;

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("addteam")]
        public async Task AddTeam(string token)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var team = await _mixerManager.GetTeamByToken(token);

            if (team == null)
            {
                await Context.Channel.SendMessageAsync(token + " is not a valid Mixer team token. The team token is on the end of the Team URL, ie: (http://mixer.com/team/xbox .. use !cb mixer addteam xbox).");

                return;
            }

            var server = GetServer();

            if (server.MixerTeams == null)
            {
                server.MixerTeams = new List<int>();
            }

            if (!server.MixerTeams.Contains(team.id))
            {
                server.MixerTeams.Add(team.id);
                await _fileService.SaveDiscordServer(server, Context.Guild);

                await Context.Channel.SendMessageAsync("Added " + team.name + " (" + token + ") to the server Mixer team list.");

                var members = await _mixerManager.GetTeamUsersByTeamId(team.id);

                foreach (var member in members)
                {
                    await _mixerService.SubscribeToLiveAnnouncements(member.id.ToString());
                    var channel = await _mixerManager.GetChannelByName(member.username);

                    if (channel.online)
                    {
                        await Announce(channel, true, team.name);
                    }
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync(team.name + " (" + token + ") is already on the server Mixer team list.");
            }
        }

        [Command("removeteam")]
        public async Task RemoveTeam(string token)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var server = GetServer();

            if (server.MixerTeams == null)
            {
                return;
            }

            var team = await _mixerManager.GetTeamByToken(token);

            if (team == null)
            {
                await Context.Channel.SendMessageAsync(token + " is not a valid Mixer team token. The team token is on the end of the Team URL, ie: (http://mixer.com/team/xbox .. use !cb mixer addteam xbox).");

                return;
            }

            if (server.MixerTeams.Contains(team.id))
            {
                server.MixerTeams.Remove(team.id);
                await _fileService.SaveDiscordServer(server, Context.Guild);

                await Context.Channel.SendMessageAsync("Removed " + team.name + " (" + token + ") from the server Mixer team list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Token " + token + " isn't on the server Mixer team list.");
            }
        }

        [Command("listteams")]
        public async Task ListTeams()
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var server = GetServer();

            if (server.MixerTeams == null)
            {
                server.MixerTeams = new List<int>();
            }

            var builder = new StringBuilder();

            foreach (var teamId in server.MixerTeams)
            {
                var team = await _mixerManager.GetTeamById(teamId);

                builder.Append($"{team.name} ({team.token}), ");
            }

            var info = "```Markdown\r\n" +
              "# Server Mixer Teams\r\n" +
              builder.ToString().Trim().TrimEnd(',') + "\r\n" +
              "```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }
    }
}
