
namespace DiscordCollectionSenderBot.FileProcessing
{
    internal interface IDirectoryController
    {
        DirectoryInfo ArchiveDirectory { get; }
        DirectoryInfo? CurrentDirectory { get; }

        Task DeleteCurrentDirectory();
    }
}