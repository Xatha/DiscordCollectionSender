
namespace DiscordBotTestApp.MessageService
{
    public interface IQueueableMessage
    {
        MessageData Data { get; init; }

        Task SendFileAsync();
        Task SendMessageAsync();
        Task SendTextAsync();
    }
}