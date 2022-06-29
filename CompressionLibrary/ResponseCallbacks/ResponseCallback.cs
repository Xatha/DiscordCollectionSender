using System;

namespace CompressionLibrary
{
    public sealed class ResponseCallback : IResponseCallback
    {
        public Progress<String> FilePath { get; set; } = new Progress<String>();
    }
}