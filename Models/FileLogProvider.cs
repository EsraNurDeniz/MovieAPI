using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace MovieApi.Models
{
    public class fileLogProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string category)
        {
            return new FileLogger();
        }
        public void Dispose()
        {
            
        }
    }
}