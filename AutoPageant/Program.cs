// -----------------------------------------------------------------------------
//  <copyright file="Program.cs" company="MultiPlayerForums.com">
//      Copyright (c) MultiPlayerForums.com All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace AutoPageant
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                ShowUsage();
                Exit(1);
            }

            if (!ExistsOnPath("pageant.exe"))
            {
                Error("Pageant not found in %PATH%");
                Exit(4);
            }

            string path = args[0];
            if (File.Exists(path) || !Directory.Exists(path))
            {
                ShowUsage();
                Exit(2);
                return;
            }

            try
            {
                var keyFiles = Directory.GetFiles(path, "*.ppk").AsEnumerable().Select(x => string.Format("\"{0}\"", x)).ToArray();

                if (keyFiles.Length < 1)
                {
                    Error("No key files were found at the specified path: {0}", path);
                    return;
                }

                string keyList = string.Join(" ", keyFiles);

                var info = new ProcessStartInfo("pageant", keyList);

                Process.Start(info);
            }
            catch (PathTooLongException)
            {
                Error("The specified path is too long.");
            }

            Exit(0);
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage: {0} <directory>", AppDomain.CurrentDomain.FriendlyName);
        }

        private static void Exit(int exitCode)
        {
            Console.ReadKey();

            Environment.Exit(exitCode);
        }

        private static void Error(string message, params object[] args)
        {
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message, args);
            Console.ForegroundColor = c;
            Exit(3);
        }
        
        // Credit: http://stackoverflow.com/a/3856090/63609 (ExistsOnPath & GetFullPath)
        public static bool ExistsOnPath(string fileName)
        {
            return GetFullPath(fileName) != null;
        }

        public static string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");

            return values != null ? values.Split(';').Select(path => Path.Combine(path, fileName)).FirstOrDefault(File.Exists) : null;

        }
    }
}
