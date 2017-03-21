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
    // Create a module with the 'sample' prefix
    [Group("live")]
    public class Live : ModuleBase
    {
        ITwitchManager twitchManager;
        IYouTubeManager youtubeManager;
        IBeamManager _beamManager;

        public Live()
        {
            twitchManager = new TwitchManager();
            youtubeManager = new YouTubeManager();
            _beamManager = new BeamManager();
        }

        [Command("check"), Summary("Sets twitch channel.")]
        public async Task Check()
        {
            string file = Constants.ConfigRootDirectory + Constants.UserDirectory + Context.Message.Author.Id + ".json";

            var user = new User();

            if (File.Exists(file))
            {
                user = JsonConvert.DeserializeObject<User>(File.ReadAllText(file));
                var channel = await youtubeManager.GetYouTubeChannelSnippetById(user.YouTubeChannelId);

                string info = "```Markdown\r\n" +
                              "# Your Settings\r\n" +
                              "- YouTube: " + (channel.items.Count > 0 ? channel.items[0].snippet.title + " (" + user.YouTubeChannelId + ")" : user.YouTubeChannelId) + "\r\n" +
                              "- Twitch: " + user.TwitchName + "\r\n" +
                              "- Beam: " + user.BeamName +  "\r\n" +
                              "- Hitbox: " + user.HitboxName + "\r\n" +
                              "```\r\n";

                await Context.Channel.SendMessageAsync(info);
            }
        }

        [Command("twitch"), Summary("Sets twitch channel.")]
        public async Task Twitch(string channel)
        {
            string file = Constants.ConfigRootDirectory + Constants.UserDirectory + Context.Message.Author.Id + ".json";

            var user = new User();

            if (File.Exists(file))
                user = JsonConvert.DeserializeObject<User>(File.ReadAllText(file));

            user.Id = Context.Message.Author.Id;
            user.TwitchName = channel;
            user.TwitchId = await twitchManager.GetTwitchIdByLogin(user.TwitchName);
            File.WriteAllText(file, JsonConvert.SerializeObject(user));

            await Context.Channel.SendMessageAsync("Your Twitch channel has been set.");
        }

        [Command("hitbox"), Summary("Sets Hitbox channel.")]
        public async Task Hitbox(string channel)
        {
            string file = Constants.ConfigRootDirectory + Constants.UserDirectory + Context.Message.Author.Id + ".json";

            var user = new User();

            if (File.Exists(file))
                user = JsonConvert.DeserializeObject<User>(File.ReadAllText(file));

            user.Id = Context.Message.Author.Id;
            user.HitboxName = channel;
            File.WriteAllText(file, JsonConvert.SerializeObject(user));

            await Context.Channel.SendMessageAsync("Your Hitbox channel has been set.");
        }

        [Command("twitchgoal"), Summary("Sets a twitch follower goal.")]
        public async Task TwitchGoal(string goal)
        {
            string file = Constants.ConfigRootDirectory + Constants.UserDirectory + Context.Message.Author.Id + ".json";

            var user = new User();

            if (File.Exists(file))
                user = JsonConvert.DeserializeObject<User>(File.ReadAllText(file));

            user.Id = Context.Message.Author.Id;
            user.TwitchFollowerGoal = goal;
            user.TwitchFollowerGoalMet = false;
            File.WriteAllText(file, JsonConvert.SerializeObject(user));
            await Context.Channel.SendMessageAsync("Your Twitch Follower Goal has been set.");
        }

        [Command("clear"), Summary("Sets twitch channel.")]
        public async Task Clear(string platform)
        {
            string file = Constants.ConfigRootDirectory + Constants.UserDirectory + Context.Message.Author.Id + ".json";

            var user = new User();

            if (File.Exists(file))
            {
                user = JsonConvert.DeserializeObject<User>(File.ReadAllText(file));

                platform = platform.ToLower();
                string label = "";

                switch (platform)
                {
                    case "twitch":
                        user.TwitchName = null;
                        user.TwitchId = null;
                        label = "Twitch";
                        break;
                    case "youtube":
                        user.YouTubeChannelId = null;
                        label = "YouTube";
                        break;
                    case "beam":
                        await Program.beamClient.UnsubscribeFromLiveAnnouncements(user.BeamId);
                        user.BeamName = null;
                        user.BeamId = null;
                        label = "Beam";
                        break;
                    case "hitbox":
                        user.HitboxName = null;
                        label = "Hitbox";
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(label))
                {
                    File.WriteAllText(file, JsonConvert.SerializeObject(user));
                    await Context.Channel.SendMessageAsync("Your " + label + " settings have been reset.");
                }
            }
        }

        [Command("youtube"), Summary("Sets youtube channel.")]
        public async Task YouTube(string channel)
        {
            if(!channel.ToLower().StartsWith("uc") || channel.Length != 24)
            {
                await Context.Channel.SendMessageAsync("Incorrect YouTube Channel ID Provided. For your Channel ID, Please visit: <https://www.youtube.com/account_advanced>");
                return;
            }

            string file = Constants.ConfigRootDirectory + Constants.UserDirectory + Context.Message.Author.Id + ".json";

            var user = new User();

            if (File.Exists(file))
                user = JsonConvert.DeserializeObject<User>(File.ReadAllText(file));

            user.Id = Context.Message.Author.Id;
            user.YouTubeChannelId = channel;
            File.WriteAllText(file, JsonConvert.SerializeObject(user));
            await Context.Channel.SendMessageAsync("Your YouTube Channel has been set.");
        }

        [Command("youtubegoal"), Summary("Sets a youtube sub goal.")]
        public async Task YouTubeGoal(string goal)
        {
            string file = Constants.ConfigRootDirectory + Constants.UserDirectory + Context.Message.Author.Id + ".json";

            var user = new User();

            if (File.Exists(file))
                user = JsonConvert.DeserializeObject<User>(File.ReadAllText(file));

            user.Id = Context.Message.Author.Id;
            user.YouTubeSubGoal = goal;
            user.YouTubeSubGoalMet = false;
            File.WriteAllText(file, JsonConvert.SerializeObject(user));
            await Context.Channel.SendMessageAsync("Your YouTub Sub Goal has been set.");
        }

        [Command("beam"), Summary("Sets beam channel.")]
        public async Task Beam(string channel)
        {
            string file = Constants.ConfigRootDirectory + Constants.UserDirectory + Context.Message.Author.Id + ".json";

            var user = new User();

            if (File.Exists(file))
                user = JsonConvert.DeserializeObject<User>(File.ReadAllText(file));

            var beamChannel = await _beamManager.GetBeamChannelByName(channel);

            if(beamChannel == null)
            {
                await Context.Channel.SendMessageAsync(channel + " is an invalid Beam channel.");
                return;
            }

            await Program.beamClient.SubscribeToLiveAnnouncements(beamChannel.id.Value.ToString());

            user.Id = Context.Message.Author.Id;
            user.BeamName = channel;
            user.BeamId = beamChannel.id.Value.ToString();
            File.WriteAllText(file, JsonConvert.SerializeObject(user));
            await Context.Channel.SendMessageAsync("Your Beam Channel has been set.");
        }

        [Command("beamgoal"), Summary("Sets a Beam follower goal.")]
        public async Task BeamGoal(string goal)
        {
            string file = Constants.ConfigRootDirectory + Constants.UserDirectory + Context.Message.Author.Id + ".json";

            var user = new User();

            if (File.Exists(file))
                user = JsonConvert.DeserializeObject<User>(File.ReadAllText(file));

            user.Id = Context.Message.Author.Id;
            user.BeamFollowerGoal = goal;
            user.BeamFollowerGoalMet = false;
            File.WriteAllText(file, JsonConvert.SerializeObject(user));
            await Context.Channel.SendMessageAsync("Your Beam Follower Goal has been set.");
        }
    }

}
