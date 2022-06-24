using System.Collections.Generic;
using LoggerLibrary;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;
using UtilsLibrary;
using static System.Net.Mime.MediaTypeNames;

namespace CompressionLibrary
{
    public class ImageCompressor 
    {
        private static readonly Logger logger = new Logger();

        private const long _8MbInMebiBytes = (long)8.389e+6;
        private const string CallerName = nameof(ImageCompressor);

        private ICompressionRatioGenerator compressionRatioGenerator;

        private List<FileInfo> imageFiles = new List<FileInfo>();
        private ResponseCallback responseCallback;
        private readonly long targetFileSize;


        public ImageCompressor(List<FileInfo> imageFiles, ResponseCallback responseCallback, long targetFileSize = _8MbInMebiBytes)
        {
            this.imageFiles = imageFiles;
            this.responseCallback = responseCallback;
            this.targetFileSize = targetFileSize;
            compressionRatioGenerator = Factory.CreateCompressionRatio(-1, targetFileSize);
        }

        //public static Task<ImageCompressor> CreateAsync(List<FileInfo> imageFiles, ResponseCallback responseCallback, long targetFileSize = _8MbInMebiBytes)
        //{
        //    var _instance = new ImageCompressor(imageFiles, responseCallback, targetFileSize);
           
        //    return Task.FromResult(_instance);
        //}

        //Add exception for .mp4 and movie.
        public async Task StartCompressionAsync()
        {
            //AddChecks.

            await CompressToTargetSize(responseCallback.FilePath);
            Console.WriteLine("=====================================Completed========================");
            return;
        }


        private async Task CompressToTargetSize(IProgress<String> progress)
        {
            var pathsOfImagesToProcess = FilesUtil.GetAllFilesPathsFromFileList(imageFiles);

            var passes = 0;

            //Take Time.
            var stopWatch = Stopwatch.StartNew();
            do
            {
                var compressedImages = await CompressImages(progress, pathsOfImagesToProcess);

                foreach (var image in compressedImages)
                {
                    if (await IsFileSizeUnderTargetSize(image.Size))
                    {
                        pathsOfImagesToProcess.Remove(image.Path);
                        progress.Report(image.Path);
                        Console.WriteLine("Removed " + image.Path);
                    }
                }
                await logger.Log($"Passes: [{passes}]", CallerName);
                passes++;
            } while (pathsOfImagesToProcess.Count > 0);

            stopWatch.Stop();
            Console.WriteLine($"Time taken: {stopWatch.Elapsed.ToString(@"m\:ss\.fff")}");
            return;
        }

        private async Task<List<(string Path, long Size)>> CompressImages(IProgress<string> progress, List<string> pathsOfImagesToProcess)
        {
            List<(string Path, long Size)> result = new();

            foreach (var filePath in pathsOfImagesToProcess)
            {
                await logger.Log($"Processing {filePath}.", CallerName);

                long fileSize = new FileInfo(filePath).Length;

                //Creates a compressed image and then replaces it. 
                using (var processedImage = await CompressImage((filePath, fileSize)))
                {
                    await FilesUtil.ReplaceImageAsync(filePath, processedImage);
                }

                long newFileSize = new FileInfo(filePath).Length;

                await logger.Log($"Previous Size: [{fileSize}] | New Size: [{newFileSize}] | Compression ratio: [{compressionRatioGenerator.Ratio}]", CallerName);
                
                result.Add((filePath, newFileSize));
            }

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private async Task<Bitmap> CompressImage((string filePath, long fileSize) file)
        {
            var ratio = await compressionRatioGenerator.GenerateCompressionRatioAsync(file.fileSize);

            using (var img = System.Drawing.Image.FromFile(file.filePath))
            {
                var task = await ScaleImage(img, ratio);
                return task;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private Task<Bitmap> ScaleImage(System.Drawing.Image image, double ratio)
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
        private Task<bool> IsFileSizeUnderTargetSize(long newFileSize) => Task.FromResult(newFileSize <= targetFileSize);

    }
}