using System.Collections.Generic;
using LoggerLibrary;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;
using UtilsLibrary;
using static System.Net.Mime.MediaTypeNames;
using log4net;
using DiscordCollectionSenderBot.Logger;

namespace CompressionLibrary
{
    public sealed class ImageCompressor : IDisposable, IImageCompressor
    {
        private const long _8MbInMebiBytes = (long)8.389e+6;

        private static readonly ILog _logger = LogAsync.GetLogger();
        private static readonly ICompressionRatioGenerator _compressionRatioGenerator = Factory.CreateCompressionRatio();

        private readonly List<FileInfo> _imageFiles = new List<FileInfo>();
        private readonly IResponseCallback _responseCallback;
        private readonly long _targetFileSize;

        private ImageCompressor(List<FileInfo> imageFiles, IResponseCallback responseCallback, long targetFileSize = _8MbInMebiBytes)
        {
            this._imageFiles = imageFiles;
            this._responseCallback = responseCallback;
            this._targetFileSize = targetFileSize;
        }

        public static Task<ImageCompressor> CreateAsync(List<FileInfo> imageFiles, IResponseCallback responseCallback, long targetFileSize = _8MbInMebiBytes)
        {
            var imageCompressor = new ImageCompressor(imageFiles, responseCallback, targetFileSize);
            return imageCompressor.InitAsync();
        }

        private Task<ImageCompressor> InitAsync()
        {
            return Task.FromResult(this);
        }

        public async Task StartCompressionAsync()
        {
            //Take Time.
            var stopWatch = Stopwatch.StartNew();
            await CompressImagesToTargetSize(_responseCallback.FilePath);
            stopWatch.Stop();

            await _logger.DebugAsync($"Compression Completed.\n Time taken: {stopWatch.Elapsed.ToString(@"m\:ss\.fff")}");
        }

        private async Task CompressImagesToTargetSize(IProgress<String> progress)
        {
            var pathsOfImagesToProcess = FilesUtil.GetAllFilesPathsFromFileList(_imageFiles);

            var passes = 0;

            do
            {
                var compressedImages = await CompressImages(pathsOfImagesToProcess);

                foreach (var image in compressedImages)
                {
                    if (await IsFileSizeUnderTargetSize(image.Size))
                    {
                        pathsOfImagesToProcess.Remove(image.Path);

                        //Report what image was removed.
                        progress.Report(image.Path);
                    }
                }
                await _logger.DebugAsync($"Passes: [{passes}]");
                passes++;
            } while (pathsOfImagesToProcess.Count > 0);
        }

        private async Task<List<(string Path, long Size)>> CompressImages(List<string> images)
        {
            List<(string Path, long Size)> results = new();

            foreach (var filePath in images)
            {
                await _logger.InfoAsync($"Processing {filePath}.");

                long fileSize = new FileInfo(filePath).Length;

                //Creates a compressed image and then replaces it. 
                using (var processedImage = await CompressImageAsync((filePath, fileSize)))
                {
                    await FilesUtil.ReplaceImageAsync(filePath, processedImage);
                }

                long newFileSize = new FileInfo(filePath).Length;

                results.Add((filePath, newFileSize));
                await _logger.DebugAsync($"Previous Size: [{fileSize}] | New Size: [{newFileSize}] | Compression ratio: [{_compressionRatioGenerator.PreviousRatio}]");
            }

            return results;
        }

        private Task<bool> IsFileSizeUnderTargetSize(long newFileSize) => Task.FromResult(newFileSize <= _targetFileSize);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private static async Task<Bitmap> CompressImageAsync((string filePath, long fileSize) file)
        {
            var ratio = await _compressionRatioGenerator.GenerateCompressionRatioAsync(file.fileSize, _8MbInMebiBytes);

            //Compresses the image by scaling it.
            using (var img = System.Drawing.Image.FromFile(file.filePath))
            {
                var image = await ScaleImage(img, ratio);
                return image;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private static Task<Bitmap> ScaleImage(System.Drawing.Image image, double ratio)
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
            return Task.FromResult(newImage);
        }

        public void Dispose()
        {

        }
    }
}