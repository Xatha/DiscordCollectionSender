﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoggerLibrary;

namespace DiscordCollectionSenderBot
{
    
    internal class DirectoryHandler : SingletonBase
    {
        private const string callerName = nameof(DirectoryHandler);
        private static readonly LoggerLibrary.Logger logger = new LoggerLibrary.Logger();

        public static List<string> FilePaths { get; set; } = new List<string>();

        public DirectoryHandler() : base(callerName)
        {

        }


    }
}
