using System.Collections.Generic;
using LoggerLibrary;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;
using static System.Net.Mime.MediaTypeNames;

namespace CompressionLibrary
{
    public static class CompressionHandler
    {
        private static readonly Logger logger = new Logger();

        private const long _8MbInMebiBytes = (long)8.389e+6;
        private const string CallerName = "CompressionHandler";

        //Bad naming. Function compress files to <8MB

        //Add exception for .mp4 and movie files
        public static async Task InitAsync(List<FileInfo> files, ResponseCallback responseCallback)
        {
            var filesOver8Mb = GetAllFilesAboveSize(files, _8MbInMebiBytes);

            if (filesOver8Mb == null)
            {
                await logger.Log("No files have to be compressed.", CallerName);
                return;
            }

            await CompressFiles(responseCallback.FilePath, filesOver8Mb);
        }

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

                if (fileSize <= maxFileSize)
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


        //Calculate file compression with following eq. 
        // z(x) = 1/log2(8_000_000) * log2(x), where z(x) is compression ratio.

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private static async Task CompressFiles(IProgress<String> progress, List<string> filePaths)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var count = 1;

            var areAllImagesUnder8mb = false;

            while (!areAllImagesUnder8mb)
            {
                if (filePaths == null || filePaths.Count <= 0)
                {
                    break;
                }

                foreach (var filePath in filePaths)
                {
                    await logger.Log($"Processing {filePath}.", CallerName);

                    long fileSize = new FileInfo(filePath).Length;

                    using (var processedImage = Task.Run(() => CompressImage((filePath, fileSize))))
                    {
                        ReplaceImage(filePath, processedImage.Result);
                    }

                    long newFileSize = await Task.Run(() => new FileInfo(filePath).Length);

                    await logger.Log($"Previous Size: [{fileSize}] | New Size: [{newFileSize}] | Compression ratio: [{GetCompressionRatio(fileSize)}]", CallerName);

                    if (newFileSize <= _8MbInMebiBytes)
                    {
                        areAllImagesUnder8mb = true;

                        var currentIndex = filePath.IndexOf(filePath);
                        filePath.Remove(currentIndex);

                        progress.Report(filePath);
                    }
                    else
                    {
                        areAllImagesUnder8mb = false;
                    }
                }

                await logger.Log($"Passes: [{count}]", CallerName);
                count++;
            }
            stopWatch.Stop();
            Console.WriteLine($"Time taken: {stopWatch.Elapsed.ToString(@"m\:ss\.fff")}");
        }

        private static void ReplaceImage(string destinationPath, System.Drawing.Image processedImage)
        {
            File.Delete(destinationPath);
            processedImage.Save(destinationPath);
        }

        private static System.Drawing.Image CompressImage((string FilePath, long FileSize) file)
        {
            var ratio = GetCompressionRatio(file.FileSize);

            using (var img = System.Drawing.Image.FromFile(file.FilePath))
            {
                return ScaleImage(img, ratio);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private static Bitmap ScaleImage(System.Drawing.Image image, double ratio)
        {
            //double ratio = height / image.Height;
            int newWidth = (int)Math.Floor(image.Width * ratio);
            int newHeight = (int)Math.Floor(image.Height * ratio);
            
            Bitmap newImage = new Bitmap(newWidth, newHeight, image.PixelFormat);

            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            image.Dispose();
            return newImage;
        }

        private static double GetCompressionRatio(long fileSizeBytes)
        {
            Int32 tolerance = 524288;
            long targetSize = _8MbInMebiBytes - tolerance;
            double multiplier = Math.Sqrt((targetSize / (double)fileSizeBytes));
            double compressionRatio = (1 / ((double)((1 / Math.Log2(targetSize))) * Math.Log2(fileSizeBytes)) * multiplier);

            return compressionRatio;
        }
    }
}