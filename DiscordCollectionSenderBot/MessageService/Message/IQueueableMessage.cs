
namespace DiscordCollectionSenderBot.MessageService
{
    public interface IQueueableMessage : IQueableReponse
    {
        IMessageData Data { get; init; }

        Task SendFileAsync();
        Task SendMessageAsync();
        Task SendTextAsync();
    }
}