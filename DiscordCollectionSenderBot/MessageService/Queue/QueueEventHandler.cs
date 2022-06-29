using DiscordCollectionSenderBot.FileProcessing;

namespace DiscordCollectionSenderBot.MessageService
{
    public class QueueEventHandler
    {
        public delegate void MessageEvent(IMessageQueue sender, IQueueEventArgs<IQueueableMessage> e);
        public delegate void ProcessingEvent(IProcessingQueue sender, IQueueEventArgs<FileInfo> e);

    }
}
