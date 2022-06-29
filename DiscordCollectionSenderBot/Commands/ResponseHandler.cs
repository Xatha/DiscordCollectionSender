using Discord.WebSocket;
using DiscordCollectionSenderBot.FileProcessing;
using log4net;
using LoggerLibrary;


namespace DiscordCollectionSenderBot.Commands
{
    internal static class ResponseHandler
    {
        private static readonly ILog _logger = LogAsync.GetLogger();

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
            await _logger.InfoAsync("Request Recived");
            var optionName = command.Data.Options.First().Options.First().Name;
            string? url;

            if (optionName is null)
            {
                _logger.Error($"{nameof(optionName)} is null.");
                await command.RespondAsync($"Something went wrong with retrieving the options parameters... please try again.");
                return;
            }
            else if (optionName == "url")
            {
                //Discord.Net Api is not very intuitive. We are just getting the value that the option "url" is storing and then trimming it.
                url = command.Data.Options.First().Options.First().Options?.FirstOrDefault()?.Value.ToString()?.Trim(' ');
            }
            else //if optionName is attachment.
            {
                var value = command.Data.Options.First().Options.First().Options?.FirstOrDefault()?.Value as Discord.Attachment;
                url = value?.Url;
            }

            var fileExtension = command.Data.Options.First().Options.First().Options?.LastOrDefault()?.Value.ToString()?.Trim(' ');
            var destinationFolder = @"C:\Users\Luca\Desktop\testFolder\";
            var filteredFileExtension = await GetFileExtensionAsync(fileExtension) ?? ".zip";
            var fullPath = await GenerateFullPath(destinationFolder, filteredFileExtension);

            if (!await Downloader.Download(url!, fullPath))
            {
                await _logger.ErrorAsync($"Could not resolve the URL from the attachment.");
                await command.RespondAsync($"Download Failed. Please try again.");
                return;
            }

            await command.RespondAsync($"Download was sucessful. Processing files...");

            var guildID = command?.GuildId;
            var textChannelID = command?.ChannelId;
            var textChannel = client.GetGuild((ulong)guildID).GetTextChannel((ulong)textChannelID);

            var sortingTypeOption = command.Data.Options.First().Options.First().Options.ElementAtOrDefault(1)?.Value ?? 0;
            var sortingTypeAsEnum = (FileSortType)Convert.ToInt32(sortingTypeOption);
            //await new FileProcessingHandler(fullPath, client, command).Init();

            //await _logger.DebugAsync($"Url is {url}");
            //await _logger.DebugAsync($"Extension is {fileExtension}");
            //await _logger.DebugAsync($"SortingType is {sortingTypeAsEnum}");
            using var processingMaster = await ProcessingMaster.CreateAsync(fullPath, sortingTypeAsEnum, textChannel);
        }

        private static async Task<string> GenerateFullPath(string destinationFolder, string? fileExtension)
        {
            var fileName = await Utils.RandomStringAsync();
            var fullPath = @$"{destinationFolder}{fileName}{fileExtension}";

            //await _logger.DebugAsync($"File name is {fileName}");
            //await _logger.DebugAsync($"File extension is {fileExtension}");
            //await _logger.DebugAsync($"Full path is {fullPath}");

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
