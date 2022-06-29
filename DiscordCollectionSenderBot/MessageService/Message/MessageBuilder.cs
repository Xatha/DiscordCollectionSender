
using DiscordCollectionSenderBot.MessageService;
using DiscordCollectionSenderBot.Factory;
using Discord.WebSocket;
using DiscordCollectionSenderBot.MessageService;
using Discord;

namespace DiscordCollectionSenderBot.MessageService.Message
{
    public class MessageBuilder
    {
        //Enums cant be null, so this will have to be default value.
        private MessageType _messageType;
        private Task<IMessageContent> _content;
        private readonly ITextChannel _textChannel;

        public MessageBuilder(ITextChannel textChannel)
        {
            _textChannel = textChannel ?? throw new NullReferenceException();
        }

        public MessageBuilder SetMessageContent(string? text = null, string? attachmentFullPath = null)
        {
            _content = MessageServiceFactory.CreateMessageContentAsync(text, attachmentFullPath) ?? throw new NullReferenceException();
            return this;
        }

        public MessageBuilder SetMessageType(MessageType messageType)
        {
            _messageType = messageType;
            return this;
        }

        public async Task<IQueueableMessage> BuildAsync()
        {
            var data = MessageServiceFactory.CreateMessageDataAsync(await _content, _messageType, _textChannel) ?? throw new NullReferenceException();
            return await MessageServiceFactory.CreateMessageAsync(await data);
        }
    }
}

