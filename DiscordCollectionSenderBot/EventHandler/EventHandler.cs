using Discord.WebSocket;
using DiscordCollectionSenderBot.Logger;
//using DiscordCollectionSenderBot.Logger;
using log4net;
using LoggerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace DiscordCollectionSenderBot
{
    internal class EventHandler : SingletonBase
    {

        private static readonly ILog logger = LogAsync.GetLogger();
        //private static readonly Logger logger = new Logger();

        private const string callerName = nameof(EventHandler);
        public Dictionary<int, object> EventActions { get; set; } = new Dictionary<int, object>();


        public EventHandler() : base(callerName)
        {
            logger.Info($"Initialised.");
        }

        internal Func<T, Task> Subscribe<T>(int eventID, Func<T, Task> action, string eventName = " ")
        {
            LogSubscribe(eventName);

            EventActions.Add(eventID, action);
            return action;
        }

        internal Func<Task> Subscribe(int eventID, Func<Task> action, string eventName = "")
        {
            LogSubscribe(eventName);
            EventActions.Add(eventID, action);
            return action;
        }

        internal Func<T, Task> Unsubscribe<T>(int eventID, string eventName = "")
        {
            LogUnsubscribe(eventName);

            var eventCopy = EventActions[eventID];
            EventActions.Remove(eventID);
            
            return (Func<T, Task>)eventCopy;
        }

        internal async Task ListEvents()
        {
            foreach (var item in EventActions)
            {
                await Task.Run(() => logger.Info(item));
            }
        }

        internal static int GenerateID()
        {
            var rand = new Random();
            var id = rand.Next(999);

            return id;
        }

        private Task LogSubscribe(string eventName) => logger.InfoAsync($"Subscribed to event: [{eventName}]");
        private Task LogUnsubscribe(string eventName) => logger.InfoAsync($"Unsubscribed to event: [{eventName}]");
    }
}
