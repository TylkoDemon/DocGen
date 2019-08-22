//
// DocGen Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Configuration;
using System;

namespace Overmodded.DocGen
{
    /// <summary>
    ///     DocGen main configuration file.
    /// </summary>
    [Serializable]
    public class AppConfig
    {
        /// <summary>
        ///     Name of the config file.
        /// </summary>
        public const string FileName = "appcfg";

        /// <summary>
        ///     If true, every DocGen action will be printed.
        /// </summary>
        public bool LogProcessing = false;

        /// <summary>
        ///     Path to README.md file that will be copied after Doc Gen.
        /// </summary>
        public string ReadMe = "style/README.md";

        /// <summary>
        ///     Name of tree in _sidebar.md.
        /// </summary>
        public string TreeTitleName = "DocGen Powered by Overmodded.DocGenerator";

        /// <summary>
        ///     Name of _sidebar.md file.
        /// </summary>
        public string SidebarName = "_sidebar.md";

        /// <summary>
        ///     Markdown content to insert before DocGen content.
        /// </summary>
        public string SidebarBefore = "style/_sidebarBefore.md";

        /// <summary>
        ///     Markdown content to insert after DocGen content.
        /// </summary>
        public string SidebarEnd = "style/_sidebarEnd.md";

        /// <summary>
        ///     Markdown content to insert at the end of every file.
        /// </summary>
        public string FileEnd = "style/_fileEnd.md";

        /// <summary>
        ///     Directory to assemblies to generate from.
        /// </summary>
        public string AssembliesDir = "Assemblies";

        /// <summary>
        ///     Name of .txt files that tells from what files DocGen should generate.
        /// </summary>
        public string AssembliesTargetsFile = "targets.txt";

        /// <summary>
        ///     Generated content directory path.
        /// </summary>
        public string DeployDir = "Deploy";

        /// <summary>
        ///     Generated content directory path to insert in sidebar.
        /// </summary>
        public string DeploySidebarPath = "_deploy";

        /// <summary>
        ///     Directory to example files.
        /// </summary>
        public string ExamplesDir = "Examples";

        /// <summary>
        ///     Array of namespaces that are disallowed by DocGen to mention in any aspect in files.
        /// </summary>
        public string[] DisallowedNamespaces = {
            "UnityEngine"
        };

        /// <summary>
        ///     List of types (classes) that are disallowed to mention.
        ///     NOTE: Currently only methods uses this field (MethodInfo.DeclaringType)
        /// </summary>
        public string[] DisallowedDeclarationTypes = {
            typeof(object).FullName
        };

        /// <summary>
        ///     Array of types that are not allowed by DocGen to mention.
        /// </summary>
        public string[] DisallowedTypes = {
            typeof(object).FullName
        };

        /// <summary>
        ///     Load configuration.
        /// </summary>
        internal static void LoadConfiguration()
        {
            Loaded = JEMConfiguration.LoadData<AppConfig>(JEMConfiguration.ResolveFilePath(FileName));
        }

        /// <summary>
        ///     Loaded application configuration data.
        /// </summary>
        internal static AppConfig Loaded { get; private set; }
    }
}
