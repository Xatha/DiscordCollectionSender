using Discord.WebSocket;

namespace DiscordBotTestApp

{
    internal class Message
    {
        public Message(SocketTextChannel textChannel) 
        {
            CreateMessage(textChannel);
        }

        public Message(SocketTextChannel textChannel, string attachmentPath)
        {
            CreateMessage(textChannel, attachmentPath);
        }

        private async Task CreateMessage(SocketTextChannel textChannel)
        {
            await textChannel.SendMessageAsync("Test message");
            //SendMessageAsync(channelID, severID)   
        }

        private async Task CreateMessage(SocketTextChannel textChannel, string attachmentPath)
        {
            await textChannel.SendFileAsync  (attachmentPath, "");
            
        }
    }
}