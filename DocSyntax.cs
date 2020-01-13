//
// DocGen Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DocGen
{
    internal static class DocSyntax
    {
        /// <summary>
        ///     Gets markdown file by adding .md extension. exdi?
        /// </summary>
        internal static string GetMarkdownFile(string fileName) => $"{fileName}.md";

        /// <summary>
        ///     Returns reference to a markdown file for given Type.
        /// </summary>
        internal static string CollectMarkDownReference(Type t)
        {
            var baseTypeName = CollectTypeName(t);
            return GetMarkdownFile($"{AppConfig.Loaded.DeploySidebarPath}{baseTypeName}");
        }

        /// <summary>
        ///     Returns a name in markdown environment of given Type.
        /// </summary>
        internal static string CollectTypeName(Type t) => "obj" + t.Name;

        /// <summary>
        ///     Removes all spaces and new lines from string.
        /// </summary>
        internal static string RemoveSpaces(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var line = Regex.Replace(str, @"\t|\n|\r", "");
            while (line.IndexOf("  ", StringComparison.Ordinal) >= 0)
            {
                line = line.Replace("  ", " ");
            }

            return line;
        }

        /// <summary>
        ///     Fixes string by replacing all not allowed characters.
        /// </summary>
        internal static string FixVarName(string str)
        {
            FixVarName(ref str);
            return str;
        }

        /// <summary>
        ///     Fixes string by replacing all not allowed characters.
        /// </summary>
        internal static bool FixVarName(ref string str)
        {
            char[] banned =
            {
                ' ', '(', ')', '[', ']', '"', '"',
                "'"[0], '<', '>', ',', '?', '/',
                '!', '-', '+', '=', '`'
            };

            var p = str;
            str = banned.Aggregate(str, (current, c) => current.Replace(c, '_'));
            return p != str;
        }
    }
}
