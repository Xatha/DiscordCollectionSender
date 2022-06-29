using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordCollectionSenderBot.Commands;
using DiscordCollectionSenderBot.Logger;
//using DiscordCollectionSenderBot.Logger;
using log4net;
using LoggerLibrary;
using System.Reflection;
using System;
using System.Xml.Linq;

namespace DiscordCollectionSenderBot
{
    internal class Client
    {
        private static readonly ILog _logger = LogAsync.GetLogger();

        //private const string callerName = "Main";
        public DiscordSocketClient SocketClient { get; init; }
        private EventHandler? eventHandler;
        private CommandService? commands;

        private Client()
        {
            SocketClient = new DiscordSocketClient();
            ClientGlobal.SocketClient = SocketClient;
        }

        //This is where we initialize our bot.
        private async Task<Client> InitializeAsync()
        {
            await InitHandlersAsync();
            await InitEventsAsync();
            await InitLoginAsync();
            return this;
        }

        public static Task<Client> CreateAsync()
        {
            var SocketClient = new Client();
            return SocketClient.InitializeAsync();
        }

        private Task InitConfigurationsAsync()
        {
            var config = new CommandServiceConfig { DefaultRunMode = RunMode.Async };
            commands = new CommandService(config);

            return Task.CompletedTask;
        }

        private Task InitHandlersAsync()
        {
            eventHandler = new EventHandler();

            return Task.CompletedTask;
        }

        private Task InitEventsAsync()
        {
            SocketClient.Log += eventHandler!.Subscribe(EventHandler.GenerateID(), async (LogMessage msg) =>
            {
                await _logger.LogClientAsync(msg);
            }, nameof(SocketClient.Log));

            SocketClient.Ready += eventHandler.Subscribe(EventHandler.GenerateID(), () =>
            {
                return ClientReadyAsync();
            }, nameof(SocketClient.Ready));

            SocketClient.SlashCommandExecuted += eventHandler.Subscribe(EventHandler.GenerateID(), (SocketSlashCommand command) =>
            {
                ResponseHandler.SlashCommandExecuted(command);
                return Task.CompletedTask;
            }, nameof(SocketClient.SlashCommandExecuted));

            return Task.CompletedTask;
        }

        private async Task InitLoginAsync()
        {
            var token = File.ReadAllText(@"C:\Users\Luca\Desktop\Programming\DiscordBots\token.txt");
            await SocketClient.LoginAsync(TokenType.Bot, token);
            await SocketClient.StartAsync();
        }

        private async Task ClientReadyAsync()
        {
            SlashCommandsGlobals slashCommandGlobals = new SlashCommandsGlobals();

            //await slashCommandGlobals.CreateCommandsAsync();
        }
    }
}

