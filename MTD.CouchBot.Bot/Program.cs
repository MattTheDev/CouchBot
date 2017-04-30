using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MTD.CouchBot.Domain;
using System.Reflection;
using System.Threading.Tasks;

namespace MTD.CouchBot.Bot
{
    class Program
    {
        private CommandService commands;
        public static DiscordSocketClient client;
        private DependencyMap map;

        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            await DoBotStuff();

            await Task.Delay(-1);
        }

        public async Task DoBotStuff()
        {
            map = new DependencyMap();
            client = new DiscordSocketClient();
            commands = new CommandService();

            await InstallCommands();
            await client.LoginAsync(TokenType.Bot, Constants.DiscordToken);
            await client.StartAsync();

            ConfigureEventHandlers();
        }

        public async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasStringPrefix("!cb ", ref argPos))) return;

            var context = new CommandContext(client, message);

            var result = await commands.ExecuteAsync(context, argPos, map);
        }

        public void ConfigureEventHandlers()
        {

        }
    }
}