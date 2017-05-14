using Discord;
using Discord.Commands;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Json;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Modules
{
    [Group("smashcast")]
    public class Smashcast : ModuleBase
    {
        ISmashcastManager _smashcastManager;

        public Smashcast()
        {
            _smashcastManager = new SmashcastManager();
        }
        
        [Command("add")]
        public async Task Add(string channel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + user.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.ServerHitboxChannels == null)
                server.ServerHitboxChannels = new List<string>();

            if (server.OwnerHitboxChannel.ToLower().Equals(channel.ToLower()))
            {
                await Context.Channel.SendMessageAsync("The channel " + channel + " is configured as the Owner Smashcast channel. " +
                    "Please remove it with the '!cb smashcast resetowner' command and then try re-adding it.");

                return;
            }

            if (!server.ServerHitboxChannels.Contains(channel.ToLower()))
            {
                server.ServerHitboxChannels.Add(channel.ToLower());
                File.WriteAllText(file, JsonConvert.SerializeObject(server));
                await Context.Channel.SendMessageAsync("Added " + channel + " to the server Smashcast streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel + " is already on the server Smashcast streamer list.");
            }
        }

        [Command("remove")]
        public async Task Remove(string channel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + user.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.ServerHitboxChannels == null)
                return;

            if (server.ServerHitboxChannels.Contains(channel.ToLower()))
            {
                server.ServerHitboxChannels.Remove(channel.ToLower());
                File.WriteAllText(file, JsonConvert.SerializeObject(server));
                await Context.Channel.SendMessageAsync("Removed " + channel + " from the server Smashcast streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel + " wasn't on the server Smashcast streamer list.");
            }
        }

        [Command("owner")]
        public async Task Owner(string channel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + user.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.ServerHitboxChannels.Contains(channel.ToLower()))
            {
                await Context.Channel.SendMessageAsync("The channel " + channel + " is in the list of server Smashcast Channels. " +
                    "Please remove it with '!cb smashcast remove " + channel + "' and then retry setting your owner channel.");

                return;
            }

            server.OwnerHitboxChannel = channel;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Owner Smashcast Channel has been set to " + channel + ".");

        }

        [Command("resetowner")]
        public async Task ResetOwner(string channel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + user.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.OwnerHitboxChannel = null;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Owner Smashcast Channel has been reset.");
        }
    }
}
