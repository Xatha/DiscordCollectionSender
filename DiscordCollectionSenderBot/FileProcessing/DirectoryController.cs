using CompressionLibrary;
using log4net;
using log4net.Repository.Hierarchy;
using LoggerLibrary;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsLibrary;

namespace DiscordCollectionSenderBot.FileProcessing
{
    internal sealed class DirectoryController : IDirectoryController
    {
        private static readonly ILog _logger = LogAsync.GetLogger();
        private readonly DirectoryInfo _archiveDirectory;
        private DirectoryInfo? _currentDirectory;

        public DirectoryInfo ArchiveDirectory => _archiveDirectory;
        public DirectoryInfo? CurrentDirectory => _currentDirectory;
        public List<FileInfo> FilesOver8Mb { get; }
        public List<FileInfo> FilesUnder8Mb { get; }


        private DirectoryController(string compressedDirectoryPath)
        {
            _archiveDirectory = new DirectoryInfo(compressedDirectoryPath);
            //Create target directroy
            //Extract file into target
            //TODO: Add checks for .mp4 and movie and other invalid file types.
            //Delete zip.
            //Delete target when all images have been sent.
        }

        public static Task<DirectoryController> CreateAsync(string archiveDirectoryPath)
        {
            var instance = new DirectoryController(archiveDirectoryPath);
            return instance.InitAsync();
        }

        private async Task<DirectoryController> InitAsync()
        {
            var directoryPath = await GetDirectoryPathFromFullPathAsync(_archiveDirectory.FullName);
            _currentDirectory = await CreateDirectoryAsync(directoryPath) ?? throw new NullReferenceException("Directory path is null");


            await ExtractDirectoryAsync(_archiveDirectory.FullName, _currentDirectory.FullName);
            await DeleteArchivedFileAsync(_archiveDirectory.FullName);

            return this;
        }

        public async Task DeleteCurrentDirectory()
        {
            try
            {
                File.Delete(_currentDirectory!.FullName);
            }
            catch (Exception e)
            {
                await _logger.ErrorAsync("Could not delete directory.", e);
            }
        }

        private async Task<DirectoryInfo?> CreateDirectoryAsync(string directoryFullPath)
        {
            await _logger.DebugAsync($"Attempting to create directory {directoryFullPath}");

            try
            {
                return Directory.CreateDirectory(directoryFullPath);
            }
            catch (Exception e)
            {
                await _logger.ErrorAsync("Could not create directory.", e);
            }
            return null;
        }
        private Task<string> GetDirectoryPathFromFullPathAsync(string fullPath)
        {
            var extensionCharCount = Path.GetExtension(fullPath).Length;
            var directoryPath = fullPath.Remove(fullPath.Length - extensionCharCount);
            return Task.FromResult(directoryPath);
        }

        private static async Task ExtractDirectoryAsync(string archieveFullPath, string destinationFullPath)
        {
            await _logger.DebugAsync($"Attempting to extract {archieveFullPath} to {destinationFullPath}...");

            await Utils.RetryAction(() =>
            {
                ZipFile.ExtractToDirectory(archieveFullPath, destinationFullPath);
            });
            await _logger.DebugAsync($"File sucessfully extracted.");
        }

        private async Task DeleteArchivedFileAsync(string fullPath)
        {
            try
            {
                File.Delete(fullPath);
            }
            catch (Exception e)
            {
                await _logger.ErrorAsync("Could not deleted archived file.", e);
            }
        }
    }
}
