using Discord.WebSocket;

namespace DiscordBotTestApp.MessageService
{
    public struct MessageData
    {
        public MessageContent Content { get; init; }
        public MessageType Type { get; init; }
        public SocketTextChannel TextChannel { get; init; }

        public MessageData(MessageContent content, MessageType messageType, SocketTextChannel textChannel)
        {
            Content = content;
            Type = messageType;
            TextChannel = textChannel;
        }

    }
}