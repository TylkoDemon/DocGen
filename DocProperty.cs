//
// DocGen Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Text;

namespace Overmodded.DocGen
{
    internal enum DocPropertyTabeleStyle
    {
        Event,
        Property,
        Field,
        Method,
        Enum
    }

    internal class DocProperty
    {
        internal string Name;
        internal string Type;
        internal string Protection;
        internal string Description;

        internal static void BuildTabeleFromProperties(StringBuilder str, DocPropertyTabeleStyle style, bool isStatic, DocProperty[] buildProperties)
        {
            if (buildProperties.Length == 0) return;

            str.AppendLine();
            str.AppendLine(); 
            string tabeleName = isStatic ? "Static " : string.Empty;
            switch (style)
            {
                case DocPropertyTabeleStyle.Event:
                    tabeleName += "Events";
                    break;
                case DocPropertyTabeleStyle.Property:
                    tabeleName += "Properties";
                    break;
                case DocPropertyTabeleStyle.Field:
                    tabeleName += "Fields";
                    break;
                case DocPropertyTabeleStyle.Method:
                    tabeleName += "Methods";
                    break;
                case DocPropertyTabeleStyle.Enum:
                    tabeleName += "Properties";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
            str.AppendLine($"### {tabeleName}");
            str.AppendLine();

            switch (style)
            {
                case DocPropertyTabeleStyle.Event:
                    str.AppendLine("| Name          | Type          | Description");
                    str.AppendLine("| ---           | ---           | ---");
                    foreach (var p in buildProperties)
                        str.AppendLine($"| {p.Name}         | {p.Type}          | {p.Description}");
                    break;
                case DocPropertyTabeleStyle.Property:
                    str.AppendLine("| Name          | Type          | Protection            | Description");
                    str.AppendLine("| ---           | ---           | ---                   | ---");
                    foreach (var p in buildProperties)
                        str.AppendLine($"| {p.Name}         | {p.Type}          | {p.Protection}            | {p.Description}");
                    break;
                case DocPropertyTabeleStyle.Field:
                    str.AppendLine("| Name          | Type          | Description");
                    str.AppendLine("| ---           | ---           | ---");
                    foreach (var p in buildProperties)
                        str.AppendLine($"| {p.Name}         | {p.Type}          | {p.Description}");
                    break;
                case DocPropertyTabeleStyle.Method:
                    str.AppendLine("| Name          | Return             | Description");
                    str.AppendLine("| ---           | ---           | ---");
                    foreach (var p in buildProperties)
                        str.AppendLine($"| {p.Name}         | {p.Type}          | {p.Description}");
                    break;
                case DocPropertyTabeleStyle.Enum:
                    str.AppendLine("| Name          | Description");
                    str.AppendLine("| ---           | ---");
                    foreach (var p in buildProperties)
                        str.AppendLine($"| {p.Name}         | {p.Description}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }
    }
}
