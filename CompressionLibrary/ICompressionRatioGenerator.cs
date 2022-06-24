namespace CompressionLibrary
{
    public interface ICompressionRatioGenerator
    {
        long FileSizeInBytes { set; }
        double Ratio { get; }
        long TargetSize { set; }
        long Tolerance { set; }

        double GenerateCompressionRatio(long fileSizeInBytes, long targetSize, long tolerance = 524288);
        double GenerateCompressionRatio(long fileSizeInBytes);
        Task<Double> GenerateCompressionRatioAsync(long fileSizeInBytes);

    }
}