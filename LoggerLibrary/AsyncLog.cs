using Discord;
using log4net;
using System.Diagnostics;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace LoggerLibrary
{
    public static class LogAsync
    {
        public static ILog GetLogger()
        {
            var stackTrace = new StackTrace();
            var name = stackTrace?.GetFrame(1)?.GetMethod()?.DeclaringType?.Name;
            return LogManager.GetLogger(name);
        }
        public static async Task<ILog> GetLoggerAsync()
        {
            var stackTrace = new StackTrace();
            var name = stackTrace?.GetFrame(1)?.GetMethod()?.DeclaringType?.Name;
            return await Task.Run(() => LogManager.GetLogger(name));
        }
        public static async Task DebugAsync(this ILog log, object message) => await Task.Run(() => log.Debug(message));
        public static async Task DebugAsync(this ILog log, object message, Exception exception) => await Task.Run(() => log.Debug(message, exception));
        public static async Task InfoAsync(this ILog log, object message) => await Task.Run(() => log.Info(message));
        public static async Task InfoAsync(this ILog log, object message, Exception exception) => await Task.Run(() => log.Info(message, exception));

        public static async Task WarnAsync(this ILog log, object message) => await Task.Run(() => log.Warn(message));
        public static async Task WarnAsync(this ILog log, object message, Exception exception) => await Task.Run(() => log.Warn(message, exception));

        public static async Task ErrorAsync(this ILog log, object message) => await Task.Run(() => log.Error(message));
        public static async Task ErrorAsync(this ILog log, object message, Exception exception) => await Task.Run(() => log.Error(message, exception));

        public static async Task FatalAsync(this ILog log, object message) => await Task.Run(() => log.Fatal(message));
        public static async Task FatalAsync(this ILog log, object message, Exception exception) => await Task.Run(() => log.Fatal(message, exception));


        //Does not use Log4Net. This is a bad solution.
        //I am not sure how I can make the Discord API message format correctly without manually doing it like this.
        //TODO: Well, I could make a custom implentation of ILog interface...
        public static Task LogClientAsync(this ILog log, LogMessage msg)
        {
            var callerName = msg.Source;
            var message = msg.Message;
            //var outputString = String.Format("[{0,-1}] {1, 0} {2, 5} {3, -11} : {4}",  GetTime(),"[THD:   ]", "[INFO]", $"[Client|{callerName}]", message);
            //Console.WriteLine(outputString);
            log.InfoFormat("| [{0,-1}] {1, 0}", $"{callerName}", message);

            return Task.CompletedTask;
        }

        private static string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

    }
}
