using System.Collections;

namespace DiscordCollectionSenderBot.MessageService
{
    public interface IMessageQueue : ICollection
    {
        public event QueueEventHandler.MessageEvent OnDequeue;
        public event QueueEventHandler.MessageEvent OnEnqueue;
        public event QueueEventHandler.MessageEvent OnMessageSent;

        Task BeginQueue(int delay);
        Task Clear();
        Task<IQueueableMessage> Dequeue();
        Task Enqueue(IQueueableMessage message);
    }
}