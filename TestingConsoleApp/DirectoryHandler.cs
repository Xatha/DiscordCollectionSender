using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoggerLibrary;

namespace TestingConsoleApp
{
    
    internal class DirectoryHandler : IDisposable
    {
        private const string CallerName = "DirectoryHandler";
        private static readonly Logger logger = new Logger();

        public static List<string> FilePaths { get; set; } = new List<string>();
        private static DirectoryHandler? instance;
        private bool disposedValue;

        public DirectoryHandler()
        {
            if (instance != null)
            {
                logger.LogError($"There can only exist one {CallerName}.", CallerName);
                throw new Exception($"{CallerName} is singleton. Only one instance can exist.");
            }
            instance = this;
        }

        ~DirectoryHandler()
        {
            instance = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DirectoryHandler()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            
            GC.SuppressFinalize(this);
            GC.Collect();
        }
    }
}
