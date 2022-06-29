using LoggerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCollectionSenderBot
{
    internal abstract class SingletonBase : IDisposable
    {
        private static SingletonBase? instance;
        public SingletonBase(string callerName)
        {
            if (instance != null)
            {
                LoggerLibrary.Logger logger = new LoggerLibrary.Logger();

                logger.LogError($"There can only exist one {callerName}.", callerName);
                throw new Exception($"{callerName} is singleton. Only one instance of {callerName} can exist.");
            }

            instance = this;
        }

        public void Dispose()
        {
            instance!.Dispose();
        }
    }
}
