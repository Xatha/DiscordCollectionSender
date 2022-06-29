using Discord;
using Discord.WebSocket;
using DiscordCollectionSenderBot.MessageService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCollectionSenderBot.Factory
{
    public static class MessageServiceFactory
    {
        public static Task<IQueueableMessage> CreateMessageAsync(IMessageData messageData)
        {
            IQueueableMessage result = new QueueableMessage(messageData);
            return Task.FromResult(result);
        }

        public static Task<IMessageData> CreateMessageDataAsync(IMessageContent content, MessageService.MessageType messageType, ITextChannel textChannel)
        {
            IMessageData result = new MessageData(content, messageType, textChannel);
            return Task.FromResult(result);
        }

        public static IMessageQueue CreateMessageQueue()
        {
            return new MessageQueue();
        }
        public static IMessageQueueHandler GetMessageQueueHandler()
        {
            return MessageQueueHandler.Instance;
        }
        public static Task<IMessageContent> CreateMessageContentAsync(string? text = null, string? attachmentFullPath = null)
        {
            IMessageContent result = new MessageContent(text, attachmentFullPath);
            return Task.FromResult(result);
        }
    }
}
