namespace DiscordCollectionSenderBot.MessageService
{
    public interface IQueueEventArgs<T>
    {
        T Get();
    }
}