//
// DocGen Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Overmodded.DocGen
{
    internal enum TypeContent
    {
        Target,
        Inherited,
    }

    internal static class DocBuilderHelper
    {
        internal static void BuildEvents(Type t, StringBuilder str, XmlNodeList nodes, TypeContent typeContent, bool applyStatic)
        {
            var events = t.GetEvents();
            var buildProperties = new List<DocProperty>();
            foreach (var e in events)
            {
                if (!CanApplyStatic(IsEventStatic(e), applyStatic))
                    continue;
                if (!IsTypeContentCorrectForTypes(e.DeclaringType, t, typeContent))
                    continue;
                //if (IsTypeArrayContainsNotAllowed(e.PropertyType, e.DeclaringType))
                //    continue;
                DocClassHelper.CollectClassEvent(nodes, e, out var name, out var type, out var description);
                buildProperties.Add(new DocProperty
                {
                    Name = name,
                    Type = type,
                    Description = description
                });
            }
            DocProperty.BuildTabeleFromProperties(str, DocPropertyTabeleStyle.Event, applyStatic, buildProperties.ToArray());
        }


        internal static void BuildFields(Type t, StringBuilder str, XmlNodeList nodes, TypeContent typeContent, bool applyStatic)
        {
            var fields = t.GetFields();
            var buildProperties = new List<DocProperty>();
            foreach (var f in fields)
            {
                if (!CanApplyStatic(f.IsStatic, applyStatic))
                    continue;
                if (!IsTypeContentCorrectForTypes(f.DeclaringType, t, typeContent))
                    continue;
                if (IsTypeArrayContainsNotAllowed(f.FieldType, f.DeclaringType))
                    continue;
                DocClassHelper.CollectClassField(nodes, f, out var name, out var type, out var description);
                buildProperties.Add(new DocProperty
                {
                    Name = name,
                    Type = type,
                    Description = description
                });
            }     
            DocProperty.BuildTabeleFromProperties(str, DocPropertyTabeleStyle.Field, applyStatic, buildProperties.ToArray());
        }

        internal static void BuildProperties(Type t, StringBuilder str, XmlNodeList nodes, TypeContent typeContent, bool applyStatic)
        {
            var properties = t.GetProperties();
            var buildProperties = new List<DocProperty>(); 
            foreach (var p in properties)
            {
                if (!CanApplyStatic(IsPropertyStatic(p), applyStatic))
                    continue;
                if (!IsTypeContentCorrectForTypes(p.DeclaringType, t, typeContent))
                    continue;
                if (IsTypeArrayContainsNotAllowed(p.PropertyType, p.DeclaringType))
                    continue;
                DocClassHelper.CollectClassProperty(nodes, p, out var name, out var type, out var protection, out var description);
                buildProperties.Add(new DocProperty
                {
                    Name = name,
                    Type = type,
                    Protection = protection,
                    Description = description
                });
            }  
            DocProperty.BuildTabeleFromProperties(str, DocPropertyTabeleStyle.Property, applyStatic, buildProperties.ToArray());
        }

        internal static void BuildMethods(Type t, StringBuilder str, XmlNodeList nodes, TypeContent typeContent, bool applyStatic)
        {
            var methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var buildProperties = new List<DocProperty>();
            foreach (var m in methods)
            {
                if (!CanApplyStatic(m.IsStatic, applyStatic))
                    continue;
                if (!m.IsPublic || m.IsAbstract || m.IsVirtual)
                    continue;
                if (!IsDeclaringTypeAllowed(m.DeclaringType))
                    continue;
                if (!IsTypeContentCorrectForTypes(m.DeclaringType, t, typeContent))
                    continue;
                if (IsTypeArrayContainsNotAllowed(new List<Type>(m.GetGenericArguments()) {m.DeclaringType}.ToArray()))
                    continue;
                if (IsMethodPropertySetOrGet(t, m))
                    continue;

                DocClassHelper.CollectClassMethod(nodes, m, out var name, out var type, out var description);
                buildProperties.Add(new DocProperty
                {
                    Name = name,
                    Type = type,
                    Description = description
                });
            }

            DocProperty.BuildTabeleFromProperties(str, DocPropertyTabeleStyle.Method, applyStatic, buildProperties.ToArray());
        }

        internal static void BuildEnum(Type t, StringBuilder str, XmlNodeList nodes)
        {
            var names = Enum.GetNames(t);
            var buildProperties = new List<DocProperty>();
            foreach (var n in names)
            {
                DocClassHelper.CollectEnumDescription(nodes, t, n, out var description);
                buildProperties.Add(new DocProperty
                {
                    Name = n,
                    Description = description
                });
            }
            DocProperty.BuildTabeleFromProperties(str, DocPropertyTabeleStyle.Enum, false, buildProperties.ToArray());
        }

        private static bool CanApplyStatic(bool isStatic, bool shouldApply)
        {
            if (isStatic)
            {
                if (!shouldApply)
                    return false;
            }
            else
            {
                if (shouldApply)
                    return false;
            }

            return true;
        }

        private static bool IsPropertyStatic(PropertyInfo p)
        {
            var setter = p.GetSetMethod();
            var getter = p.GetGetMethod();
            if (setter == null && getter == null)
                return false;// lul
            return (setter?.IsStatic ?? false) || (getter?.IsStatic ?? false);
        }

        private static bool IsEventStatic(EventInfo p)
        {
            var setter = p.GetAddMethod();
            var getter = p.GetRemoveMethod();
            return (setter?.IsStatic ?? false) || (getter?.IsStatic ?? false);
        }

        private static bool IsMethodPropertySetOrGet(Type t, MethodInfo m)
        {
            // check properties
            var properties = t.GetProperties();
            if (properties.Any(p => m == p.GetSetMethod() || m == p.GetGetMethod()))
                return true;

            // properties OK
            // check events
            var events = t.GetEvents();
            return events.Any(e => m == e.GetAddMethod() || m == e.GetRemoveMethod());
        }

        internal static bool IsTypeArrayContainsNotAllowed(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            foreach (var t in types)
            {
                var fullName = t?.FullName;
                if (fullName == null)
                    continue;

                if (t.DeclaringType != null && t.DeclaringType.Assembly != t.Assembly)
                {
                    return true;
                }

                if (!IsTypeNameAllowed(t))
                {
                    return true;
                }

                if (AppConfig.Loaded.DisallowedNamespaces.Any(n => fullName.StartsWith(n)))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsTypeContentCorrectForTypes(Type a, Type b, TypeContent typeContent)
        {
            switch (typeContent)
            {
                case TypeContent.Target:
                    if (a == b)
                        return true;
                    break;
                case TypeContent.Inherited:
                    if (a != b)
                        return true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeContent), typeContent, null);
            }

            return false;
        }

        private static bool IsDeclaringTypeAllowed(Type t)
        {
            var fullName = t.FullName ?? throw new NullReferenceException("oops");
            if (AppConfig.Loaded.DisallowedDeclarationTypes.Any(n => fullName.StartsWith(n)))
            {
                return false;
            }

            return true;
        }

        private static bool IsTypeNameAllowed(Type t)
        {
            var fullName = t.FullName ?? throw new NullReferenceException("oops");
            if (fullName.Contains("+")) // ignore type in another types
                return false;

            if (AppConfig.Loaded.DisallowedTypes.Any(n => fullName.StartsWith(n)))
            {
                return false;
            }

            if (AppConfig.Loaded.DisallowedTypes.Any(n => fullName.EndsWith(n)))
            {
                return false;
            }

            return true;
        }
    }
}
