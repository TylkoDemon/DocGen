//
// DocGen Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Xml;

namespace DocGen
{
    internal static class DocXmlUtil
    {
        internal const string XmlName = "name";
        internal const string XmlSummary = "summary";
        internal const string XmlNotAvailable = @"N\A";

        internal static bool CollectSummaryFromNode(XmlNodeList nodes, string targetName, out string summary)
        {
            foreach (XmlNode node in nodes)
            {
                var nodeName = node.Attributes?[XmlName]?.Value ?? throw new NullReferenceException();
                var typeName = nodeName.Remove(0, 2);
                if (typeName != targetName) continue;
                summary = node.SelectSingleNode(XmlSummary)?.InnerText;
                summary = DocSyntax.RemoveSpaces(summary);
                return true;
            }

            summary = XmlNotAvailable;
            return false;
        }

    }
}
