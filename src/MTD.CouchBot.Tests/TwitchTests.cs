using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using System.Threading.Tasks;
using Xunit;

namespace MTD.CouchBot.Tests
{
    public class TwitchTests
    {
        readonly string _testChannel = "CouchBotIsAlive";
        readonly string _testBadChannel = "XxxHorribleChannelNamexxX";
        readonly string _testChannelId = "155539012";
        readonly string _testBadChannelId = "-123456789";
        readonly string _testTeamToken = "ths";
        readonly string _testBadTeamToken = "BADTOKENLAWLELELELEL";
        readonly string _testAlwaysLiveStreamId = "27446517";
        readonly string _testGameName = "League of Legends";

        readonly ITwitchManager _twitchManager;

        public TwitchTests()
        {
            _twitchManager = new TwitchManager();
        }

        [Fact]
        public async Task CanGetTwitchChannelId()
        {
            var response = await _twitchManager.GetTwitchIdByLogin(_testChannel);

            Assert.True(!string.IsNullOrEmpty(response));
        }

        [Fact]
        public async Task CanGetFailedTwitchChannelId()
        {
            var response = await _twitchManager.GetTwitchIdByLogin(_testBadChannel);

            Assert.True(string.IsNullOrEmpty(response));
        }

        [Fact]
        public async Task CanGetChannelFeed()
        {
            var response = await _twitchManager.GetChannelFeedPosts(_testChannelId);

            Assert.True(response != null && response.posts != null);
            Assert.True(response.posts.Count > 0);
        }

        [Fact]
        public async Task CanGetFailedChannelFeed()
        {
            var response = await _twitchManager.GetChannelFeedPosts(_testBadChannelId);

            Assert.True(response.posts.Count == 0);
        }

        [Fact]
        public async Task CanGetDelimitedTwitchMemberList()
        {
            var response = await _twitchManager.GetDelimitedListOfTwitchMemberIds(_testTeamToken);

            Assert.True(!string.IsNullOrEmpty(response));
            Assert.True(response.Split(',').Length > 0);
        }

        [Fact]
        public async Task CanGetFailedDelimitedTwitchMemberList()
        {
            var response = await _twitchManager.GetDelimitedListOfTwitchMemberIds(_testBadTeamToken);

            Assert.True(string.IsNullOrEmpty(response));
        }

        [Fact]
        public async Task CanGetFollowersByName()
        {
            var response = await _twitchManager.GetFollowersByName(_testChannel);

            Assert.True(response.follows.Count > 0);
        }

        [Fact]
        public async Task CanGetFailedFollowersByName()
        {
            var response = await _twitchManager.GetFollowersByName(_testBadChannel);

            Assert.True(response == null);
        }

        [Fact]
        public async Task CanGetStreamById()
        {
            var response = await _twitchManager.GetStreamById(_testAlwaysLiveStreamId);

            Assert.True(response != null && response.stream != null);
            Assert.True(response.stream.channel.display_name.Equals("Monstercat"));
        }

        [Fact]
        public async Task CanGetStreamsByGameName()
        {
            var response = await _twitchManager.GetStreamsByGameName(_testGameName);

            Assert.True(response != null);
            Assert.True(response.Count > 0);
        }

        [Fact]
        public async Task CanGetStreamsByIdList()
        {
            var response = await _twitchManager.GetStreamsByIdList(new System.Collections.Generic.List<string>() { _testAlwaysLiveStreamId });

            Assert.True(response != null && response.streams != null && response.streams.Count > 0);
            Assert.True(response.streams[0].channel.display_name.Equals("Monstercat"));
        }

        [Fact]
        public async Task CanGetTwitchTeamByName()
        {
            var response = await _twitchManager.GetTwitchTeamByName("ths");

            Assert.True(response != null);
            Assert.True(response.DisplayName == "The Hammer Squad");
        }
    }
}
