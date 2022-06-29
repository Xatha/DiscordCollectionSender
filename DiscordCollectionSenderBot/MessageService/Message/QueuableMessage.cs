using Discord.WebSocket;

namespace DiscordCollectionSenderBot.MessageService
{
    internal class QueueableMessage : IQueueableMessage
    {
        public IMessageData Data { get; init; }

        public QueueableMessage(IMessageData messageData)
        {
            Data = messageData;
        }

        public async Task SendMessageAsync()
        {
            switch (Data.Type)
            {
                case MessageType.SendFile:
                    await SendFileAsync();
                    break;
                case MessageType.SendMessage:
                    await SendTextAsync();
                    break;
            }
            await Task.CompletedTask;
        }

        public async Task SendTextAsync()
        {
            var textChannel = Data.TextChannel;
            var text = Data.Content.Text;
            await textChannel.SendMessageAsync(text);
        }

        public async Task SendFileAsync()
        {
            var textChannel = Data.TextChannel;
            var attachmentFullPath = Data.Content.AttachmentFullPath;
            var text = Data.Content.Text;
            await textChannel.SendFileAsync(attachmentFullPath, text);
        }
    }
}