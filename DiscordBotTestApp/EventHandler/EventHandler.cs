using Discord.WebSocket;
using LoggerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace DiscordBotTestApp
{
    internal class EventHandler : SingletonBase
    {
        private const string callerName = nameof(EventHandler);
        private static readonly Logger logger = new Logger();
        public Dictionary<int, object> EventActions { get; set; } = new Dictionary<int, object>();


        public EventHandler() : base(callerName)
        {
            logger.Log($"{callerName} initialised.", callerName);
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
                await logger.Log(item, callerName);
            }
        }

        internal static int GenerateID()
        {
            var rand = new Random();
            var id = rand.Next(999);

            return id;
        }

        private void LogSubscribe(string eventName) => logger.Log($"Subscribed to event: [{eventName}]", callerName);
        private void LogUnsubscribe(string eventName) => logger.Log($"Unsubscribed to event: [{eventName}]", callerName);
    }
}
