using LoggerLibrary;
using System.IO.Compression;
using CompressionLibrary;
using Discord;
using Discord.WebSocket;

namespace DiscordBotTestApp
{
    public class FileProcessingHandler
    {
        private const string callerName = nameof(FileProcessingHandler);
        private static readonly Logger logger = new Logger();
        private static readonly MessageHandler messageHandler = new MessageHandler();
        private readonly DiscordSocketClient socketClient;
        private readonly ulong? guildID;
        private readonly ulong? textChannelID;
        private bool extractionComplete = false;

        public FileProcessingHandler(string fullPath, DiscordSocketClient socketClient, SocketSlashCommand command)
        {
            this.socketClient = socketClient;
            this.guildID = command?.GuildId;
            this.textChannelID = command?.ChannelId;

            Init(fullPath);
        }

        private async Task Init(string fullPath)
        {
            await logger.Log($"Beginning processing files...", callerName);
            var directoryPath = await GetDirectoryPathFromFullPath(fullPath);
            
            await CreateDirectory(directoryPath);

            await ExtractDirectory(fullPath, directoryPath);

            if (extractionComplete)
            {
                List<FileInfo> fileInfo = new List<FileInfo>();

                foreach (var file in Directory.GetFiles(directoryPath))
                {
                    fileInfo.Add(new FileInfo(file));
                }

                var filesUnder8Mb = CompressionHandler.GetAllFilesUnderSize(fileInfo, (long)8.389e+6);
                if (filesUnder8Mb != null)
                {
                    foreach (var file in filesUnder8Mb)
                    {
                        new Message(socketClient.GetGuild((ulong)guildID).GetTextChannel((ulong)textChannelID), file);
                    }
                }

                ResponseCallback response = new ResponseCallback();
                response.FilePath.ProgressChanged += FilePath_ProgressChanged;

                await CompressionHandler.InitAsync(fileInfo, response);
            }

        }

        private async void FilePath_ProgressChanged(object? sender, string e)
        {
            await logger.Log($"File {e} has been compressed", callerName);

            new Message(socketClient.GetGuild((ulong)guildID).GetTextChannel((ulong)textChannelID), e);

        }

        private async Task ExtractDirectory(string archieveFullPath, string destinationFullPath)
        {
            await logger.Log($"Attempting to extract {archieveFullPath} to {destinationFullPath}...", callerName);

            await Utils.RetryAction(() =>
            {
                ZipFile.ExtractToDirectory(archieveFullPath, destinationFullPath);
            });

            extractionComplete = true;
            await logger.Log($"File sucessfully extracted.", callerName);
        }

        private async Task CreateDirectory(string directoryFullPath)
        {
            await logger.Log($"Attempting to creat directory {directoryFullPath}", callerName);

            try
            {
                Directory.CreateDirectory(directoryFullPath);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<string> GetDirectoryPathFromFullPath(string fullPath)
        {
            var extensionCharCount = Path.GetExtension(fullPath).Length;
            var directoryPath = fullPath.Remove(fullPath.Length - extensionCharCount);
            return directoryPath;
        }
    }
}