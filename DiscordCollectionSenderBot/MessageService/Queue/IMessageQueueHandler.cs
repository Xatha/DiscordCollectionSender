
using DiscordCollectionSenderBot.Factory;

namespace DiscordCollectionSenderBot.MessageService
{
    public interface IMessageQueueHandler
    {
        public static IMessageQueueHandler Instance { get; }
        public IMessageQueue Queue { get; }

        Task AddMessage(IQueueableMessage message);
    }
}