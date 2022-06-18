using System;
using Discord;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace LoggerLibrary
{
    public class Logger
    {
        public static int loggerCount;

        public int ID { get; }

        public Logger()
        {
            ID = 0;
        }

        public Task Log(LogMessage msg)
        {
            var callerName = msg.Source;
            var message = msg.Message;

            var outputString = String.Format("[{0,-1}] [{1, -1}] {2, -11} {3}", "INFO", GetTime(), $"[{callerName}]", message);
            Console.WriteLine(outputString);

            return Task.CompletedTask;
        }

        public Task Log(object msg, string callerName = "    ")
        {
            
            var outputString = String.Format("[{0,-1}] [{1, -1}] {2, -11} {3}", "INFO", GetTime(), $"[{callerName}]", msg);
            Console.WriteLine(outputString);
            return Task.CompletedTask;
        }

      
        public Task Log(int objectID, object msg)
        {
            Console.WriteLine($"[Info] [ID = {objectID}] : {msg}");
            return Task.CompletedTask;
        }

        public Task LogWarn(object msg)
        {
            Console.WriteLine($"[Warn] : {msg}");
            return Task.CompletedTask;
        }
        public Task LogWarn(int objectID, object msg)
        {
            Console.WriteLine($"[Warn] [ID = {objectID}] : {msg}");
            return Task.CompletedTask;
        }

        public Task LogError(object msg)
        {
            Console.WriteLine($"[Error] : {msg}");
            return Task.CompletedTask;
        }
        public Task LogError(int objectID, object msg)
        {
            Console.WriteLine($"[Error] [ID = {objectID}] : {msg}");
            return Task.CompletedTask;
        }

        public Task LogError(object msg, string callerName = "    ")
        {
            var outputString = String.Format("[{0,-1}] [{1, -1}] {2, -11} {3}", "ERROR", GetTime(), $"[{callerName}]", msg);
            Console.WriteLine(outputString);
            return Task.CompletedTask;
        }

        private String GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
