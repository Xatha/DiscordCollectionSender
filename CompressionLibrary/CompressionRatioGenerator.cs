namespace CompressionLibrary
{
    public struct CompressionRatioGenerator : ICompressionRatioGenerator
    {
        public double PreviousRatio { get; private set; } = 0;

        public CompressionRatioGenerator()
        {

        }
        //Calculate file compression with following eq. 
        // z(x) = 1/log2(8_000_000) * log2(x), where z(x) is compression ratio.
        public ValueTask<double> GenerateCompressionRatioAsync(long fileSizeInBytes, long targetSize, long tolerance = 524288)
        {
            long adjustedTargetSize = targetSize - tolerance;

            double multiplier = Math.Sqrt((adjustedTargetSize / (double)fileSizeInBytes));
            double compressionRatio = (1 / ((double)((1 / Math.Log2(adjustedTargetSize))) * Math.Log2(fileSizeInBytes)) * multiplier);

            PreviousRatio = compressionRatio;

            return ValueTask.FromResult(compressionRatio);
        }
    }
}
