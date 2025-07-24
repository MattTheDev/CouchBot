using System.Security.Cryptography;
using CB.Accessors.Contracts;
using Discord.Interactions;

namespace CB.Bot.Commands.Application;

public class FunSlashCommands(IFunAccessor funAccessor) : InteractionModuleBase
{
    [SlashCommand("roll", "Roll a D20")]
    public async Task RollAsync()
    {
        var rollResult = RandomNumberGenerator.GetInt32(1, 21);
        await RespondAsync($"You rolled a {rollResult}!");  
    }

    [SlashCommand("haibai", "They comes .. they goes ..")]
    public async Task HaiBaiAsync()
    {
        var haiBaiCount = await funAccessor.IncrementHaiBai();

        await RespondAsync(
            $"_ _" +
            $"[`HaiBai Count: {haiBaiCount}`](https://cdn.discordapp.com/attachments/313015271566868480/502228420881678336/couch.gif?ex=6882a2aa&is=6881512a&hm=8541f1b54df685b3ee50627db3c2c1fae730e371d3e4fd7d78eadb40a1d1102f&)");
    }
}