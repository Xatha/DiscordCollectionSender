using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionLibrary
{
    public static class Factory
    {
        //Refrain from using this, since it increases coupling.
        internal static ICompressionRatioGenerator CoupledCreate<T>(T instance) where T : ICompressionRatioGenerator
        {
            return instance;
        }

        internal static ICompressionRatioGenerator CreateCompressionRatio(long fileSizeInBytes, long targetSize, long tolerance = 524288)
        {
            return new CompressionRatioGenerator(fileSizeInBytes, targetSize, tolerance);
        }

        internal static Task<ICompressionRatioGenerator> CreateCompressionRatioAsync(long fileSizeInBytes, long targetSize, long tolerance = 524288)
        {
            return Task.FromResult<ICompressionRatioGenerator>(new CompressionRatioGenerator(fileSizeInBytes, targetSize, tolerance));
        }
    }
}
