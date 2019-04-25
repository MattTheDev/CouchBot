namespace MTD.CouchBot.Domain.Models.Mixer
{
    public class MixerServerTeam
    {
        public ulong DiscordGuildId { get; set; }
        public MixerUserTeamResponse Team { get; set; }
    }
}