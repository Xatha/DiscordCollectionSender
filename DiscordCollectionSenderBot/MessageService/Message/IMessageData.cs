using Discord;
using Discord.WebSocket;

namespace DiscordCollectionSenderBot.MessageService
{
    public interface IMessageData
    {
        IMessageContent Content { get; init; }
        IMessageChannel TextChannel { get; init; }
        MessageType Type { get; init; }
    }
}