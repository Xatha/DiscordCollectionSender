using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Mail;
using System.Windows.Input;
using LoggerLibrary;
using System.Text;
using System;
using DiscordBotTestApp.Commands;

namespace DiscordBotTestApp
{
    public class Program
    {
        private const string callerName = "Main";
        private Logger logger;
        private DiscordSocketClient client;
        private EventHandler eventHandler;

        private readonly CommandService commands;

        public static Task Main() => new Program().MainAsync();

        public async Task MainAsync()
        {
            await InitHandlers();

            await InitEvents();

            await InitLogin();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task InitLogin()
        {
            var token = File.ReadAllText(@"C:\Users\Luca\Desktop\Programming\DiscordBots\token.txt");
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
        }

        private Task InitHandlers()
        {
            client = new DiscordSocketClient();
            logger = new Logger();
            eventHandler = new EventHandler();

            return Task.CompletedTask;
        }

        private Task InitEvents()
        {
            client.Log += eventHandler.Subscribe(EventHandler.GenerateID(), async (LogMessage msg) 
                => await logger.Log(msg), nameof(client.Log));

            client.Ready += eventHandler.Subscribe(EventHandler.GenerateID(), async () 
                => await Client_Ready(), nameof(client.Ready));

            client.SlashCommandExecuted += eventHandler.Subscribe(EventHandler.GenerateID(), async (SocketSlashCommand command) 
                => await ResponseHandler.SlashCommandExecutedAsync(command, client), nameof(client.SlashCommandExecuted));

            client.MessageReceived += eventHandler.Subscribe(EventHandler.GenerateID(), async (SocketMessage messageArg) 
                => await Client_MessageReceived(messageArg), nameof(client.MessageReceived));

            return Task.CompletedTask;
        }



        internal async Task Client_MessageReceived(SocketMessage messageArg)
        {
            // Don't process the command if it was a system message
            var message = messageArg as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) ||
                message.HasMentionPrefix(client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);
        }


        public async Task Client_Ready()
        {
            SlashCommandsGlobals slashCommandGlobals = new SlashCommandsGlobals(client);

            //await slashCommandGlobals.CreateCommandsAsync();

            
        }
    }
}