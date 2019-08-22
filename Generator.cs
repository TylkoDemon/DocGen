//
// DocGen Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core;
using JEM.Core.Debugging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Overmodded.DocGen
{
    internal static class Generator
    {
        internal static void Run()
        {
            JEMLogger.Log("Loading appcfg.", "APP");
            AppConfig.LoadConfiguration();

            if (!Directory.Exists(AssembliesDirectory))
            {
                JEMLogger.LogError($"Assemblies directory `{AssembliesDirectory}` does not exist.", "APP");
                return;
            }

            var assembliesTargets = AssembliesDirectory + JEMVar.DirectorySeparatorChar + AppConfig.Loaded.AssembliesTargetsFile;
            if (!File.Exists(assembliesTargets))
            {
                JEMLogger.LogError($"File defining target assemblies `{assembliesTargets}` does not exist.", "APP");
                return;
            }

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                JEMLogger.LogError("Failed to resolve assembly " + args.Name, "APP");
                return Assembly.LoadFrom(args.Name);
            };

            AssembliesNames = File.ReadAllLines(assembliesTargets);
            JEMLogger.Log($"{AssembliesNames.Length} target assemblies found.", "APP");
            foreach (var a in AssembliesNames) JEMLogger.Log($"\t{a}", "APP");
            Console.WriteLine();

            if (Directory.Exists(DeployDirectory))
                Directory.Delete(DeployDirectory, true);

            Directory.CreateDirectory(DeployDirectory);

            foreach (var a in AssembliesNames)
            {
                var file = AssembliesDirectory + JEMVar.DirectorySeparatorChar + a + ".dll";
                AssemblyName assemblyName = null;
                try
                {
                    assemblyName = AssemblyName.GetAssemblyName(file);
                }
                catch (Exception e)
                {
                    JEMLogger.LogException(e, "APP");
                }

                var assembly = assemblyName == null ? null : Assembly.Load(assemblyName);
                if (assembly == null)
                    JEMLogger.LogError($"Unable to load assembly `{file}`", "APP");
                else
                {
                    JEMLogger.Log($"Assembly {file} loaded..", "APP");
                    LoadedAssemblies.Add(assembly);
                }
            }

            JEMLogger.Log("Generating.", "APP");
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
            JEMLogger.Log($"Generating doc of { Path.GetFileName(file)} ({assembly.GetTypes().Length} unique types to process)", "APP");

            var xmlFile = Path.GetDirectoryName(file) + JEMVar.DirectorySeparatorChar +
                          Path.GetFileNameWithoutExtension(file) + ".xml";
            var doc = new XmlDocument();
            doc.Load(xmlFile);

            var b = new DocBuilder(assembly, doc, DeployDirectory, ExamplesDirectory);
            var files = b.Build();

            JEMLogger.Log($"Done! ({files.Count} .md files generated)", "APP");
        }

        internal static List<Assembly> LoadedAssemblies { get; } = new List<Assembly>();

        internal static string[] AssembliesNames { get; private set; } = new string[0];

        internal static string AssembliesDirectory =>
            Environment.CurrentDirectory + JEMVar.DirectorySeparatorChar + AppConfig.Loaded.AssembliesDir;

        internal static string DeployDirectory => AppConfig.Loaded.DeployDir;

        internal static string ExamplesDirectory =>
            Environment.CurrentDirectory + JEMVar.DirectorySeparatorChar + AppConfig.Loaded.ExamplesDir;
    }
}
