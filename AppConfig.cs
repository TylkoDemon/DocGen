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
    internal class AppConfig
    {
        public const string FileName = "appcfg";

        public string ReadMe = "style/README.md";
        public string TreeTitleName = "DocGen Powered by Overmodded.DocGenerator";

        public string SidebarName = "_sidebar.md";
        public string SidebarBefore = "style/_sidebarBefore.md";
        public string SidebarEnd = "style/_sidebarEnd.md";

        public string FileEnd = "style/_fileEnd.md";

        public string AssembliesDir = "Assemblies";
        public string AssembliesTargetsFile = "targets.txt";

        public string DeployDir = "Deploy";
        public string DeploySidebarPath = "_deploy";

        public string ExamplesDir = "Examples";

        public string[] DisallowedNamespaces = {
            "UnityEngine"
        };

        public string[] DisallowedDeclarationTypes = {
            typeof(object).FullName
        };

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
