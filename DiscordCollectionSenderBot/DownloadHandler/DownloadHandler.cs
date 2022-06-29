using LoggerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCollectionSenderBot
{
    internal static class Downloader
    {
        private static readonly LoggerLibrary.Logger logger = new LoggerLibrary.Logger();
        private const string callerName = nameof(Downloader);

        internal static async ValueTask<bool> Download(string url, string fullPath)
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
                catch (Exception e)
                {
                    await logger.LogError($"Download Failed.",callerName);
                    await logger.LogError(e, callerName);

                    result = false;
                }
            }

            return result;
        }







    }
}
