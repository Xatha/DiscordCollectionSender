using Discord.WebSocket;
using LoggerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DiscordBotTestApp.Commands
{
    internal static class ResponseHandler
    {
        private static readonly Logger logger = new Logger();
        private const string callerName = nameof(ResponseHandler);

        private static async Task logToConsole(SocketSlashCommand command) => await logger.Log($"Command {command.Data.Name} was used.", callerName);

        internal static Task SlashCommandExecuted(SocketSlashCommand command) =>
            command.CommandName switch
            {
                "gateway-test" => GatewayTestResponseAsync(command),
                "stop" => StopCommandResponseAsync(),
                "ping" => PingCommandResponseAsync(command),
                "my-purpose" => MyPurposeCommandResponseAsync(command),
                "post-collection" => PostCollectionCommandResponseAsync(command, ClientGlobal.SocketClient),
                _ => throw new ArgumentNullException(nameof(command)),
            };
        private static async Task GatewayTestResponseAsync(SocketSlashCommand command)
        {
            await command.RespondAsync($"Testing...");
            Console.WriteLine("Task delay start!");
            await Task.Delay(5000);
            Console.WriteLine("Task delay done!");
            
        }

        private static Task StopCommandResponseAsync()
        {
            Environment.Exit(0);
            return Task.CompletedTask;
        }


        private static async Task PingCommandResponseAsync(SocketSlashCommand command) => await command.RespondAsync($"Pong!");

        private static async Task MyPurposeCommandResponseAsync(SocketSlashCommand command) 
            => await command.RespondAsync($"My answer to your questions are...\n 1. Beep Boop, how could you tell? \n 2. I am a robot, I can compute billions of calculation by the time you blink, do you really doubt my intelligence mere mortal? \n 3. My creator is stupid, and created my name by smashing is keyboard. \n 4. I do not test, I *am* the test.");

        private async static Task PostCollectionCommandResponseAsync(SocketSlashCommand command, DiscordSocketClient client)
        {
            var optionName = command.Data.Options.First().Options.First().Name;
            string? url;

            if (optionName == null)
            {
                //Implement logger.
                return;
            }
            else if (optionName == "url")
            {
                url = command.Data.Options.First().Options.First().Options?.FirstOrDefault()?.Value.ToString()?.Trim(' ');
            }
            else //if optionName is attachment,
            {
                var value = command.Data.Options.First().Options.First().Options?.FirstOrDefault()?.Value as Discord.Attachment;
                url = value?.Url;
            }


            if (String.IsNullOrEmpty(url))
            {
                await logger.LogError($"Could not resolve the URL from the attachment.", $"{callerName}.{command.CommandName}");
                await command.RespondAsync($"Could not resolve the URL from the attachment. Please try again.");
                return;
            }

            var fileExtension = command.Data.Options.First().Options.First().Options?.LastOrDefault()?.Value.ToString()?.Trim(' ');

            var destinationFolder = @"C:\Users\Luca\Desktop\testFolder\";
            var filteredFileExtension = await GetFileExtensionAsync(fileExtension) ?? ".zip";

            var fullPath = await GenerateFullPath(destinationFolder, filteredFileExtension);

            if (!await DownloadHandler.Download(url, fullPath))
            {
                await command.RespondAsync($"Download Failed. Please try again.");
                return;
            }

            await command.RespondAsync($"Download was sucessful. Processing files...");

            await new FileProcessingHandler(fullPath, client, command).Init();
            Console.WriteLine("=========RH==========");
        }

        private static async Task<string> GenerateFullPath(string destinationFolder, string? fileExtension)
        {
            var fileName = await Utils.RandomStringAsync();
            var fullPath = @$"{destinationFolder}{fileName}{fileExtension}";

            await logger.Log($"File name is {fileName}", callerName);
            await logger.Log($"File extension is {fileExtension}", callerName);
            await logger.Log($"Full path is {fullPath}", callerName);

            return fullPath;
        }
       
        internal static Task<string?> GetFileExtensionAsync(string? fileExtension)
        {
            if (fileExtension == "rar" || fileExtension == ".rar")
            {
                return Task.FromResult<string?>(".rar");
            }
            else if (fileExtension == "zip" || fileExtension == ".zip")
            {
                return Task.FromResult<string?>(".zip");
            }

            return Task.FromResult<string?>(null);
        }

    }

}
