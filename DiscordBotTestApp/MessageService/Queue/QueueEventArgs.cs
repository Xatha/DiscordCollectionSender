namespace DiscordBotTestApp.MessageService
{
    public class QueueEventArgs : EventArgs, IQueueEventArgs
    {
        private readonly IQueueableMessage _message;
        public QueueEventArgs(IQueueableMessage message)
        {
            _message = message;
        }
        public IQueueableMessage GetMessage()
        {
            return _message;
        }
    }
}