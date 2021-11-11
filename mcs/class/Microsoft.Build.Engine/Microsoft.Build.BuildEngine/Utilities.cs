//
// Utilities.cs:
//
// Author:
//   Marek Sieradzki (marek.sieradzki@gmail.com)
//   Lluis Sanchez Gual <lluis@novell.com>
//   Michael Hutchinson <mhutchinson@novell.com>
//
// (C) 2005 Marek Sieradzki
// Copyright (c) 2008 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Mono.XBuild.Utilities;
 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Shared;

namespace Microsoft.Build.BuildEngine
{
	/// <summary>Contains utility methods used by MSBuild. This class cannot be inherited.</summary>
	// Token: 0x0200008A RID: 138
	public static class Utilities
	{
        internal static string FromMSBuildPath (string relPath)
		{
			return MSBuildUtils.FromMSBuildPath (relPath);
		}
		// Token: 0x06000587 RID: 1415 RVA: 0x0001ED50 File Offset: 0x0001DD50
		internal static void UpdateConditionedPropertiesTable(Hashtable conditionedPropertiesTable, string leftValue, string rightValueExpanded)
		{
			if (conditionedPropertiesTable != null && rightValueExpanded.Length > 0)
			{
				string[] array = leftValue.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					Match match = Utilities.singlePropertyRegex.Match(array[i]);
					if (match.Success)
					{
						int num = rightValueExpanded.IndexOf('|');
						string value;
						if (num == -1 || i == array.Length - 1)
						{
							value = rightValueExpanded;
							i = array.Length;
						}
						else
						{
							value = rightValueExpanded.Substring(0, num);
							rightValueExpanded = rightValueExpanded.Substring(num + 1);
						}
						string key = match.Groups[1].ToString();
						StringCollection stringCollection = (StringCollection)conditionedPropertiesTable[key];
						if (stringCollection == null)
						{
							stringCollection = new StringCollection();
							conditionedPropertiesTable[key] = stringCollection;
						}
						if (!stringCollection.Contains(value))
						{
							stringCollection.Add(value);
						}
					}
				}
			}
		}
/*
		// Token: 0x06000588 RID: 1416 RVA: 0x0001EE2F File Offset: 0x0001DE2F
		internal static void GatherReferencedPropertyNames(string condition, XmlAttribute conditionAttribute, Expander expander, Hashtable conditionedPropertiesTable)
		{
			Utilities.EvaluateCondition(condition, conditionAttribute, expander, conditionedPropertiesTable, ParserOptions.AllowPropertiesAndItemLists, null, null);
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x0001EE3E File Offset: 0x0001DE3E
		internal static bool EvaluateCondition(string condition, XmlAttribute conditionAttribute, Expander expander, ParserOptions itemListOptions, Project parentProject)
		{
			return Utilities.EvaluateCondition(condition, conditionAttribute, expander, parentProject.ConditionedProperties, itemListOptions, parentProject.ParentEngine.LoggingServices, parentProject.ProjectBuildEventContext);
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x0001EE63 File Offset: 0x0001DE63
		internal static bool EvaluateCondition(string condition, XmlAttribute conditionAttribute, Expander expander, ParserOptions itemListOptions, EngineLoggingServices loggingServices, BuildEventContext buildEventContext)
		{
			return Utilities.EvaluateCondition(condition, conditionAttribute, expander, null, itemListOptions, loggingServices, buildEventContext);
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x0001EE74 File Offset: 0x0001DE74
		internal static bool EvaluateCondition(string condition, XmlAttribute conditionAttribute, Expander expander, Hashtable conditionedPropertiesTable, ParserOptions itemListOptions, EngineLoggingServices loggingServices, BuildEventContext buildEventContext)
		{
			ErrorUtilities.VerifyThrow(conditionAttribute != null || condition.Length == 0, "If condition is non-empty, you must provide the XML node representing the condition.");
			if (condition == null || condition.Length == 0)
			{
				return true;
			}
			Hashtable hashtable = Utilities.cachedExpressionTrees[(int)itemListOptions];
			GenericExpressionNode genericExpressionNode = (GenericExpressionNode)hashtable[condition];
			if (genericExpressionNode == null)
			{
				genericExpressionNode = new Parser
				{
					LoggingServices = loggingServices,
					LogBuildEventContext = buildEventContext
				}.Parse(condition, conditionAttribute, itemListOptions);
				lock (hashtable)
				{
					hashtable[condition] = genericExpressionNode;
				}
			}
			ConditionEvaluationState state = new ConditionEvaluationState(conditionAttribute, expander, conditionedPropertiesTable, condition);
			bool result;
			lock (genericExpressionNode)
			{
				result = genericExpressionNode.Evaluate(state);
				genericExpressionNode.ResetState();
			}
			return result;
		}
*/
		// Token: 0x0600058C RID: 1420 RVA: 0x0001EF4C File Offset: 0x0001DF4C
		internal static void SetXmlNodeInnerContents(XmlNode node, string s)
		{
			ErrorUtilities.VerifyThrow(s != null, "Need value to set.");
			if (s.IndexOf('<') != -1)
			{
				try
				{
					node.InnerXml = s;
					return;
				}
				catch (XmlException)
				{
				}
			}
			node.InnerText = s;
		}

		// Token: 0x0600058D RID: 1421 RVA: 0x0001EF98 File Offset: 0x0001DF98
		internal static string GetXmlNodeInnerContents(XmlNode node)
		{
			if (!node.HasChildNodes)
			{
				return string.Empty;
			}
			if (node.ChildNodes.Count == 1 && (node.FirstChild.NodeType == XmlNodeType.Text || node.FirstChild.NodeType == XmlNodeType.CDATA))
			{
				return node.InnerText;
			}
			string innerXml = node.InnerXml;
			int num = innerXml.IndexOf('<');
			if (num == -1)
			{
				return node.InnerText;
			}
			bool flag = Utilities.ContainsNoTagsOtherThanComments(innerXml, num);
			if (flag)
			{
				return node.InnerText;
			}
			bool flag2 = innerXml.IndexOf("<![CDATA[", StringComparison.Ordinal) == 0;
			if (flag2)
			{
				return node.InnerText;
			}
			return innerXml;
		}

		// Token: 0x0600058E RID: 1422 RVA: 0x0001F02C File Offset: 0x0001E02C
		private static bool ContainsNoTagsOtherThanComments(string innerXml, int firstLessThan)
		{
			bool flag = false;
			for (int i = firstLessThan; i < innerXml.Length; i++)
			{
				if (!flag && i < innerXml.Length - 3 && innerXml[i] == '<' && innerXml[i + 1] == '!' && innerXml[i + 2] == '-' && innerXml[i + 3] == '-')
				{
					flag = true;
					i += 3;
				}
				else
				{
					if (!flag && innerXml[i] == '<')
					{
						return false;
					}
					if (flag && i < innerXml.Length - 2 && innerXml[i] == '-' && innerXml[i + 1] == '-' && innerXml[i + 2] == '>')
					{
						flag = false;
						i += 2;
					}
				}
			}
			return true;
		}

		// Token: 0x0600058F RID: 1423 RVA: 0x0001F0E4 File Offset: 0x0001E0E4
		internal static string RemoveXmlNamespace(string xml)
		{
			return Utilities.xmlnsPattern.Replace(xml, string.Empty);
		}

		/// <summary>Converts the specified string into a syntax that allows the MSBuild engine to interpret the character literally.</summary>
		/// <returns>The converted value of the specified string.</returns>
		/// <param name="unescapedExpression">The string to convert.</param>
		// Token: 0x06000590 RID: 1424 RVA: 0x0001F0F6 File Offset: 0x0001E0F6
		public static string Escape(string unescapedExpression)
		{
			return EscapingUtilities.Escape(unescapedExpression);
		}
/*
		// Token: 0x06000591 RID: 1425 RVA: 0x0001F100 File Offset: 0x0001E100
		internal static BuildEventFileInfo CreateBuildEventFileInfo(XmlNode xmlNode, string defaultFile)
		{
			ErrorUtilities.VerifyThrow(xmlNode != null, "Need Xml node.");
			int line = 0;
			int column = 0;
			string text = XmlUtilities.GetXmlNodeFile(xmlNode, string.Empty);
			if (text.Length == 0)
			{
				text = defaultFile;
			}
			else
			{
				XmlSearcher.GetLineColumnByNode(xmlNode, out line, out column);
			}
			return new BuildEventFileInfo(text, line, column);
		}*/

		// Token: 0x06000592 RID: 1426 RVA: 0x0001F14D File Offset: 0x0001E14D
		internal static Hashtable CreateTableIfNecessary(Hashtable table)
		{
			if (table == null)
			{
				return new Hashtable(StringComparer.OrdinalIgnoreCase);
			}
			return table;
		}

		// Token: 0x06000593 RID: 1427 RVA: 0x0001F15E File Offset: 0x0001E15E
		internal static Dictionary<string, V> CreateTableIfNecessary<V>(Dictionary<string, V> table)
		{
			if (table == null)
			{
				return new Dictionary<string, V>(StringComparer.OrdinalIgnoreCase);
			}
			return table;
		}

		// Token: 0x040002D2 RID: 722
		private static readonly Regex singlePropertyRegex = new Regex("^\\$\\(([^\\$\\(\\)]*)\\)$");

		// Token: 0x040002D3 RID: 723
		private static volatile Hashtable[] cachedExpressionTrees = new Hashtable[]
		{
			new Hashtable(StringComparer.OrdinalIgnoreCase),
			new Hashtable(StringComparer.OrdinalIgnoreCase),
			new Hashtable(StringComparer.OrdinalIgnoreCase),
			new Hashtable(StringComparer.OrdinalIgnoreCase),
			new Hashtable(StringComparer.OrdinalIgnoreCase),
			new Hashtable(StringComparer.OrdinalIgnoreCase),
			new Hashtable(StringComparer.OrdinalIgnoreCase),
			new Hashtable(StringComparer.OrdinalIgnoreCase)
		};

		// Token: 0x040002D4 RID: 724
		private static readonly Regex xmlnsPattern = new Regex("xmlns=\"[^\"]*\"\\s*");
	}
}

