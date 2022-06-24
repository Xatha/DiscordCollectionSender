using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionLibrary
{
    public class CompressionRatioGenerator : ICompressionRatioGenerator
    {
        public double Ratio
        {
            get => GenerateCompressionRatio(_fileSizeInBytes, _targetSize, _tolerance);
        }

        public long FileSizeInBytes
        {
            set => _fileSizeInBytes = value;
        }
        public long TargetSize
        {
            set => _targetSize = value;
        }
        public long Tolerance
        {
            set => _tolerance = value;
        }

        private long _fileSizeInBytes;
        private long _targetSize;
        private long _tolerance;

        public CompressionRatioGenerator(long fileSizeInBytes, long targetSize, long tolerance = 524288)
        {
            this._fileSizeInBytes = fileSizeInBytes;
            this._targetSize = targetSize;
            this._tolerance = tolerance;
        }

        //Calculate file compression with following eq. 
        // z(x) = 1/log2(8_000_000) * log2(x), where z(x) is compression ratio.
        public double GenerateCompressionRatio(long fileSizeInBytes, long targetSize, long tolerance = 524288)
        {
            _fileSizeInBytes = fileSizeInBytes;
            long adjustedTargetSize = targetSize - tolerance;

            double multiplier = Math.Sqrt((adjustedTargetSize / (double)fileSizeInBytes));
            double compressionRatio = (1 / ((double)((1 / Math.Log2(adjustedTargetSize))) * Math.Log2(fileSizeInBytes)) * multiplier);
            return compressionRatio;
        }

        public double GenerateCompressionRatio(long fileSizeInBytes)
        {
            _fileSizeInBytes = fileSizeInBytes;
            long adjustedTargetSize = _targetSize - _tolerance;

            double multiplier = Math.Sqrt((adjustedTargetSize / (double)fileSizeInBytes));
            double compressionRatio = (1 / ((double)((1 / Math.Log2(adjustedTargetSize))) * Math.Log2(fileSizeInBytes)) * multiplier);
            return compressionRatio;
        }

        public async Task<double> GenerateCompressionRatioAsync(long fileSizeInBytes)
        {
            var compressionRatio = await Task.Run(() => GenerateCompressionRatio(fileSizeInBytes));
            return compressionRatio;
        }
    }
}
