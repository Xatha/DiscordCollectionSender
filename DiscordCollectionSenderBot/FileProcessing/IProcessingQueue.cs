using DiscordCollectionSenderBot.MessageService;
using System.Collections;

namespace DiscordCollectionSenderBot.FileProcessing
{
    public interface IProcessingQueue : ICollection
    {
        event QueueEventHandler.ProcessingEvent OnDequeue;
        event QueueEventHandler.ProcessingEvent OnEnqueue;
        event QueueEventHandler.ProcessingEvent OnProcessed;
        Task BeginQueue(int delay);
        Task Clear();
        Task<FileInfo> Dequeue();
        Task Enqueue(FileInfo file);
    }
}