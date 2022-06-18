using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotTestApp
{
    internal static class Utils
    {
        private static readonly LoggerLibrary.Logger logger = new LoggerLibrary.Logger();
        // Credit: https://stackoverflow.com/a/1344258
        public static async Task<String> RandomStringAsync()
        {
            var random = await Task.Run(() => new Random());

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];


            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        // Credit: https://stackoverflow.com/a/37154588
        internal static Task RetryAction(Action action, int timeoutMs = 60000)
        {
            var time = Stopwatch.StartNew();
            while (time.ElapsedMilliseconds < timeoutMs)
            {
                try
                {
                    action();
                    return Task.CompletedTask;
                }
                catch (IOException e)
                {
                    // access error
                    if (e.HResult != -2147024864)
                        throw;
                }
            }
            logger.LogError("Action failed... Failed perform action within allotted time.");
            throw new Exception("Failed perform action within allotted time.");
        }
    }
}
