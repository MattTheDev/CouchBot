using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            // Not needed. Initially ran to fix new data structure. Keeping just in case needed in future. FixGuilds();

            _discord.JoinedGuild += Client_JoinedGuild;
            _discord.LeftGuild += Client_LeftGuild;
        }
        
        public async Task Client_JoinedGuild(IGuild arg)
        {
            await CreateGuild(arg);
            var owner = await arg.GetOwnerAsync();
            await _loggingService.LogAudit($"Joined guild {arg.Name} ({arg.Id}) owned by {owner.Username} ({owner.Id}).");
        }

        public async Task Client_LeftGuild(IGuild arg)
        {
            File.Delete(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + arg.Id + ".json");
            var owner = await arg.GetOwnerAsync();
            await _loggingService.LogAudit($"Left guild {arg.Name} ({arg.Id}) owned by {owner.Username} ({owner.Id}).");
        }      

        public async Task CreateGuild(IGuild arg)
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
    }
}
