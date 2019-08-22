//
// DocGen Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Linq;
using System.Reflection;

namespace Overmodded.DocGen
{
    internal static class DocClassUtil
    {
        /// <summary>
        ///     Gets a markdown string from array of parameters with objects references.
        /// </summary>
        internal static string GetParametersMarkdownString(ParameterInfo[] parameters)
        {
            string name;
            if (parameters.Length != 0)
            {
                name = parameters.Aggregate("(", (current, parameter) => current + GetTypeMarkdown(parameter.ParameterType));
                name += ")";
            }
            else name = "()";
            return name;
        }

        /// <summary>
        ///     Gets a string from array of parameters that allows to get xml documentation. EX. (System.String,System.Boolean)
        /// </summary>
        internal static string GetParametersXmlString(ParameterInfo[] parameters, string fullName)
        {
            if (parameters.Length != 0)
            {
                fullName += "(";
                fullName = parameters.Aggregate(fullName, (current, p) => current + p.ParameterType.FullName);
                fullName += ")";
            }

            return fullName;
        }

        /// <summary>
        ///     Gets markdown string of given type.
        ///     It includes type reference.
        /// </summary>
        internal static string GetTypeMarkdown(Type t)
        {
            var name = string.Empty;
            var typeName = DocSyntax.FixVarName(t.Name); // fix type name
            if (!CanDefineTypeReference(t))
            {
                name += "`";
                name += typeName;
                name += "` ";
            }
            else
            {
                name += "[`";
                name += typeName;
                name += $"`]({DocSyntax.CollectMarkDownReference(t)})";
            }

            return name;
        }

        /// <summary>
        ///     Checks if generator can create reference link to a given Type.
        /// </summary>
        internal static bool CanDefineTypeReference(Type t)
        {
            if (IsBasicType(t))
                return false; // we can't create references for basic System types. no need to check if type is in loaded assemblies

            // check if type is in any of loaded assemblies (Target)
            bool any = Generator.LoadedAssemblies.Any(a => a == t.Assembly);
            if (!any)
                return false; // the type is not a member of loaded assemblies, can't create reference

            // As IEnumerator has it's weird naming, we need to check for ` character.
            if (t.Name.Contains("`"))
                return false;

            // Type OK. Reference can be created.
            return true;
        }

        /// <summary>
        ///     Checks if given Type is 'Basic' (is a long, int, bool, etc.)
        /// </summary>
        private static bool IsBasicType(Type t)
        {
            if (t == typeof(long))
                return true;
            if (t == typeof(int))
                return true;
            if (t == typeof(short))
                return true;
            if (t == typeof(ulong))
                return true;
            if (t == typeof(uint))
                return true;
            if (t == typeof(ushort))
                return true;
            if (t == typeof(string))
                return true;
            if (t == typeof(bool))
                return true;
            if (t == typeof(float))
                return true;
            if (t == typeof(double))
                return true;
            if (t == typeof(object[]))
                return true;
            if (t == typeof(void))
                return true;
            if (t == typeof(byte))
                return true;
            if (t == typeof(Action) || t.Name.Contains("Action"))
                return true;
            if (t == typeof(Type))
                return true;
            if (t.Name == "T")
                return true;
            if (t.Name.ToLower() == "object")
                return true;

            return false;
        }
    }
}
