using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace UtilsLibrary
{
    public static class FilesUtil
    {
        public static List<string>? GetAllFilesAboveSize(List<FileInfo> files, long minFileSize)
        {
            var results = new List<string>();

            var filePaths = GetAllFilesPathsFromFileList(files);

            foreach (var filePath in filePaths)
            {
                var fileSize = new FileInfo(filePath).Length;

                if (fileSize > minFileSize)
                {
                    results.Add(filePath);
                }
            }

            return results.Count == 0 ? null : results;
        }

        public static List<string>? GetAllFilesUnderSize(List<FileInfo> files, long maxFileSize)
        {
            var results = new List<string>();
            var filePaths = GetAllFilesPathsFromFileList(files);

            foreach (var filePath in filePaths)
            {
                var fileSize = new FileInfo(filePath).Length;

                if (fileSize < maxFileSize)
                {

                    results.Add(filePath);
                }
            }

            return results.Count == 0 ? null : results;
        }

        public static List<string> GetAllFilesPathsFromFileList(List<FileInfo> files)
        {
            var filePaths = new List<string>();

            foreach (var file in files)
            {
                var filePath = file.FullName;
                filePaths.Add(filePath);
            }
            return filePaths;
        }

        public static Task<List<FileInfo>?> SortByMinSizeAsync(List<FileInfo> files, long minFileSize)
        {
            var results = new List<FileInfo>();

            foreach (var file in files)
            {
                var fileSize = file.Length;

                if (fileSize > minFileSize)
                {
                    results.Add(file);
                }
            } 

            return results.Count == 0 ? Task.FromResult<List<FileInfo>?>(null) : Task.FromResult<List<FileInfo>?>(results);
        }

        public static Task<List<FileInfo>?> SortByMaxSizeAsync(List<FileInfo> files, long maxFileSize)
        {
            var results = new List<FileInfo>();

            foreach (var file in files)
            {
                var fileSize = file.Length;

                if (fileSize < maxFileSize)
                {
                    results.Add(file);
                }
            }

            return results.Count == 0 ? Task.FromResult<List<FileInfo>?>(null) : Task.FromResult<List<FileInfo>?>(results);
        }

        public static Task<List<string>?> GetAllFilesPathsFromFileListAsync(List<FileInfo> files)
        {
            var filePaths = new List<string>();

            foreach (var file in files)
            {
                var filePath = file.FullName;
                filePaths.Add(filePath);
            }
            return Task.FromResult<List<string>?>(filePaths);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public static Task ReplaceImageAsync(string destinationPath, Image processedImage)
        {
            File.Delete(destinationPath);
            processedImage.Save(destinationPath);
            return Task.CompletedTask;
        }

    }
}
