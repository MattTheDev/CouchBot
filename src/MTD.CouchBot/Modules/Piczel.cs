using Discord.Commands;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MTD.CouchBot.Managers;

namespace MTD.CouchBot.Modules
{
    [Group("piczel")]
    public class Piczel : BaseModule
    {
        private readonly PiczelManager _piczelManager;
        private readonly BotSettings _botSettings;
        private readonly FileService _fileService;

        public Piczel(IOptions<BotSettings> botSettings, PiczelManager piczelManager, FileService fileService) : base(botSettings, fileService)
        {
            _botSettings = botSettings.Value;
            _piczelManager = piczelManager;
            _fileService = fileService;
        }

        [Command("add")]
        public async Task Add(string name)
        {
            if (!IsAdmin)
            {
                return;
            }

            var piczelChannelId = await _piczelManager.GetUserIdByName(name);

            if (!piczelChannelId.HasValue)
            {
                await Context.Channel.SendMessageAsync($"Piczel Channel {name} does not exist.");

                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server.ServerPiczelChannelIds == null)
            {
                server.ServerPiczelChannelIds = new List<int>();
            }

            if (server.OwnerPiczelChannelId.HasValue && server.OwnerPiczelChannelId.Value == piczelChannelId)
            {
                await Context.Channel.SendMessageAsync($"The channel {name} is configured as the Owner Piczel channel. " +
                    "Please remove it with the '!cb piczel resetowner' command and then try re-adding it.");

                return;
            }

            if (!server.ServerPiczelChannelIds.Contains(piczelChannelId.Value))
            {
                server.ServerPiczelChannelIds.Add(piczelChannelId.Value);
                await _fileService.SaveDiscordServer(server, Context.Guild);

                await Context.Channel.SendMessageAsync($"Added {name} to the server Piczel streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"{name} is already on the server Piczel streamer list.");
            }
        }

        [Command("remove")]
        public async Task Remove(string name)
        {
            if (!IsAdmin)
            {
                return;
            }

            var piczelChannelId = await _piczelManager.GetUserIdByName(name);

            if (!piczelChannelId.HasValue)
            {
                await Context.Channel.SendMessageAsync("Piczel Channel " + name + " does not exist.");

                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server.ServerPiczelChannelIds == null)
            {
                return;
            }

            if (server.ServerPiczelChannelIds.Contains(piczelChannelId.Value))
            {
                server.ServerPiczelChannelIds.Remove(piczelChannelId.Value);
                await _fileService.SaveDiscordServer(server, Context.Guild);

                await Context.Channel.SendMessageAsync("Removed " + name + " from the server Piczel streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(name + " wasn't on the server Piczel streamer list.");
            }
        }

        [Command("owner")]
        public async Task Owner(string name)
        {
            if (!IsAdmin)
            {
                return;
            }

            var piczelChannelId = await _piczelManager.GetUserIdByName(name);

            if (!piczelChannelId.HasValue)
            {
                await Context.Channel.SendMessageAsync("Piczel Channel " + name + " does not exist.");

                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server.ServerPiczelChannelIds != null && server.ServerPiczelChannelIds.Contains(piczelChannelId.Value))
            {
                await Context.Channel.SendMessageAsync("The channel " + name + " is in the list of server Piczel Channels. " +
                    "Please remove it with '!cb Piczel remove " + name + "' and then retry setting your owner channel.");

                return;
            }

            server.OwnerPiczelChannelId = piczelChannelId;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Owner Piczel Channel has been set to " + name + ".");
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

            server.OwnerPiczelChannelId = null;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Owner Piczel Channel has been reset.");
        }
    }
}
