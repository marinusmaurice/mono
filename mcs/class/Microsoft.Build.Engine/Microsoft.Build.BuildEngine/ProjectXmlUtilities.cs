using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Build.Shared;

namespace Microsoft.Build.BuildEngine
{
	// Token: 0x020000F5 RID: 245
	internal class ProjectXmlUtilities
	{
		// Token: 0x06000A3B RID: 2619 RVA: 0x00036CA0 File Offset: 0x00035CA0
		internal static List<XmlElement> GetValidChildElements(XmlElement element)
		{
			List<XmlElement> list = new List<XmlElement>();
			foreach (object obj in element)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlNodeType nodeType = xmlNode.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType != XmlNodeType.Comment && nodeType != XmlNodeType.Whitespace)
					{
						ProjectXmlUtilities.ThrowProjectInvalidChildElement(xmlNode);
					}
				}
				else
				{
					XmlElement xmlElement = (XmlElement)xmlNode;
					ProjectXmlUtilities.VerifyThrowProjectValidNamespace(xmlElement);
					list.Add(xmlElement);
				}
			}
			return list;
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x00036D30 File Offset: 0x00035D30
		internal static void VerifyThrowProjectXmlElementChild(XmlNode childNode)
		{
			if (childNode.NodeType != XmlNodeType.Element)
			{
				ProjectXmlUtilities.ThrowProjectInvalidChildElement(childNode);
			}
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x00036D44 File Offset: 0x00035D44
		internal static void VerifyThrowProjectNoChildElements(XmlElement element)
		{
			List<XmlElement> validChildElements = ProjectXmlUtilities.GetValidChildElements(element);
			if (validChildElements.Count > 0)
			{
				ProjectXmlUtilities.ThrowProjectInvalidChildElement(element.FirstChild);
			}
		}

		// Token: 0x06000A3E RID: 2622 RVA: 0x00036D6C File Offset: 0x00035D6C
		internal static void ThrowProjectInvalidChildElement(XmlNode child)
		{
			ProjectErrorUtilities.ThrowInvalidProject(child, "UnrecognizedChildElement", child.Name, child.ParentNode.Name);
		}

		// Token: 0x06000A3F RID: 2623 RVA: 0x00036D8A File Offset: 0x00035D8A
		internal static void VerifyThrowElementName(XmlElement element, string expected)
		{
			ErrorUtilities.VerifyThrowNoAssert(string.Equals(element.Name, expected, StringComparison.Ordinal), "Expected " + expected + " element, got " + element.Name);
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x00036DB4 File Offset: 0x00035DB4
		internal static void VerifyThrowProjectValidNameAndNamespace(XmlElement element)
		{
			XmlUtilities.VerifyThrowProjectValidElementName(element);
			ProjectXmlUtilities.VerifyThrowProjectValidNamespace(element);
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x00036DC2 File Offset: 0x00035DC2
		internal static void VerifyThrowProjectValidNamespace(XmlElement element)
		{
			if (element.Prefix.Length > 0 || !string.Equals(element.NamespaceURI, "http://schemas.microsoft.com/developer/msbuild/2003", StringComparison.OrdinalIgnoreCase))
			{
				ProjectErrorUtilities.ThrowInvalidProject(element, "CustomNamespaceNotAllowedOnThisChildElement", element.Name, element.ParentNode.Name);
			}
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x00036E04 File Offset: 0x00035E04
		internal static void VerifyThrowProjectNoAttributes(XmlElement element)
		{
			foreach (object obj in element.Attributes)
			{
				XmlAttribute attribute = (XmlAttribute)obj;
				ProjectXmlUtilities.ThrowProjectInvalidAttribute(attribute);
			}
		}

		// Token: 0x06000A43 RID: 2627 RVA: 0x00036E5C File Offset: 0x00035E5C
		internal static void VerifyThrowProjectInvalidAttribute(bool condition, XmlAttribute attribute)
		{
			if (!condition)
			{
				ProjectXmlUtilities.ThrowProjectInvalidAttribute(attribute);
			}
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x00036E67 File Offset: 0x00035E67
		internal static void ThrowProjectInvalidAttribute(XmlAttribute attribute)
		{
			ProjectErrorUtilities.ThrowInvalidProject(attribute, "UnrecognizedAttribute", attribute.Name, attribute.OwnerElement.Name);
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x00036E88 File Offset: 0x00035E88
		internal static XmlAttribute GetConditionAttribute(XmlElement element, bool verifySoleAttribute)
		{
			XmlAttribute result = null;
			foreach (object obj in element.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				string name;
				if ((name = xmlAttribute.Name) != null && name == "Condition")
				{
					result = xmlAttribute;
				}
				else
				{
					ProjectErrorUtilities.VerifyThrowInvalidProject(!verifySoleAttribute, xmlAttribute, "UnrecognizedAttribute", xmlAttribute.Name, element.Name);
				}
			}
			return result;
		}

		// Token: 0x06000A46 RID: 2630 RVA: 0x00036F18 File Offset: 0x00035F18
		internal static XmlAttribute SetOrRemoveAttribute(XmlElement element, string name, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				element.RemoveAttribute(name);
				return null;
			}
			element.SetAttribute(name, value);
			return element.Attributes[name];
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x00036F4C File Offset: 0x00035F4C
		internal static string GetAttributeValue(XmlAttribute attribute)
		{
			if (attribute != null)
			{
				return attribute.Value;
			}
			return string.Empty;
		}
	}
}
