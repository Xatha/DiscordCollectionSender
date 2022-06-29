using CompressionLibrary;
using Discord.WebSocket;
using DiscordCollectionSenderBot.MessageService;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCollectionSenderBot.FileProcessing
{
    internal class ProcessingQueue : Queue<FileInfo>, IProcessingQueue
    {
        private const long _8MbInMebiBytes = (long)8.389e+6;

        private static readonly IMessageQueueHandler _messageQueueHandler = Factory.MessageServiceFactory.GetMessageQueueHandler();
        private readonly SocketTextChannel _socket;
        private readonly ImageCompressor _imageCompressor;

        private int _queueSize;
        private ConcurrentDictionary<int, FileInfo> _fileMap = new();

        public event QueueEventHandler.ProcessingEvent OnDequeue;
        public event QueueEventHandler.ProcessingEvent OnEnqueue;
        public event QueueEventHandler.ProcessingEvent OnProcessed;

        public ProcessingQueue(IEnumerable<FileInfo> collection) : base(collection)
        {

        }

        public ProcessingQueue(SocketTextChannel socketTextChannel) : base()
        {
            _socket = socketTextChannel;
        }

        public async Task BeginQueue(int delay)
        {
            _queueSize = this.Count;
            
            while (this.Count > 0)
            {
                var currentFile = this.Peek();

                if (currentFile.Length <= _8MbInMebiBytes)
                {
                    var msg = await new MessageService.Message.MessageBuilder(_socket)
                                                .SetMessageContent(null, currentFile.FullName)
                                                .SetMessageType(MessageType.SendFile).BuildAsync();
                    await _messageQueueHandler.AddMessage(msg);
                    await this.Dequeue();
                }
                else if (currentFile.Length >= _8MbInMebiBytes)
                {
                   // await CompressionLibrary.Factory.CreateImageCompressorAsync();
                }

                //await currentMessage.SendMessageAsync();

                if (OnProcessed is not null)
                {
                    OnProcessed(this, new ProcessingQueueEventArgs(currentFile));
                }

                await Task.Delay(delay);
            }
        }

        public new Task Enqueue(FileInfo file)
        {
            lock (this)
            {
                base.Enqueue(file);
            }

            if (OnEnqueue is not null)
            {
                OnEnqueue(this, new ProcessingQueueEventArgs(file));
            }

            return Task.CompletedTask;
        }

        public new Task<FileInfo> Dequeue()
        {
            FileInfo file;
            lock (this)
            {
                file = base.Dequeue();
            }
            if (OnDequeue is not null)
            {
                OnDequeue(this, new ProcessingQueueEventArgs(file));
            }

            return Task.FromResult(file);
        }

        public new Task Clear()
        {
            lock (this)
            {
                this.Clear();
                this._queueSize = 0;
            }
            return Task.CompletedTask;
        }
    }
}
