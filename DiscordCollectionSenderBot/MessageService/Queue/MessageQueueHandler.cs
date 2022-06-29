using DiscordCollectionSenderBot.Factory;
using DiscordCollectionSenderBot.Logger;
using log4net;
using LoggerLibrary;
using System.Reactive.Concurrency;

namespace DiscordCollectionSenderBot.MessageService
{
    public class MessageQueueHandler : IMessageQueueHandler
    {
        private static readonly ILog _logger = LogAsync.GetLogger();

        private static readonly Lazy<IMessageQueueHandler> _instance = new Lazy<IMessageQueueHandler>(() => new MessageQueueHandler());
        public static IMessageQueueHandler Instance { get { return _instance.Value; } }

        public static bool IsRunning { get; private set; }
        public IMessageQueue Queue { get; } = MessageServiceFactory.CreateMessageQueue();

        private MessageQueueHandler()
        {
            IsRunning = false;

            HookEvents(Queue);
        }

        public async Task AddMessage(IQueueableMessage message)
        {
            await Queue.Enqueue(message);
        }

        private static object _lock = new object();
        //This will basically make sure that when messages get queued they will be send eventually.
        private async Task ResolveQueue()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                //await _logger.InfoAsync("Queueing...");
                await Queue.BeginQueue(1000);
                //await _logger.InfoAsync("Queueing... finished");
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
        private async void MessageQueue_OnEnqueue(IMessageQueue sender, IQueueEventArgs<IQueueableMessage> e)
        {
            //await _logger.InfoAsync($"Message has been queued. [{sender.Count}]"); 
            await ResolveQueue();
        }

        private async void MessageQueue_OnDequeue(IMessageQueue sender, IQueueEventArgs<IQueueableMessage> e)
        {
            //await logger.Log($"Message {e.GetMessage()} has been dequeued [{sender.Count}]", callerName);
            //await _logger.InfoAsync($"Testing queue... {e.Get().Data.Type}");
            //await _logger.InfoAsync($"Message {e.Get()} has been dequeued [{sender.Count}]");
        }
        private async void MessageQueue_OnMessageSent(IMessageQueue sender, IQueueEventArgs<IQueueableMessage> e)
        {
            //logger.Log($"[{sender.QueueSize}/{sender.Count}] Messages sent");
            //await logger.InfoAsync($"[{sender.Count}/{sender.Count}] Messages sent");
        }
    }

}
