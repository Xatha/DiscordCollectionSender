namespace DiscordCollectionSenderBot.MessageService
{
    public class MessageQueueEventArgs : EventArgs, IQueueEventArgs<IQueueableMessage>
    {
        private readonly IQueueableMessage _message;
        public MessageQueueEventArgs(IQueueableMessage message)
        {
            _message = message;
        }
        public IQueueableMessage Get()
        {
            return _message;
        }
    }
}