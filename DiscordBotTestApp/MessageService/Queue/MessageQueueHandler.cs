using LoggerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotTestApp.MessageService
{
    internal class MessageQueueHandler
    {
        private const string callerName = nameof(MessageQueueHandler);
        private readonly Logger logger;
        private static readonly Lazy<MessageQueueHandler> _instance = new Lazy<MessageQueueHandler>(() => new());
        public static MessageQueueHandler Instance { get { return _instance.Value; } }

        public static bool IsRunning { get; private set; }
        public static bool SendMessagesAutomatically { get; private set; }

        private readonly IMessageQueue _messageQueue = new MessageQueue();

        private readonly int _id;

        private MessageQueueHandler()
        {
            logger = new();
            IsRunning = false;
            SendMessagesAutomatically = false;

            _id = GetHashCode();
            logger.Log(this._id, callerName);

            HookEvents(_messageQueue);
        }

        public async Task AddMessage(IQueueableMessage message)
        {
            await _messageQueue.Enqueue(message);
        }

        //This will basically make sure that when messages get queued they will be send eventually.
        private async Task ResolveQueue()
        {
            if (!IsRunning)
            {
                await logger.Log("Queueing...", callerName);
                IsRunning = true;
                await _messageQueue.BeginQueue(2000);
                await logger.Log("Queueing... finished", callerName);
                IsRunning = false;
            }

        }

        private Task HookEvents(IMessageQueue messageQueue)
        {
            messageQueue.OnEnqueue += MessageQueue_OnEnqueue;
            messageQueue.OnDequeue += MessageQueue_OnDequeue;
            messageQueue.OnMessageSent += MessageQueue_OnMessageSent;
            return Task.CompletedTask;
        }

        //If we get a request and we're not already resolving/runnig the queue, this will make sure that it will run again.
        private async void MessageQueue_OnEnqueue(IMessageQueue sender, IQueueEventArgs e)
        {
            await logger.Log($"Message has been queued. [{sender.Count}]", callerName);

            await ResolveQueue();
        }

        private async void MessageQueue_OnDequeue(IMessageQueue sender, IQueueEventArgs e)
        {
            await logger.Log($"Message {e.GetMessage()} has been dequeued [{sender.Count}]", callerName);
        }
        private void MessageQueue_OnMessageSent(IMessageQueue sender, IQueueEventArgs e)
        {
            //logger.Log($"[{sender.QueueSize}/{sender.Count}] Messages sent");
        }
    }

}
