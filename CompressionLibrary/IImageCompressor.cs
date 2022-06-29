
namespace CompressionLibrary
{
    public interface IImageCompressor : IDisposable
    {
        Task StartCompressionAsync();
    }
}