﻿//
// DocGen Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using JEM;

namespace DocGen
{
    internal class DocBuilder
    {
        internal Assembly Assembly { get; }
        internal XmlDocument Document { get; }
        internal string DeployDir { get; }
        internal string ExamplesDir { get; }

        private readonly XmlNodeList _nodes;

        internal DocBuilder(Assembly a, XmlDocument doc, string deployDir, string examplesDir)
        {
            Assembly = a;
            Document = doc;
            DeployDir = deployDir;
            ExamplesDir = examplesDir;

            _nodes = doc.DocumentElement?.SelectNodes("/doc/members/member") ?? throw new NullReferenceException();
        }

        internal List<string> Build()
        {
            var allTypes = Assembly.GetTypes();
            var buildFiles = new List<string>();
            foreach (var type in allTypes)
            {
                Program.Process($"Generating from type: {type.FullName}");
                var fileName = BuildType(type);
                if (string.IsNullOrEmpty(fileName))
                {
                    Program.Process($"Target type ({type.Name}) refused to generate content.", true);
                    continue;
                }

                buildFiles.Add(fileName);
                Program.Process($"Content successfully generated from: {type.FullName}");
            }

            Program.Process($"Generating Sidebar from {buildFiles.Count} files.");

            // TODO: Support multiple assemblies
            var str = new StringBuilder();
            if (File.Exists(AppConfig.Loaded.SidebarBefore))
            {
                str.AppendLine(File.ReadAllText(AppConfig.Loaded.SidebarBefore));
                str.AppendLine();
            }

            str.AppendLine($"- {AppConfig.Loaded.TreeTitleName}");
            foreach (var f in buildFiles)
            {
                str.AppendLine($" - [{f}]({DocSyntax.GetMarkdownFile($"{AppConfig.Loaded.DeploySidebarPath}{f}")})");
            }

            if (File.Exists(AppConfig.Loaded.SidebarEnd))
            {
                str.AppendLine();
                str.AppendLine(File.ReadAllText(AppConfig.Loaded.SidebarEnd));
            }

            WriteAllText($"{DeployDir}{EnvironmentUtility.DirectorySeparator}{AppConfig.Loaded.SidebarName}", str.ToString());

            if (File.Exists(AppConfig.Loaded.ReadMe))
            {
                File.Copy(AppConfig.Loaded.ReadMe, $"{DeployDir}{EnvironmentUtility.DirectorySeparator}README.md");
            }

            return buildFiles;
        }

        private string BuildType(Type t)
        {
            if (!t.IsVisible)
            {
                Program.Process($"Target type ({t.Name}) is not visible!", true);
                return null;
            }

            if (DocBuilderHelper.IsTypeArrayContainsNotAllowed(t))
            {
                Program.Process($"Target type ({t.Name}) is not allowed to be generated!", true);
                return null;
            }

            var fileName = DocSyntax.CollectTypeName(t);
            var filePath = DocSyntax.GetMarkdownFile($"{DeployDir}{EnvironmentUtility.DirectorySeparator}{fileName}");
            if (DocSyntax.FixVarName(ref filePath))
            {
                Program.Process($"Target type ({t.Name}) has invalid filePath ({filePath}).", true);
                return null;
            }

            var str = new StringBuilder();
            var subStr = new StringBuilder();

            if (t.IsClass)
            {
                Program.Process($"New class: {t.Name}");

                // HEADER
                str.AppendLine($"# {t.Name}");
                if (t.BaseType != null && t.BaseType != typeof(object))
                {
                    str.AppendLine($"<small>class in `{Path.GetFileNameWithoutExtension(new Uri(t.Assembly.CodeBase).AbsolutePath)}` " +
                                   $"/ inherits from {DocClassUtil.GetTypeMarkdown(t.BaseType, false)}</small>");
                }
                else
                {
                    str.AppendLine($"<small>{(t.IsAbstract && t.IsSealed ? "static " : string.Empty)}class in `{Path.GetFileNameWithoutExtension(new Uri(t.Assembly.CodeBase).AbsolutePath)}`</small>");
                }
                str.AppendLine(string.Empty);

                // SUMMARY
                DocXmlUtil.CollectSummaryFromNode(_nodes, t.FullName, out var classSummary);
                str.AppendLine("### Description");
                str.AppendLine(classSummary);

                // EVENTS
                DocBuilderHelper.BuildEvents(t, str, _nodes, TypeContent.Target, false);
                // FIELDS
                DocBuilderHelper.BuildFields(t, str, _nodes, TypeContent.Target, false);
                // PROPERTIES
                DocBuilderHelper.BuildProperties(t, str, _nodes, TypeContent.Target, false);
                // METHODS
                DocBuilderHelper.BuildMethods(t, str, _nodes, TypeContent.Target, false);

                // STATIC EVENTS
                DocBuilderHelper.BuildEvents(t, str, _nodes, TypeContent.Target, true);
                // STATIC FIELDS
                DocBuilderHelper.BuildFields(t, str, _nodes, TypeContent.Target, true);
                // STATIC PROPERTIES
                DocBuilderHelper.BuildProperties(t, str, _nodes, TypeContent.Target, true);
                // STATIC METHODS
                DocBuilderHelper.BuildMethods(t, str, _nodes, TypeContent.Target, true);

                subStr.Clear();
                // INHERITED MEMBERS
                // EVENTS
                DocBuilderHelper.BuildEvents(t, str, _nodes, TypeContent.Inherited, false);
                // INHERITED MEMBERS
                // FIELDS
                DocBuilderHelper.BuildFields(t, subStr, _nodes, TypeContent.Inherited, false);
                // INHERITED MEMBERS
                // PROPERTIES
                DocBuilderHelper.BuildProperties(t, subStr, _nodes, TypeContent.Inherited, false);
                // INHERITED MEMBERS
                // METHODS
                DocBuilderHelper.BuildMethods(t, subStr, _nodes, TypeContent.Inherited, false);

                // INHERITED MEMBERS
                // STATIC EVENTS
                DocBuilderHelper.BuildEvents(t, str, _nodes, TypeContent.Inherited, true);
                // INHERITED MEMBERS
                // STATIC FIELDS
                DocBuilderHelper.BuildFields(t, subStr, _nodes, TypeContent.Inherited, true);
                // INHERITED MEMBERS
                // STATIC PROPERTIES
                DocBuilderHelper.BuildProperties(t, subStr, _nodes, TypeContent.Inherited, true);
                // INHERITED MEMBERS
                // STATIC METHODS
                DocBuilderHelper.BuildMethods(t, subStr, _nodes, TypeContent.Inherited, true);

                if (!string.IsNullOrEmpty(subStr.ToString()) && !string.IsNullOrWhiteSpace(subStr.ToString()))
                {
                    str.AppendLine();
                    str.AppendLine();
                    str.AppendLine("## Inherited Members");
                    str.AppendLine(subStr.ToString());
                }

                // EXAMPLES
                BuildExample(t, str);
            }
            else if (t.IsEnum)
            {
                Program.Process($"New Enum: {t.Name}");

                // HEADER
                str.AppendLine($"# {t.Name}");
                if (t.BaseType != null && t.BaseType != typeof(Object))
                {
                    str.AppendLine($"<small>enumeration in `{Path.GetFileNameWithoutExtension(new Uri(t.Assembly.CodeBase).AbsolutePath)}`</small>");
                }
                str.AppendLine(string.Empty);

                // SUMMARY
                DocXmlUtil.CollectSummaryFromNode(_nodes, t.FullName, out var classSummary);
                str.AppendLine("### Description");
                str.AppendLine(classSummary);

                // PROPERTIES
                DocBuilderHelper.BuildEnum(t, str, _nodes);

                // EXAMPLES
                BuildExample(t, str);
            }

            if (File.Exists(AppConfig.Loaded.FileEnd))
            {
                str.AppendLine();
                str.AppendLine(File.ReadAllText(AppConfig.Loaded.FileEnd));
            }

            WriteAllText(filePath, str.ToString());
            return fileName;
        }

        private void BuildExample(Type t, StringBuilder str)
        {
            var exampleFile = FindExampleFile(t);
            if (string.IsNullOrEmpty(exampleFile))
                return;

            str.AppendLine();
            str.AppendLine("## Examples");
            str.AppendLine();
            var exampleStr = File.ReadAllText(exampleFile);
            var allTypes = t.Assembly.GetTypes();
            foreach (var a in allTypes)
            {
                var s = a.Name;
                exampleStr = exampleStr.Replace($"(#{s})", $"({DocSyntax.CollectMarkDownReference(a)})");
            }

            str.AppendLine(exampleStr);
        }

        private string FindExampleFile(Type t)
        {
            if (!Directory.Exists(ExamplesDir))
                return null;

            string fileName = ExamplesDir + EnvironmentUtility.DirectorySeparator + t.Name + ".md";
            if (File.Exists(fileName))
                return fileName;
            else
            {
                if (t.IsClass)
                {
                    string[] allFiles = Directory.GetFiles(ExamplesDir, "*.md");
                    var methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var f in allFiles)
                    {
                        var exampleName = Path.GetFileNameWithoutExtension(f);
                        foreach (var m in methods)
                        {
                            if (m.DeclaringType != t)
                                continue;
                            var returnType = m.ReturnType.Name;
                            if (exampleName == returnType)
                                return f;

                            var genericParameters = m.GetGenericArguments();
                            foreach (var g in genericParameters)
                            {
                                var parameterType = g.Name;
                                if (exampleName == parameterType)
                                    return f;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static void WriteAllText(string filePath, string str)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(filePath, str);
        }
    }
}
