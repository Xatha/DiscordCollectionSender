
using System.Collections;

namespace DiscordBotTestApp.MessageService
{
    internal interface IMessageQueue : ICollection
    {
        event QueueEventHandler.QueueEvent OnDequeue;
        event QueueEventHandler.QueueEvent OnEnqueue;
        event QueueEventHandler.QueueEvent OnMessageSent;

        Task BeginQueue(int delay);
        Task Clear();
        Task<IQueueableMessage> Dequeue();
        Task Enqueue(IQueueableMessage message);
    }
}