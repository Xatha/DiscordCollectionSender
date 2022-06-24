using Discord.Commands;
using Discord.WebSocket;
using Discord;
using DiscordBotTestApp.Commands;
using LoggerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotTestApp
{
    internal class Client
    {
        private const string callerName = "Main";
        public DiscordSocketClient SocketClient { get; init; }
        private EventHandler eventHandler;
        private Logger logger;

        private CommandService commands;

        private Client()
        {
            SocketClient = new DiscordSocketClient();
            ClientGlobal.SocketClient = SocketClient;
        }


        //This is where we initialize our bot.
        private async Task<Client> InitializeAsync()
        {
            await InitHandlers();
            await InitEvents();
            await InitLogin();
            return this;
        }

        public static Task<Client> CreateAsync()
        {
            var SocketClient = new Client();
            return SocketClient.InitializeAsync();
        }

        private Task InitConfigurations()
        {
            var config = new CommandServiceConfig { DefaultRunMode = RunMode.Async };
            commands = new CommandService(config);

            return Task.CompletedTask;
        }

        private Task InitHandlers()
        {
            logger = new Logger();
            eventHandler = new EventHandler();

            return Task.CompletedTask;
        }

        private Task InitEvents()
        {
            SocketClient.Log += eventHandler.Subscribe(EventHandler.GenerateID(), async (LogMessage msg)
                => await logger.Log(msg), nameof(SocketClient.Log));

            SocketClient.Ready += eventHandler.Subscribe(EventHandler.GenerateID(), async ()
                => await Client_Ready(), nameof(SocketClient.Ready));

            //This is probably not very smart, but if I await the response it will block the thread/gateway.
            SocketClient.SlashCommandExecuted += eventHandler.Subscribe(EventHandler.GenerateID(), async (SocketSlashCommand command)
                => ResponseHandler.SlashCommandExecuted(command), nameof(SocketClient.SlashCommandExecuted));
            
            return Task.CompletedTask;
        }

        private async Task InitLogin()
        {
            var token = File.ReadAllText(@"C:\Users\Luca\Desktop\Programming\DiscordBots\token.txt");
            await SocketClient.LoginAsync(TokenType.Bot, token);
            await SocketClient.StartAsync();
        }


        private async Task Client_Ready()
        {
            SlashCommandsGlobals slashCommandGlobals = new SlashCommandsGlobals();
            
            //await slashCommandGlobals.CreateCommandsAsync();


        }
    }




}

