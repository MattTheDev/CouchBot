using Discord.WebSocket;

namespace CB.Bot.Services;

public class GuildInteractionService(DiscordSocketClient discordSocketClient)
{
    public void Init()
    {
        discordSocketClient.JoinedGuild += JoinedGuild;
    }

    private Task JoinedGuild(SocketGuild arg)
    {
        throw new NotImplementedException();
    }
}