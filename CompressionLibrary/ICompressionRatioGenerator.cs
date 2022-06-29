
namespace CompressionLibrary
{
    public interface ICompressionRatioGenerator
    {
        public double PreviousRatio { get; }
        ValueTask<double> GenerateCompressionRatioAsync(long fileSizeInBytes, long targetSize, long tolerance = 524288);
    }
}