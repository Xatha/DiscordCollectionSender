using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotTestApp.MessageService
{
    internal class QueueEventHandler
    {
        public delegate void QueueEvent(IMessageQueue sender, IQueueEventArgs e);
    }
}
