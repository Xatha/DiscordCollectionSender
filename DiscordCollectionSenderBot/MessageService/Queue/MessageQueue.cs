

namespace DiscordCollectionSenderBot.MessageService
{
    internal class MessageQueue : Queue<IQueueableMessage>, IMessageQueue
    {
        public int QueueSize { get; private set; }

        public event QueueEventHandler.MessageEvent? OnEnqueue;
        public event QueueEventHandler.MessageEvent? OnDequeue;
        public event QueueEventHandler.MessageEvent? OnMessageSent;

        public MessageQueue(IEnumerable<IQueueableMessage> collection) : base(collection)
        {
        
        }

        public MessageQueue() : base()
        {

        }

        // Lack of a better term... Basically, start 'pushing' messages and removing the sent messages from the queue. 
        public async Task BeginQueue(int delay)
        {
            QueueSize = this.Count;
            while (this.Count > 0)
            {
                var currentMessage = await this.Dequeue();

                await currentMessage.SendMessageAsync();

                if (OnMessageSent is not null)
                {
                    OnMessageSent(this, new MessageQueueEventArgs(currentMessage));
                }

                await Task.Delay(delay);
            }
        }

        public new Task Enqueue(IQueueableMessage message)
        {
            lock (this)
            {
                base.Enqueue(message);
            }

            if (OnEnqueue is not null)
            {
                OnEnqueue(this, new MessageQueueEventArgs(message));
            }

            return Task.CompletedTask;
        }

        public new Task<IQueueableMessage> Dequeue()
        {
            IQueueableMessage message;
            lock (this)
            {
                message = base.Dequeue();
            }
            if (OnDequeue is not null)
            {
                OnDequeue(this, new MessageQueueEventArgs(message));
            }

            return Task.FromResult(message);
        }

        public new Task Clear()
        {
            lock (this)
            {
                this.Clear();
                this.QueueSize = 0;
            }
            return Task.CompletedTask;
        }
    }
}
