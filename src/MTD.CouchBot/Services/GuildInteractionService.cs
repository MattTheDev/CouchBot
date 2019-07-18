using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Services
{
    public class GuildInteractionService
    {
        private readonly DiscordShardedClient _discord;
        private readonly BotSettings _botSettings;
        private readonly FileService _fileService;
        private readonly LoggingService _loggingService;

        public GuildInteractionService(DiscordShardedClient discord, IOptions<BotSettings> botSettings, FileService fileService,
            LoggingService loggingService)
        {
            _discord = discord;
            _botSettings = botSettings.Value;
            _fileService = fileService;
            _loggingService = loggingService;
        }

        public void Init()
        {
            _discord.JoinedGuild += Client_JoinedGuild;
            _discord.LeftGuild += Client_LeftGuild;
            _discord.UserJoined += Client_UserJoined;
            _discord.UserLeft += Client_UserLeft;
        }

        private async Task Client_JoinedGuild(IGuild arg)
        {
            await CreateGuild(arg);
            var channel = (IMessageChannel) _discord.GetChannel(_botSettings.BotConfig.JoinAndLeaveFeedChannelId);

            await channel.SendMessageAsync("", false, await GetJoinLeftEmbed(arg, true));
        }

        private async Task Client_LeftGuild(IGuild arg)
        {
            File.Delete(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + arg.Id + ".json");
            var channel = (IMessageChannel)_discord.GetChannel(_botSettings.BotConfig.JoinAndLeaveFeedChannelId);

            await channel.SendMessageAsync("", false, await GetJoinLeftEmbed(arg, false));
        }

        private async Task Client_UserLeft(IGuildUser arg)
        {
            var guild = new DiscordServer();
            var guildFile = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + arg.Guild.Id + ".json";

            if (File.Exists(guildFile))
            {
                var json = File.ReadAllText(guildFile);
                guild = JsonConvert.DeserializeObject<DiscordServer>(json);
            }

            if (guild != null)
            {
                if (guild.GreetingsChannel != 0 && guild.Goodbyes)
                {
                    var channel = (IMessageChannel)await arg.Guild.GetChannelAsync(guild.GreetingsChannel);

                    if (string.IsNullOrEmpty(guild.GoodbyeMessage))
                    {
                        guild.GoodbyeMessage = "Good bye, " + arg.Username + ", thanks for hanging out!";
                    }

                    var name = "";
                    if (guild.GreetingMessage.Contains("%RANDOMUSER%"))
                    {
                        var users = (await arg.Guild.GetUsersAsync(CacheMode.AllowDownload));

                        var random = new Random().Next(0, users.Count - 1);

                        name = users.ElementAt(random).Username;
                    }

                    guild.GoodbyeMessage = guild.GoodbyeMessage.Replace("%USER%", arg.Username).Replace("%NEWLINE%", "\r\n").Replace("%RANDOMUSER%", name);

                    await channel.SendMessageAsync(guild.GoodbyeMessage);
                }
            }
        }

        private async Task Client_UserJoined(IGuildUser arg)
        {
            var guild = new DiscordServer();
            var guildFile = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + arg.Guild.Id + ".json";

            if (File.Exists(guildFile))
            {
                var json = File.ReadAllText(guildFile);
                guild = JsonConvert.DeserializeObject<DiscordServer>(json);
            }

            if (guild != null)
            {
                if (guild.GreetingsChannel != 0 && guild.Greetings)
                {
                    var channel = (IMessageChannel)await arg.Guild.GetChannelAsync(guild.GreetingsChannel);

                    if (string.IsNullOrEmpty(guild.GreetingMessage))
                    {
                        guild.GreetingMessage = "Welcome to the server, " + arg.Mention;
                    }

                    var name = "";
                    if (guild.GreetingMessage.Contains("%RANDOMUSER%"))
                    {
                        var users = (await arg.Guild.GetUsersAsync(CacheMode.AllowDownload));

                        var random = new Random().Next(0, users.Count - 1);

                        name = users.ElementAt(random).Username;
                    }

                    guild.GreetingMessage = guild.GreetingMessage.Replace("%USER%", arg.Mention).Replace("%NEWLINE%", "\r\n").Replace("%RANDOMUSER%", name);

                    await channel.SendMessageAsync(guild.GreetingMessage);
                }

                if (guild.JoinRole != 0)
                {
                    var discordGuild = _discord.GetGuild(guild.Id);

                    var role = discordGuild?.GetRole(guild.JoinRole);
                    if (role == null)
                    {
                        return;
                    }

                    await arg.AddRoleAsync(role);
                }
            }
        }

        private async Task CreateGuild(IGuild arg)
        {
            var guild = new DiscordServer();
            var guildFile = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + arg.Id + ".json";

            if (File.Exists(guildFile))
            {
                var json = File.ReadAllText(guildFile);
                guild = JsonConvert.DeserializeObject<DiscordServer>(json);
            }

            var owner = await arg.GetUserAsync(arg.OwnerId);
            guild.Id = arg.Id;
            guild.OwnerId = arg.OwnerId;
            guild.OwnerName = owner.Username;
            guild.Name = arg.Name;
            guild.AllowEveryone = true;

            var guildJson = JsonConvert.SerializeObject(guild);
            File.WriteAllText(guildFile, guildJson);
        }

        public async Task CheckGuildConfigurations()
        {
            var files = _fileService.GetConfiguredServerPaths();
            var badConfigurations = new List<DiscordServer>();

            foreach (var file in files)
            {
                var path = Path.GetFileNameWithoutExtension(file);
                try
                {
                    var server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

                    if (server.Id != ulong.Parse(path))
                    {
                        _loggingService.LogInfo("Bad Configuration Found: " + path);

                        var guild = _discord.GetGuild(ulong.Parse(path));

                        if (guild == null)
                        {
                            continue;
                        }

                        var guildOwner = _discord.GetUser(guild.OwnerId);

                        server.Id = guild.Id;
                        server.Name = guild.Name;
                        server.OwnerId = guild.OwnerId;
                        server.OwnerName = guildOwner == null ? "" : guildOwner.Username;

                        _fileService.SaveDiscordServer(server);

                        _loggingService.LogInfo("Server Configuration Fixed: " + path);
                    }
                }
                catch (Exception ex)
                {
                    await _loggingService.LogError("Error in CheckGuildConfigurations: " + ex.Message);
                }
            }
        }
        
        public async Task AddRole(IGuildUser user, IRole guildRole, IMessageChannel channel)
        {
            try
            {
                await user.AddRoleAsync(guildRole);
                await channel.SendMessageAsync(
                    $"You've been successfully added to the {guildRole.Name} role.");
            }
            catch (Exception)
            {
                await channel.SendMessageAsync(
                    "I was unable to add you to the role successfully. Check my permissions, and try again.");
            }
        }

        public async Task RemoveRole(IGuildUser user, IRole guildRole, IMessageChannel channel)
        {
            try
            {
                await user.RemoveRoleAsync(guildRole);
                await channel.SendMessageAsync(
                    $"You've been successfully removed from the {guildRole.Name} role.");
            }
            catch (Exception)
            {
                await channel.SendMessageAsync(
                    "I was unable to remove you from the role successfully. Check my permissions, and try again.");
            }
        }

        public async Task<Embed> GetJoinLeftEmbed(IGuild guild, bool isJoining)
        {
            var random = new Random();

            var owner = await guild.GetOwnerAsync();
            var users = await guild.GetUsersAsync();

            var embedBuilder = new EmbedBuilder();
            var footerBuilder = new EmbedFooterBuilder();
            var authorBuilder = new EmbedAuthorBuilder();

            authorBuilder.Name = isJoining ? "Joined a New Server" : "Left a Server";

            embedBuilder.Fields.Add(
                new EmbedFieldBuilder
                {
                    IsInline = true,
                    Name = "Name",
                    Value = guild.Name
                }
            );

            embedBuilder.Fields.Add(
                new EmbedFieldBuilder
                {
                    IsInline = true,
                    Name = "Owner",
                    Value = owner.Nickname ?? owner.Username
                }
            );

            embedBuilder.Fields.Add(
                new EmbedFieldBuilder
                {
                    IsInline = true,
                    Name = "Users",
                    Value = users.Count(x => !x.IsBot)
                }
            );

            embedBuilder.Fields.Add(
                new EmbedFieldBuilder
                {
                    IsInline = true,
                    Name = "Bots",
                    Value = users.Count(x => x.IsBot)
                }
            );

            embedBuilder.Color = new Color(
                random.Next(1, 256),
                random.Next(1, 256),
                random.Next(1, 256));

            embedBuilder.ThumbnailUrl = guild.IconUrl ?? _discord.CurrentUser.GetAvatarUrl();

            footerBuilder.Text = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

            embedBuilder.Author = authorBuilder;
            embedBuilder.Footer = footerBuilder;

            return embedBuilder.Build();
        }
    }
}
