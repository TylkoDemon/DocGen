//
// DocGen Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Reflection;
using System.Xml;

namespace Overmodded.DocGen
{
    internal static class DocClassHelper
    {
        internal static void CollectClassEvent(XmlNodeList nodes, EventInfo @event, out string name, out string type, out string description)
        {
            name = $"`{@event.Name}`";
            type = DocClassUtil.GetTypeMarkdown(@event.EventHandlerType);
            description = FullSummaryCollection(@event.DeclaringType, @event.Name, null, nodes);
        }

        internal static void CollectClassField(XmlNodeList nodes, FieldInfo field, out string name, out string type,
            out string description)
        {
            name = $"`{field.Name}`";
            type = DocClassUtil.GetTypeMarkdown(field.FieldType);
            description = FullSummaryCollection(field.DeclaringType, field.Name, null, nodes);
        }

        internal static void CollectClassProperty(XmlNodeList nodes, PropertyInfo property, out string name,
            out string type, out string protection, out string description)
        {
            name = $"`{property.Name}`";
            type = DocClassUtil.GetTypeMarkdown(property.PropertyType);
            protection = FullProtectionCollection(property);
            description = FullSummaryCollection(property.DeclaringType, property.Name, null, nodes);
        }

        internal static void CollectClassMethod(XmlNodeList nodes, MethodInfo info, out string name, out string returnType,
            out string description)
        {
            var parameters = info.GetParameters();
            returnType = DocClassUtil.GetTypeMarkdown(info.ReturnType);
            name = $"`{info.Name}`{DocClassUtil.GetParametersMarkdownString(parameters)}";
            description = FullSummaryCollection(info.DeclaringType, info.Name, parameters, nodes);
        }

        internal static void CollectEnumDescription(XmlNodeList nodes, Type t, string name, out string description)
        {
            description = FullSummaryCollection(t, name, null, nodes);
        }

        private static string FullProtectionCollection(PropertyInfo property)
        {
            var protection = string.Empty;
            if (property.GetGetMethod() != null)
                protection += "`get;`";
            if (property.GetSetMethod() != null)
            {
                if (!string.IsNullOrEmpty(protection))
                    protection += " ";
                protection += "`set;`";
            }

            if (string.IsNullOrEmpty(protection)) protection = @"N\A";
            return protection;
        }

        private static string FullSummaryCollection(Type t, string n, ParameterInfo[] p, XmlNodeList nodes)
        {
            var fullXmlName = (t?.FullName ?? throw new NullReferenceException("oops")) + "." + n;
            if (p != null) fullXmlName = DocClassUtil.GetParametersXmlString(p, fullXmlName);

            DocXmlUtil.CollectSummaryFromNode(nodes, fullXmlName, out var description);
            return description;
        }
    }
}
