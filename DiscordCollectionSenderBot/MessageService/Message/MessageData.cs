using Discord;
using Discord.WebSocket;

namespace DiscordCollectionSenderBot.MessageService
{
    public struct MessageData : IMessageData
    {
        public IMessageContent Content { get; init; }
        public MessageType Type { get; init; }
        public IMessageChannel TextChannel { get; init; }

        public MessageData(IMessageContent content, MessageType messageType, IMessageChannel textChannel)
        {
            Content = content;
            Type = messageType;
            TextChannel = textChannel;
        }

    }
}