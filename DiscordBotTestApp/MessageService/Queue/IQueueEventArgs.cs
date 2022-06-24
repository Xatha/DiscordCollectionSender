namespace DiscordBotTestApp.MessageService
{
    public interface IQueueEventArgs
    {
        IQueueableMessage GetMessage();
    }
}