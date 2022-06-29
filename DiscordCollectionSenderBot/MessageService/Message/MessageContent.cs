namespace DiscordCollectionSenderBot.MessageService
{
    public struct MessageContent : IMessageContent
    {
        public string? Text { get; init; }
        public string? AttachmentFullPath { get; init; }

        public MessageContent(string? text = null, string? attachmentFullPath = null)
        {
            Text = text ?? "";
            AttachmentFullPath = attachmentFullPath;
        }
    }
}