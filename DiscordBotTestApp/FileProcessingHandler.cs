using LoggerLibrary;
using System.IO.Compression;
using CompressionLibrary;
using Discord;
using Discord.WebSocket;
using UtilsLibrary;
using DiscordBotTestApp.MessageService;

namespace DiscordBotTestApp
{
    public class FileProcessingHandler
    {
        private const string callerName = nameof(FileProcessingHandler);
        private static readonly MessageQueueHandler messageQueueController = MessageQueueHandler.Instance;
        private static readonly Logger logger = new();
        private readonly string fullPath;
        private readonly DiscordSocketClient socketClient;
        private readonly ulong? guildID;
        private readonly ulong? textChannelID;
        private bool extractionComplete = false;

        public FileProcessingHandler(string fullPath, DiscordSocketClient socketClient, SocketSlashCommand command)
        {
            this.fullPath = fullPath;
            this.socketClient = socketClient;
            this.guildID = command?.GuildId;
            this.textChannelID = command?.ChannelId;
        }

        public async Task Init()
        {
            await logger.Log($"Beginning processing files...", callerName);
            var directoryPath = await GetDirectoryPathFromFullPath(this.fullPath);

            await CreateDirectory(directoryPath);

            await ExtractDirectory(this.fullPath, directoryPath);

            //Delete the compressed file, because it is not needed anymore.
            await DeleteCompressedFile(this.fullPath);

            if (extractionComplete)
            {
                List<FileInfo> filesInfo = new List<FileInfo>();

                foreach (var file in Directory.GetFiles(directoryPath))
                {
                    filesInfo.Add(new FileInfo(file));
                }

                var _8MbInMebiBytes = (long)8.389e+6;
                var filesUnder8Mb = await FilesUtil.SortByMaxSizeAsync(filesInfo, _8MbInMebiBytes);


                if (filesUnder8Mb != null)
                {
                    var textChannel = socketClient.GetGuild((ulong)guildID).GetTextChannel((ulong)textChannelID);


                    foreach (var file in filesUnder8Mb)
                    {
                        //await Message.CreateMessage(textChannel, file.FullName);


                        //await messageQueue.Enqueue(await Message.CreateMessageAsync(
                        //   new MessageData(
                        //       new MessageContent(null, file.FullName),
                        //       MessageService.MessageType.SendFile,
                        //       textChannel
                        //       )));

                        var msg = await QueueableMessage.CreateMessageAsync(
                          new MessageData(
                              new MessageContent(null, file.FullName),
                              MessageService.MessageType.SendFile,
                              textChannel
                              ));

                        await messageQueueController.AddMessage(msg);
                    }
                }
                Console.WriteLine("========================================");

                //await queue.BeginQueue(2000);

                var filesOver8Mb = await FilesUtil.SortByMinSizeAsync(filesInfo, _8MbInMebiBytes);
                if (filesOver8Mb == null)
                {

                    return;
                }

                ResponseCallback response = new ResponseCallback();
                response.FilePath.ProgressChanged += FilePath_ProgressChanged;

                // var imageCompressor = await ImageCompressor.CreateAsync(filesOver8Mb, response);
                await new ImageCompressor(filesOver8Mb, response).StartCompressionAsync();
            }

        }

        private async void FilePath_ProgressChanged(object? sender, string e)
        {
            await logger.Log($"File {e} has been compressed", callerName);

            var textChannel = socketClient.GetGuild((ulong)guildID).GetTextChannel((ulong)textChannelID);
            //await Message.CreateMessage(textChannel, e);

            //await messageQueue.Enqueue(await Message.CreateMessageAsync(
            //    new MessageData(
            //        new MessageContent(e),
            //            MessageService.MessageType.SendMessage,
            //            textChannel
            //        )));
            var msg = await QueueableMessage.CreateMessageAsync(
    new MessageData(
        new MessageContent(e),
            MessageService.MessageType.SendMessage,
            textChannel
        ));
            await messageQueueController.AddMessage(msg);

        }

        private Task DeleteCompressedFile(string fullPath)
        {
            try
            {
                File.Delete(fullPath);
            }
            catch (Exception)
            {
                throw;
            }
            return Task.CompletedTask;
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