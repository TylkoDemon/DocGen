//
// DocGen Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using JEM;
using JEM.Debugging;

namespace DocGen
{
    internal static class Generator
    {
        internal static void Run()
        {
            Logger.Log("Loading appcfg.", "APP");
            AppConfig.LoadConfiguration();

            if (!Directory.Exists(AssembliesDirectory))
            {
                Logger.LogError($"Assemblies directory `{AssembliesDirectory}` does not exist.", "APP");
                return;
            }

            var assembliesTargets = AssembliesDirectory + EnvironmentUtility.DirectorySeparator + AppConfig.Loaded.AssembliesTargetsFile;
            if (!File.Exists(assembliesTargets))
            {
                Logger.LogError($"File defining target assemblies `{assembliesTargets}` does not exist.", "APP");
                return;
            }

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                Logger.LogError("Failed to resolve assembly " + args.Name, "APP");
                return Assembly.LoadFrom(args.Name);
            };

            AssembliesNames = File.ReadAllLines(assembliesTargets);
            Logger.Log($"{AssembliesNames.Length} target assemblies found.", "APP");
            foreach (var a in AssembliesNames) Logger.Log($"\t{a}", "APP");
            Console.WriteLine();

            if (Directory.Exists(DeployDirectory))
                Directory.Delete(DeployDirectory, true);

            Directory.CreateDirectory(DeployDirectory);

            foreach (var a in AssembliesNames)
            {
                var file = AssembliesDirectory + EnvironmentUtility.DirectorySeparator + a + ".dll";
                AssemblyName assemblyName = null;
                try
                {
                    assemblyName = AssemblyName.GetAssemblyName(file);
                }
                catch (Exception e)
                {
                    Logger.LogException(e, "APP");
                }

                var assembly = assemblyName == null ? null : Assembly.Load(assemblyName);
                if (assembly == null)
                    Logger.LogError($"Unable to load assembly `{file}`", "APP");
                else
                {
                    Logger.Log($"Assembly {file} loaded..", "APP");
                    LoadedAssemblies.Add(assembly);
                }
            }

            Logger.Log("Generating.", "APP");
            foreach (var assembly in LoadedAssemblies)
            {
                try
                {
                    GenerateOffAssembly(assembly);
                }
                catch (TypeLoadException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private static void GenerateOffAssembly(Assembly assembly)
        {  
            var file = new Uri(assembly.CodeBase).AbsolutePath;
            Logger.Log($"Generating doc of { Path.GetFileName(file)} ({assembly.GetTypes().Length} unique types to process)", "APP");

            var xmlFile = Path.GetDirectoryName(file) + EnvironmentUtility.DirectorySeparator +
                          Path.GetFileNameWithoutExtension(file) + ".xml";
            var doc = new XmlDocument();
            doc.Load(xmlFile);

            var b = new DocBuilder(assembly, doc, DeployDirectory, ExamplesDirectory);
            var files = b.Build();

            Logger.Log($"Done! ({files.Count} .md files generated)", "APP");
        }

        internal static List<Assembly> LoadedAssemblies { get; } = new List<Assembly>();

        internal static string[] AssembliesNames { get; private set; } = new string[0];

        internal static string AssembliesDirectory =>
            Environment.CurrentDirectory + EnvironmentUtility.DirectorySeparator + AppConfig.Loaded.AssembliesDir;

        internal static string DeployDirectory => AppConfig.Loaded.DeployDir;

        internal static string ExamplesDirectory =>
            Environment.CurrentDirectory + EnvironmentUtility.DirectorySeparator + AppConfig.Loaded.ExamplesDir;
    }
}
