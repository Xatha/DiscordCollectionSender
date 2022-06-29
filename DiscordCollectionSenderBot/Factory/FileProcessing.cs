using DiscordCollectionSenderBot.FileProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCollectionSenderBot.Factory
{
    internal static class FileProcessing
    {
        public static async Task<IDirectoryController> CreateDirectoryController(string archiveDirectoryPath)
        {
            return await DirectoryController.CreateAsync(archiveDirectoryPath);
        }

    }


}
