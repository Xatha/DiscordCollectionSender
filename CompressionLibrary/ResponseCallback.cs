using System;

namespace CompressionLibrary
{
    public class ResponseCallback 
    {
        public Progress<String> FilePath { get; set; } = new Progress<String>();

    }
}