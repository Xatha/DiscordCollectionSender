using DiscordCollectionSenderBot.MessageService;

namespace DiscordCollectionSenderBot.FileProcessing
{
    internal class ProcessingQueueEventArgs : IQueueEventArgs<FileInfo>
    {
        private readonly FileInfo _fileInfo;
        public ProcessingQueueEventArgs(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }
        public FileInfo Get()
        {
            return _fileInfo;
        }
    }
}