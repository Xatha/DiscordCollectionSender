using LoggerLibrary;
using Microsoft.VisualBasic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static DiscordBotTestApp.MessageService.QueueEventHandler;

namespace DiscordBotTestApp.MessageService
{
    internal class MessageQueue : Queue<IQueueableMessage>, IMessageQueue
    {
        public int QueueSize { get; private set; }

        public event QueueEvent? OnEnqueue;
        public event QueueEvent? OnDequeue;
        public event QueueEvent? OnMessageSent;

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
                    OnMessageSent(this, new QueueEventArgs(currentMessage));
                }

                await Task.Delay(delay);
            }
        }

        //ConcurrentDictionary<string, Task<Message>> MessageQueuedTasks = new ConcurrentDictionary<string, Task<Message>>();
        public new Task Enqueue(IQueueableMessage message)
        {
            lock (this)
            {
                base.Enqueue(message);
            }

            if (OnEnqueue is not null)
            {
                OnEnqueue(this, new QueueEventArgs(message));
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
                OnDequeue(this, new QueueEventArgs(message));
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
