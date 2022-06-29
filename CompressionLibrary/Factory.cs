using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionLibrary
{
    public static class Factory
    {
        public async static Task<IImageCompressor> CreateImageCompressorAsync(List<FileInfo> imageFiles, IResponseCallback responseCallback, long targetFileSize = (long)8.389e+6)
        {
            return await ImageCompressor.CreateAsync(imageFiles, responseCallback, targetFileSize);
        }
        internal static ICompressionRatioGenerator CreateCompressionRatio()
        {
            return new CompressionRatioGenerator();
        }

        public static Task<IResponseCallback> CreateResponseCallbackAsync()
        {
            return Task.FromResult<IResponseCallback>(new ResponseCallback());
        }
    }
}
