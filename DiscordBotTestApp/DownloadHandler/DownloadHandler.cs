using LoggerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotTestApp
{
    internal static class DownloadHandler
    {
        private static readonly Logger logger = new Logger();
        private const string callerName = nameof(DownloadHandler);

        internal static async Task<bool> Download(string url, string fullPath)
        {
            var result = false;

            await logger.Log($"Control provided with a URL: {url}... attempting to download...", callerName);

            using (var downloader = new FileDownloader())
            {
                try
                {
                    downloader.DownloadFileAsync(url, fullPath);

                    await logger.Log($"File downloaded succesfully. File can be found at [{fullPath}]", callerName);

                    result = true;
                }
                catch (Exception)
                {
                    await logger.LogError($"Download Failed.",callerName);

                    return false;
                }
            }

            return result;
        }







    }
}
