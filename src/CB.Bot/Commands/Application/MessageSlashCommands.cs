using CB.Accessors.Contracts;
using CB.Shared.Enums;
using CB.Shared.Utilities;
using Discord.Interactions;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace CB.Bot.Commands.Application;

[Group("message", "Message commands")]
public class MessageSlashCommands(IGuildAccessor guildAccessor,
    IMessageConfigurationAccessor messageConfigurationAccessor) : BaseSlashCommands
{
    [SlashCommand(
        "live",
        "Configure server 'Message' settings",
        false,
        RunMode.Async)]
    private async Task LiveMessageConfigurationAsync(string message)
    {
        await DoStuff(message, MessageType.Live);
    }

    [SlashCommand(
        "published",
        "Configure server 'Message' settings",
        false,
        RunMode.Async)]
    private async Task PublishedMessageConfigurationAsync(string message)
    {
        await DoStuff(message, MessageType.Published);
    }

    [SlashCommand(
        "offline",
        "Configure server 'Message' settings",
        false,
        RunMode.Async)]
    private async Task OfflineMessageConfigurationAsync(string message)
    {
        await DoStuff(message, MessageType.Offline);
    }

    [SlashCommand(
        "greeting",
        "Configure server 'Message' settings",
        false,
        RunMode.Async)]
    private async Task GreetingMessageConfigurationAsync(string message)
    {
        await DoStuff(message, MessageType.Greeting);
    }

    [SlashCommand(
        "goodbye",
        "Configure server 'Message' settings",
        false,
        RunMode.Async)]
    private async Task GoodbyeMessageConfigurationAsync(string message)
    {
        await DoStuff(message, MessageType.Goodbye);
    }

    private async Task DoStuff(string message, 
        MessageType messageType)
    {
        await SocketInteraction.DeferAsync(true);

        if (!await IsUserAdmin())
        {
            return;
        }

        var guild = await guildAccessor.GetConfigurationSummaryByIdAsync(Context.Guild.Id.ToString());
        var messages = GetMessage(messageType, message);

        switch (messageType)
        {
            case MessageType.Live:
                guild.MessageConfiguration.LiveMessage = messages.message;
                break;
            case MessageType.Published:
                guild.MessageConfiguration.PublishedMessage = messages.message;
                break;
            case MessageType.Greeting:
                guild.MessageConfiguration.GreetingMessage = messages.message;
                break;
            case MessageType.Goodbye:
                guild.MessageConfiguration.GoodbyeMessage = messages.message;
                break;
            case MessageType.Offline:
                guild.MessageConfiguration.StreamOfflineMessage = messages.message;
                break;
        }

        await messageConfigurationAccessor.UpdateAsync(guild.MessageConfiguration);
        await SocketInteraction.FollowupAsync(messages.output, ephemeral: true);
    }

    public (string message, string output) GetMessage(MessageType messageType, string message)
    {
        string outputMessage;

        if (message.ToLower().Equals("clear"))
        {
            message = messageType switch
            {
                MessageType.Live => "%CHANNEL% just went live with %GAME% - %TITLE% - %URL%",
                MessageType.Published => "%CHANNEL% just published a new video - %TITLE% - %URL%",
                MessageType.Offline => "This stream is now offline.",
                MessageType.Greeting => "Welcome to the server, %USER%",
                MessageType.Goodbye => "Good bye, %USER%, thanks for hanging out!",
                _ => message
            };

            outputMessage = $"Your '{messageType.ToString().FirstLetterToUpper()}' message has been reset.";
        }
        else if (messageType == MessageType.Offline &&
                 message.ToLower().Equals("none"))
        {
            message = "";
            outputMessage =
                $"Your '{messageType.ToString().FirstLetterToUpper()}' announcements will no longer be updated when streams go offline.";
        }
        else if (message.ToLower().Equals("empty"))
        {
            message = "_ _";
            outputMessage =
                $"Your '{messageType.ToString().FirstLetterToUpper()}' message has been set to .. empty..";
        }
        else
        {
            outputMessage = $"Your '{messageType.ToString().FirstLetterToUpper()}' message has successfully been set.";
        }

        return (message, outputMessage);
    }
}