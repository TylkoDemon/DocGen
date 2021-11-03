//
// DocGen Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.IO;
using JEM.Configuration;
using JEM.Debugging;

namespace DocGen
{
    internal static class Program
    {
        private static void Main()
        {          
            // update window
            Console.Title = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);

            // set configuration directory
            var workDir = Environment.CurrentDirectory;
            ConfigurationFactory.CurrentDirectory = workDir;

            // set up logger
            Logger.LoggerMode = LoggerMode.Normal;
            Logger.ClearLoggerDirectory();
            Logger.OnLogAppended += (in Log log) => 
            {
                ConsoleColor previousColor = Console.ForegroundColor;
                string prefixes = $"[{DateTime.Now:T}] {log.SourceGroup.ToUpper()}";
                string message;
                string stacktrace = string.Empty;
                switch (log.Type)
                {
                    case LogType.Log:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        message = $"{prefixes} {log.Contents}";
                        stacktrace = string.Empty;
                        break;
                    case LogType.Warning:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        message = $"{prefixes} WARN {log.Contents}";
                        stacktrace = string.Empty;
                        break;
                    case LogType.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        message = $"{prefixes} ERR {log.Contents}";
                        stacktrace = string.Empty;
                        break;
                    case LogType.Exception:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine();

                        message = $"{prefixes} FATAL {log.Contents}";
                        if (string.IsNullOrWhiteSpace(stacktrace))
                        {
                            stacktrace = "Stacktrace is not available for this element.";
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(log.Type), log.Type, null);
                }

                Console.WriteLine(message);
                if (!string.IsNullOrEmpty(stacktrace))
                {
                    Console.WriteLine(stacktrace);
                }

                Console.ForegroundColor = previousColor;
            };

            Logger.Log($"Current game work directory is `{workDir}`.", "APP");

            Generator.Run();

            Console.WriteLine("Press any key to close.");
            Console.Read();
        }

        internal static void Process(string str, bool warn = false)
        {
            if (AppConfig.Loaded.LogProcessing)
            {
                if (warn)
                    Logger.LogWarning(str, "PROCESSING");
                else
                {
                    Logger.Log(str, "PROCESSING");
                }
            }
        }
    }
}
