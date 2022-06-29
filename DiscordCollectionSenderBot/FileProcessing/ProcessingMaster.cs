using CompressionLibrary;
using Discord;
using DiscordCollectionSenderBot.MessageService;
using DiscordCollectionSenderBot.MessageService.Message;
using log4net;
using LoggerLibrary;
using UtilsLibrary;

namespace DiscordCollectionSenderBot.FileProcessing
{
    internal class ProcessingMaster : IDisposable
    {
        private static readonly ILog _logger = LogAsync.GetLogger();
        private static readonly IMessageQueueHandler _messageQueueHandler = Factory.MessageServiceFactory.GetMessageQueueHandler();
        private readonly ITextChannel _socket;
        private IDirectoryController _directoryController;
        private IResponseCallback _compressionResponse;

        private ProcessingMaster(ITextChannel textChannelWebSocket)
        {
            _socket = textChannelWebSocket;
        }
        ~ProcessingMaster()
        {
            _logger.Info("Finalizer called");
        }


        public static async Task<ProcessingMaster> CreateAsync(string archiveDirectoryPath, FileSortType fileSortType, ITextChannel textChannelWebSocket)
        {
            var instance = new ProcessingMaster(textChannelWebSocket);
            await instance.InitAsync(archiveDirectoryPath, fileSortType);
            return instance;
        }

        private async Task<ProcessingMaster> InitAsync(string archiveDirectoryPath, FileSortType fileSortType)
        {
            Console.WriteLine("xdd");
            _directoryController = await Factory.FileProcessing.CreateDirectoryController(archiveDirectoryPath);
            _compressionResponse = await CompressionLibrary.Factory.CreateResponseCallbackAsync();
            _compressionResponse.FilePath.ProgressChanged += Compression_ProgressChanged;

            //var filesInDirectory = _directoryController.CurrentDirectory!.EnumerateFiles().ToList();

            var filesUnsorted = _directoryController.CurrentDirectory!.GetFiles();
            var filesSorted = SortFiles(fileSortType, filesUnsorted);

            //Maps each file with a coressponding number, which serves as the position.
            var fileMap = await MapFilesToPositionAsync(filesSorted);

            var filesOver8Mb = await FilesUtil.SortByMinSizeAsync(filesSorted, (long)8.389e+6);
            if (filesOver8Mb is not null)
            {
                await ProcessFilesOver8Mb(filesOver8Mb, fileMap);
            }

            await AppendFilesToMessageQueue(fileMap);
            _messageQueueHandler.Queue.OnMessageSent += Queue_OnMessageSent;
            return this;
        }

        private async void Queue_OnMessageSent(IMessageQueue sender, IQueueEventArgs<IQueueableMessage> e)
        {
            var msg = e.Get();
            var filePathSent = msg.Data.Content?.AttachmentFullPath;
            if (filePathSent is not null)
            {
                var directory = new FileInfo(filePathSent).Directory;
                if (directory?.GetFiles().Length > 1)
                {
                    File.Delete(filePathSent);
                }
                else
                {
                    File.Delete(filePathSent);
                    directory.Delete(true);
                    await _logger.InfoAsync("Files sucessfully processed and sent.");
                }
            }
        }

        private async Task AppendFilesToMessageQueue(Dictionary<int, FileInfo> fileMap)
        {
            foreach (var file in fileMap)
            {
                try
                {
                    var msg = await new MessageBuilder(_socket).SetMessageType(MessageService.MessageType.SendFile)
                                                               .SetMessageContent("", file.Value.FullName)
                                                               .BuildAsync();
                    await _messageQueueHandler.AddMessage(msg);
                }
                catch (Exception e)
                {
                    await _logger.WarnAsync("Message could not be succesfully build.", e);
                    throw;
                }
            }
        }

        private async Task ProcessFilesOver8Mb(List<FileInfo> filesOver8Mb, Dictionary<int, FileInfo> fileMap)
        {
            if (filesOver8Mb is not null)
            {
                using (var imageCompressor = await CompressionLibrary.Factory.CreateImageCompressorAsync(filesOver8Mb, _compressionResponse))
                {
                    await imageCompressor.StartCompressionAsync();
                }

                foreach (var file in filesOver8Mb)
                {
                    foreach (var mappedFile in fileMap)
                    {
                        if (file == mappedFile.Value)
                        {
                            fileMap[mappedFile.Key] = file;
                        }
                    }
                }
            }
        }

        private static List<FileInfo> SortFiles(FileSortType fileSortType, FileInfo[] unsortedFiles)
        {
            return fileSortType switch
            {
                FileSortType.ByNameAscending => unsortedFiles.OrderBy(f => f.Name).ToList(), //If FileSortType.ByName
                FileSortType.ByDateAscending => unsortedFiles.OrderBy(f => f.LastWriteTimeUtc).ToList(), // If FileSortType.ByDate\
                FileSortType.BySizeAscending => unsortedFiles.OrderBy(f => f.Length).ToList(), // If FileSortType.BySize
                FileSortType.ByNameDescending => unsortedFiles.OrderByDescending(f => f.Name).ToList(), //If FileSortType.ByName
                FileSortType.ByDateDescending => unsortedFiles.OrderByDescending(f => f.LastWriteTimeUtc).ToList(), // If FileSortType.ByDate\
                FileSortType.BySizeDescending => unsortedFiles.OrderByDescending(f => f.Length).ToList(), // If FileSortType.BySize
                _ => throw new ArgumentOutOfRangeException(nameof(fileSortType), fileSortType, null)
            };
        }

        private Task<Dictionary<int, FileInfo>> MapFilesToPositionAsync(List<FileInfo> filesInDirectory)
        {
            var result = new Dictionary<int, FileInfo>();
            var position = 0;
            foreach (var file in filesInDirectory)
            {
                result.Add(position++, file);
            }
            return Task.FromResult(result);
        }

        private void Compression_ProgressChanged(object? sender, string e)
        {

        }

        public void Dispose()
        {
            _messageQueueHandler.Queue.OnMessageSent -= Queue_OnMessageSent;
        }
    }
}
