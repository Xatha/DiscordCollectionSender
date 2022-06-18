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
        

        internal static async Task SlashCommandExecutedAsync(SocketSlashCommand command, DiscordSocketClient client)
        {
            switch (command.CommandName)
            {
                case "stop":
                    {
                        await command.RespondAsync($"Bot Terminated.");
                        Environment.Exit(0);
                    }
                    break;
                case "ping":
                    {
                        await command.RespondAsync($"Pong!");
                    }
                    break;
                case "my-purpose":
                    {
                        await logToConsole(command);

                        await command.RespondAsync($"My answer to your questions are...\n 1. Beep Boop, how could you tell? \n 2. I am a robot, I can compute billions of calculation by the time you blink, do you really doubt my intelligence mere mortal? \n 3. My creator is stupid, and created my name by smashing is keyboard. \n 4. I do not test, I *am* the test.");
                    }
                    break;
                case "post-collection":
                    {
                        await logToConsole(command);

                        var optionName = command.Data.Options.First().Options.First().Name;

                        await PostCollectionLogic(optionName, command, client);
                    }
                    break;
                default:
                    await logger.LogError($"Could not find a response to the command.", callerName);
                    break;
            }
        }

        private async static Task PostCollectionLogic(string optionName, SocketSlashCommand command, DiscordSocketClient client)
        {
            if (String.IsNullOrEmpty(optionName))
            {
                await logger.LogError("FieldName is null or empty.", $"{callerName}.{command.CommandName}");
                return;
            }

            string? url = "";
            string? fileExtension = "";

            if (optionName == "url")
            {
                url = command.Data.Options.First().Options.First().Options?.FirstOrDefault()?.Value.ToString()?.Trim(' ');
                fileExtension = command.Data.Options.First().Options.First().Options?.LastOrDefault()?.Value.ToString()?.Trim(' ');
            }
            else if (optionName == "attachment")
            {
                var value = command.Data.Options.First().Options.First().Options?.FirstOrDefault()?.Value as Discord.Attachment;
                url = value?.Url;
                fileExtension = command.Data.Options.First().Options.First().Options?.LastOrDefault()?.Value.ToString()?.Trim(' ');
            }

            if (String.IsNullOrEmpty(url))
            {
                await logger.LogError($"Could not resolve the URL from the attachment.", $"{callerName}.{command.CommandName}");
                await command.RespondAsync($"Could not resolve the URL from the attachment. Please try again.");
                return;
            }

            var destinationFolder = @"C:\Users\Luca\Desktop\testFolder\";
            var FilteredFileExtension = await GetFileExtensionAsync(fileExtension) ?? ".zip";
            var fullPath = await GenerateFullPath(destinationFolder, FilteredFileExtension);

            if (await DownloadHandler.Download(url, fullPath) == false)
            {
                await command.RespondAsync($"Download Failed. Please try again.");
                return;
            }

            await command.RespondAsync($"Download was sucessful. Processing files...");

            new FileProcessingHandler(fullPath, client, command);
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
        private static async Task logToConsole(SocketSlashCommand command) => await logger.Log($"Command {command.Data.Name} was used.", callerName);

        internal static async Task<string?> GetFileExtensionAsync(string? fileExtension)
        {
            if (fileExtension == "rar" || fileExtension == ".rar")
            {
                return ".rar";
            }
            else if (fileExtension == "zip" || fileExtension == ".zip")
            {
                return ".zip";
            }

            return null;
        }

    }

}
