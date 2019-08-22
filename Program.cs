//
// DocGen Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Configuration;
using JEM.Core.Debugging;
using System;
using System.IO;

namespace Overmodded.DocGen
{
    internal class Program
    {
        private static void Main()
        {          
            // update window
            Console.Title = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);

            // set configuration directory
            var workDir = Environment.CurrentDirectory;
            JEMConfiguration.CurrentDirectory = workDir;

            // set up logger
            JEMLogger.SilentMode = false;
            JEMLogger.Init();
            JEMLogger.LoggerCustomRootDirectory = workDir;
            JEMLogger.ClearLoggerDirectory();
            JEMLogger.OnLogAppended += (source, type, value, stacktrace) =>
            {
                ConsoleColor previousColor = Console.ForegroundColor;
                string prefixes = $"[{DateTime.Now:T}] {source.ToUpper()}";
                string message;
                switch (type)
                {
                    case JEMLogType.Log:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        message = $"{prefixes} {value}";
                        stacktrace = string.Empty;
                        break;
                    case JEMLogType.Warning:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        message = $"{prefixes} WARN {value}";
                        stacktrace = string.Empty;
                        break;
                    case JEMLogType.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        message = $"{prefixes} ERR {value}";
                        stacktrace = string.Empty;
                        break;
                    case JEMLogType.Exception:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine();

                        message = $"{prefixes} FATAL {value}";
                        if (string.IsNullOrWhiteSpace(stacktrace))
                        {
                            stacktrace = "Stacktrace is not available for this element.";
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }

                Console.WriteLine(message);
                if (!string.IsNullOrEmpty(stacktrace))
                {
                    Console.WriteLine(stacktrace);
                }

                Console.ForegroundColor = previousColor;
            };

            JEMLogger.Log($"Current game work directory is `{workDir}`.", "APP");

            Generator.Run();

            Console.WriteLine("Press any key to close.");
            Console.Read();
        }
    }
}
