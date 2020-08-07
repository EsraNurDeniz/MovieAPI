using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace MovieApi.Models
{
  public class FileLogger : ILogger 
  {
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var message = string.Format("{0}: {1} - {2}", logLevel.ToString(), eventId.Id, formatter(state, exception));
        WriteMessageToFile(message);
    }
    private static void WriteMessageToFile(string message)
    {
        const string filePath = "Logs\\LogFile.txt";
        using (var streamWriter = new StreamWriter(filePath, true))
        {
            streamWriter.WriteLine(message);
            streamWriter.Close();
        }
    }
    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }
    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }
    }
}