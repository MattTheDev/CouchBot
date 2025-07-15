using Discord.Commands;

namespace CB.Bot.Commands.Text;

public class UtilityTextCommands : ModuleBase
{
    [Command("ping",
        true,
        "Test bot response.",
        RunMode = RunMode.Async)]
    private async Task PingAsync()
    {
        await ReplyAsync("Pong!")
            .ConfigureAwait(false);
    }
}