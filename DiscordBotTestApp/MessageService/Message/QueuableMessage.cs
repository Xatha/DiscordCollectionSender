using Discord.WebSocket;

namespace DiscordBotTestApp.MessageService

{
    internal class QueueableMessage : IQueueableMessage
    {
        public MessageData Data { get; init; }


        private QueueableMessage(MessageData messageData)
        {
            Data = messageData;
        }

        public static Task<QueueableMessage> CreateMessageAsync(MessageData messageData)
        {
            return Task.FromResult(new QueueableMessage(messageData));
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