using Discord.Interactions;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace CB.Bot.Commands.Application;

public class UtilityApplicationCommands : InteractionModuleBase
{
    [SlashCommand("ping",
        "Test bot response.",
        true,
        RunMode.Async)]
    private async Task PingAsync()
    {
        await RespondAsync("Pong!")
            .ConfigureAwait(false);
    }
}