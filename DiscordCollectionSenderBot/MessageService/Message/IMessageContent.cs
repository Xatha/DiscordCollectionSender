namespace DiscordCollectionSenderBot.MessageService
{
    public interface IMessageContent
    {
        string? AttachmentFullPath { get; init; }
        string? Text { get; init; }
    }
}