using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCollectionSenderBot
{
    public static class ClientGlobal
    {
        private static DiscordSocketClient _socketClient;
        public static DiscordSocketClient SocketClient
        {
            get
            {
                return _socketClient == null ? throw new InvalidOperationException("Global variable has not been set.") : _socketClient;
            }
            set
            {
                _socketClient = _socketClient == null ? value : throw new InvalidOperationException("Global variable has already been set.");
            }
        }
    }
}
