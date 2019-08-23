//
// DocGen Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging;
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
                name = "(";
                for (var index = 0; index < parameters.Length; index++)
                {
                    var parameter = parameters[index];
                    name = name + GetTypeMarkdown(parameter.ParameterType, fixedName: parameter.Name);
                    //if (index + 1 < parameters.Length)
                    //    name += " ` ` ";
                }

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
            try
            {
                if (parameters.Length != 0)
                {
                    fullName += "(";
                    for (var index = 0; index < parameters.Length; index++)
                    {
                        var parameter = parameters[index];
                        fullName += parameter.ParameterType.FullName.Replace('&', '@');
                        if (index + 1 < parameters.Length)
                            fullName += ",";
                    }

                    fullName += ")";
                }
            }
            catch (Exception)
            {
                fullName = "GEN_ERR";
            }

            return fullName;
        }

        /// <summary>
        ///     Gets markdown string of given type.
        ///     It includes type reference.
        /// </summary>
        internal static string GetTypeMarkdown(Type t, bool quote = true, string fixedName = null)
        {
            var name = string.Empty;
            var typeName = DocSyntax.FixVarName(t.Name); // fix type name
            if (t.IsByRef)
            {
                typeName = typeName.Remove(typeName.Length - 1, 1);
                typeName = $"ref {typeName}";
            }

            if (!string.IsNullOrEmpty(fixedName))
            {
                typeName += $" {fixedName}";
            }

            if (!CanDefineTypeReference(t))
            {
                if (quote)
                    name += "`";
                name += typeName;
                if (quote)
                    name += "`";
                name += " ";
            }
            else
            {
                name += "[";
                if (quote)
                    name += "`";
                name += typeName;
                if (quote)
                    name += "`";
                name += $"]({DocSyntax.CollectMarkDownReference(t)})";
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
