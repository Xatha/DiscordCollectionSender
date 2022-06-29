
namespace CompressionLibrary
{
    public interface IResponseCallback
    {
        Progress<string> FilePath { get; set; }
    }
}