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
using DiscordBotTestApp.MessageService;

namespace DiscordBotTestApp
{
    public class Program
    {
        private Client? instance;


        public static Task Main() => new Program().MainAsync();

        public async Task MainAsync()
        {
            instance = await Client.CreateAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
    }
}