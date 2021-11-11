using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Build.Shared;
using Microsoft.Win32;

namespace Microsoft.Build.BuildEngine
{
    [Flags]
	internal enum ExpanderOptions
	{
		// Token: 0x0400013C RID: 316
		Invalid = 0,
		// Token: 0x0400013D RID: 317
		ExpandProperties = 1,
		// Token: 0x0400013E RID: 318
		ExpandItems = 2,
		// Token: 0x0400013F RID: 319
		ExpandPropertiesAndItems = 3,
		// Token: 0x04000140 RID: 320
		ExpandMetadata = 4,
		// Token: 0x04000141 RID: 321
		ExpandPropertiesAndMetadata = 5,
		// Token: 0x04000142 RID: 322
		ExpandAll = 7
	}
	// Token: 0x02000043 RID: 67
	internal class Expander
	{
		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060002AC RID: 684 RVA: 0x0000DCAA File Offset: 0x0000CCAA
		internal Dictionary<string, string> ItemMetadata
		{
			get
			{
				return this.itemMetadata;
			}
		}

		// Token: 0x060002AD RID: 685 RVA: 0x0000DCB2 File Offset: 0x0000CCB2
		internal Expander(BuildPropertyGroup properties)
		{
			this.options = ExpanderOptions.ExpandProperties;
			this.properties = properties;
		}

		// Token: 0x060002AE RID: 686 RVA: 0x0000DCC8 File Offset: 0x0000CCC8
		internal Expander(BuildPropertyGroup properties, string implicitMetadataItemType, Dictionary<string, string> unqualifiedItemMetadata)
		{
			this.options = ExpanderOptions.ExpandPropertiesAndMetadata;
			this.properties = properties;
			this.itemMetadata = unqualifiedItemMetadata;
			this.implicitMetadataItemType = implicitMetadataItemType;
		}

		// Token: 0x060002AF RID: 687 RVA: 0x0000DCEC File Offset: 0x0000CCEC
		internal Expander(BuildPropertyGroup properties, Hashtable items) : this(new ReadOnlyLookup(items, properties), null, ExpanderOptions.ExpandPropertiesAndItems)
		{
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x0000DCFD File Offset: 0x0000CCFD
		internal Expander(BuildPropertyGroup properties, Hashtable items, ExpanderOptions options) : this(new ReadOnlyLookup(items, properties), null, options)
		{
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x0000DD0E File Offset: 0x0000CD0E
		internal Expander(ReadOnlyLookup lookup, Dictionary<string, string> itemMetadata) : this(lookup, itemMetadata, ExpanderOptions.ExpandAll)
		{
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x0000DD19 File Offset: 0x0000CD19
		internal Expander(ReadOnlyLookup lookup) : this(lookup, null, ExpanderOptions.ExpandPropertiesAndItems)
		{
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x0000DD24 File Offset: 0x0000CD24
		internal Expander(ReadOnlyLookup lookup, Dictionary<string, string> itemMetadata, ExpanderOptions options)
		{
			ErrorUtilities.VerifyThrow(options != ExpanderOptions.Invalid, "Must specify options");
			this.lookup = lookup;
			this.itemMetadata = itemMetadata;
			this.options = options;
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x0000DD52 File Offset: 0x0000CD52
		internal Expander(Expander expander, ExpanderOptions options) : this(expander.lookup, expander.itemMetadata, options)
		{
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x0000DD67 File Offset: 0x0000CD67
		internal Expander(Expander expander, SpecificItemDefinitionLibrary itemDefinitionLibrary) : this(expander.lookup, null, expander.options)
		{
			if (this.implicitMetadataItemType == null)
			{
				this.itemMetadata = expander.itemMetadata;
			}
			this.specificItemDefinitionLibrary = itemDefinitionLibrary;
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x0000DD98 File Offset: 0x0000CD98
		internal void SetMetadataInMetadataTable(string itemType, string name, string value)
		{
			ErrorUtilities.VerifyThrow((this.options & ExpanderOptions.ExpandMetadata) != ExpanderOptions.Invalid, "Must be expanding metadata");
			ErrorUtilities.VerifyThrow(this.implicitMetadataItemType == null || string.Equals(this.implicitMetadataItemType, itemType, StringComparison.OrdinalIgnoreCase), "Unexpected metadata type");
			if (this.itemMetadata == null)
			{
				this.itemMetadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				this.implicitMetadataItemType = itemType;
			}
			if (string.Equals(this.implicitMetadataItemType, itemType, StringComparison.OrdinalIgnoreCase))
			{
				this.itemMetadata[name] = value;
				return;
			}
			this.itemMetadata[itemType + "." + name] = value;
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x0000DE34 File Offset: 0x0000CE34
		internal List<BuildItem> ExpandAllIntoBuildItems(string expression, XmlAttribute expressionAttribute)
		{
			List<BuildItem> list = new List<BuildItem>();
			string text = this.ExpandPropertiesLeaveEscaped(this.ExpandMetadataLeaveEscaped(expression));
			if (text.Length > 0)
			{
				List<string> list2 = ExpressionShredder.SplitSemiColonSeparatedList(text);
				foreach (string text2 in list2)
				{
					BuildItemGroup buildItemGroup = this.ExpandSingleItemListExpressionIntoItemsLeaveEscaped(text2, expressionAttribute);
					if (buildItemGroup != null)
					{
						using (IEnumerator enumerator2 = buildItemGroup.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								object obj = enumerator2.Current;
								BuildItem item = (BuildItem)obj;
								list.Add(item);
							}
							continue;
						}
					}
					list.Add(new BuildItem(null, text2));
				}
			}
			return list;
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x0000DF0C File Offset: 0x0000CF0C
		internal List<TaskItem> ExpandAllIntoTaskItems(string expression, XmlAttribute expressionAttribute)
		{
			List<BuildItem> list = this.ExpandAllIntoBuildItems(expression, expressionAttribute);
			List<TaskItem> list2 = new List<TaskItem>(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].IsUninitializedItem)
				{
					list2.Add(new TaskItem(list[i]));
				}
				else
				{
					list2.Add(new TaskItem(list[i].FinalItemSpecEscaped));
				}
			}
			return list2;
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0000DF79 File Offset: 0x0000CF79
		internal string ExpandAllIntoString(XmlAttribute expressionAttribute)
		{
			return this.ExpandAllIntoString(expressionAttribute.Value, expressionAttribute);
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0000DF88 File Offset: 0x0000CF88
		internal string ExpandAllIntoString(string expression, XmlNode expressionNode)
		{
			return EscapingUtilities.UnescapeAll(this.ExpandAllIntoStringLeaveEscaped(expression, expressionNode));
		}

		// Token: 0x060002BB RID: 699 RVA: 0x0000DF97 File Offset: 0x0000CF97
		internal string ExpandAllIntoStringLeaveEscaped(XmlAttribute expressionAttribute)
		{
			return this.ExpandAllIntoStringLeaveEscaped(expressionAttribute.Value, expressionAttribute);
		}

		// Token: 0x060002BC RID: 700 RVA: 0x0000DFA6 File Offset: 0x0000CFA6
		internal string ExpandAllIntoStringLeaveEscaped(string expression, XmlNode expressionNode)
		{
			ErrorUtilities.VerifyThrow(expression != null, "Must pass in non-null expression.");
			if (expression.Length == 0)
			{
				return expression;
			}
			return this.ExpandItemsIntoStringLeaveEscaped(this.ExpandPropertiesLeaveEscaped(this.ExpandMetadataLeaveEscaped(expression)), expressionNode);
		}

		// Token: 0x060002BD RID: 701 RVA: 0x0000DFD8 File Offset: 0x0000CFD8
		internal List<string> ExpandAllIntoStringList(string expression, XmlNode expressionNode)
		{
			List<string> list = ExpressionShredder.SplitSemiColonSeparatedList(this.ExpandAllIntoStringLeaveEscaped(expression, expressionNode));
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = EscapingUtilities.UnescapeAll(list[i]);
			}
			return list;
		}

		// Token: 0x060002BE RID: 702 RVA: 0x0000E018 File Offset: 0x0000D018
		internal List<string> ExpandAllIntoStringList(XmlAttribute expressionAttribute)
		{
			return this.ExpandAllIntoStringList(expressionAttribute.Value, expressionAttribute);
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0000E027 File Offset: 0x0000D027
		internal List<string> ExpandAllIntoStringListLeaveEscaped(string expression, XmlNode expressionNode)
		{
			return ExpressionShredder.SplitSemiColonSeparatedList(this.ExpandAllIntoStringLeaveEscaped(expression, expressionNode));
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x0000E036 File Offset: 0x0000D036
		internal List<string> ExpandAllIntoStringListLeaveEscaped(XmlAttribute expressionAttribute)
		{
			return this.ExpandAllIntoStringListLeaveEscaped(expressionAttribute.Value, expressionAttribute);
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x0000E048 File Offset: 0x0000D048
		private string ExpandPropertiesLeaveEscaped(string sourceString)
		{
			if ((this.options & ExpanderOptions.ExpandProperties) != ExpanderOptions.ExpandProperties)
			{
				return sourceString;
			}
			int num = sourceString.IndexOf("$(", StringComparison.Ordinal);
			if (num == -1)
			{
				return sourceString;
			}
			StringBuilder stringBuilder = new StringBuilder(sourceString.Length);
			int num2 = 0;
			while (num != -1)
			{
				stringBuilder.Append(sourceString, num2, num - num2);
				int num3 = sourceString.IndexOf(')', num);
				if (num3 == -1)
				{
					stringBuilder.Append(sourceString, num, sourceString.Length - num);
					num2 = sourceString.Length;
				}
				else
				{
					string text = sourceString.Substring(num + 2, num3 - num - 2);
					string value;
					if (text.StartsWith("Registry:", StringComparison.OrdinalIgnoreCase))
					{
						value = this.ExpandRegistryValue(text, null);
					}
					else
					{
						BuildProperty buildProperty;
						if (this.lookup != null)
						{
							buildProperty = this.lookup.GetProperty(text);
						}
						else
						{
							buildProperty = this.properties[text];
						}
						if (buildProperty == null)
						{
							value = string.Empty;
						}
						else
						{
							value = buildProperty.FinalValueEscaped;
						}
					}
					stringBuilder.Append(value);
					num2 = num3 + 1;
				}
				num = sourceString.IndexOf("$(", num2, StringComparison.Ordinal);
			}
			stringBuilder.Append(sourceString, num2, sourceString.Length - num2);
			return stringBuilder.ToString();
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x0000E164 File Offset: 0x0000D164
		private string ExpandRegistryValue(string registryExpression, XmlNode node)
		{
			string text = registryExpression.Substring(9);
			int num = text.IndexOf('@');
			int num2 = text.LastIndexOf('@');
			ProjectErrorUtilities.VerifyThrowInvalidProject(num == num2, node, "InvalidRegistryPropertyExpression", "$(" + registryExpression + ")", string.Empty);
			string text2 = (num2 == -1 || num2 == text.Length - 1) ? null : text.Substring(num2 + 1);
			string text3 = (num2 != -1) ? text.Substring(0, num2) : text;
			string result = string.Empty;
			if (text3 != null)
			{
				text3 = EscapingUtilities.UnescapeAll(text3);
				if (text2 != null)
				{
					text2 = EscapingUtilities.UnescapeAll(text2);
				}
				try
				{
					object value = Registry.GetValue(text3, text2, null);
					if (value != null)
					{
						ProjectErrorUtilities.VerifyThrowInvalidProject(value is string, node, "NonStringDataInRegistry", text);
						result = Convert.ToString(value, CultureInfo.CurrentCulture);
					}
					else
					{
						result = string.Empty;
					}
				}
				catch (ArgumentException ex)
				{
					ProjectErrorUtilities.VerifyThrowInvalidProject(false, node, "InvalidRegistryPropertyExpression", "$(" + registryExpression + ")", ex.Message);
				}
				catch (IOException ex2)
				{
					ProjectErrorUtilities.VerifyThrowInvalidProject(false, node, "InvalidRegistryPropertyExpression", "$(" + registryExpression + ")", ex2.Message);
				}
				catch (SecurityException ex3)
				{
					ProjectErrorUtilities.VerifyThrowInvalidProject(false, node, "InvalidRegistryPropertyExpression", "$(" + registryExpression + ")", ex3.Message);
				}
			}
			return result;
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x0000E2DC File Offset: 0x0000D2DC
		internal string ExpandMetadataLeaveEscaped(string expression)
		{
			if ((this.options & ExpanderOptions.ExpandMetadata) != ExpanderOptions.ExpandMetadata)
			{
				return expression;
			}
			string result;
			if (expression.IndexOf("%(", StringComparison.Ordinal) == -1)
			{
				result = expression;
			}
			else if (expression.IndexOf("@(", StringComparison.Ordinal) == -1)
			{
				result = ItemExpander.itemMetadataPattern.Replace(expression, new MatchEvaluator(this.ExpandSingleMetadata));
			}
			else if (ItemExpander.listOfItemVectorsWithoutSeparatorsPattern.IsMatch(expression))
			{
				result = expression;
			}
			else
			{
				result = ItemExpander.nonTransformItemMetadataPattern.Replace(expression, new MatchEvaluator(this.ExpandSingleMetadata));
			}
			return result;
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x0000E35C File Offset: 0x0000D35C
		private string ExpandSingleMetadata(Match itemMetadataMatch)
		{
			ErrorUtilities.VerifyThrow(itemMetadataMatch.Success, "Need a valid item metadata.");
			string value = itemMetadataMatch.Groups["NAME"].Value;
			string itemType = null;
			if (itemMetadataMatch.Groups["ITEM_SPECIFICATION"].Length > 0)
			{
				itemType = itemMetadataMatch.Groups["TYPE"].Value;
			}
			string text = null;
			text = this.GetValueFromMetadataTable(itemType, value, text);
			if (text == null)
			{
				text = this.GetDefaultMetadataValue(itemType, value, text);
			}
			return text ?? string.Empty;
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0000E3E4 File Offset: 0x0000D3E4
		private string GetValueFromMetadataTable(string itemType, string metadataName, string metadataValue)
		{
			if (this.itemMetadata == null)
			{
				return null;
			}
			if (this.implicitMetadataItemType == null)
			{
				string key;
				if (itemType == null)
				{
					key = metadataName;
				}
				else
				{
					key = itemType + "." + metadataName;
				}
				this.itemMetadata.TryGetValue(key, out metadataValue);
			}
			else if (itemType == null || string.Equals(itemType, this.implicitMetadataItemType, StringComparison.OrdinalIgnoreCase))
			{
				this.itemMetadata.TryGetValue(metadataName, out metadataValue);
			}
			return metadataValue;
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0000E44A File Offset: 0x0000D44A
		private string GetDefaultMetadataValue(string itemType, string metadataName, string metadataValue)
		{
			if (this.specificItemDefinitionLibrary == null)
			{
				return null;
			}
			if (itemType == null || string.Equals(itemType, this.specificItemDefinitionLibrary.ItemType, StringComparison.OrdinalIgnoreCase))
			{
				metadataValue = this.specificItemDefinitionLibrary.GetDefaultMetadataValue(metadataName);
			}
			return metadataValue;
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x0000E47C File Offset: 0x0000D47C
		private string ExpandItemsIntoStringLeaveEscaped(string expression, XmlNode expressionNode)
		{
			if (string.IsNullOrEmpty(expression) || this.lookup == null)
			{
				return expression;
			}
			if ((this.options & ExpanderOptions.ExpandItems) != ExpanderOptions.ExpandItems)
			{
				return expression;
			}
			return ItemExpander.ExpandEmbeddedItemVectors(expression, expressionNode, this.lookup);
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x0000E4AC File Offset: 0x0000D4AC
		internal BuildItemGroup ExpandSingleItemListExpressionIntoItemsLeaveEscaped(string singleItemVectorExpression, XmlAttribute itemVectorAttribute)
		{
			if (this.lookup == null)
			{
				return null;
			}
			Match match;
			return this.ExpandSingleItemListExpressionIntoItemsLeaveEscaped(singleItemVectorExpression, itemVectorAttribute, out match);
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x0000E4CD File Offset: 0x0000D4CD
		internal BuildItemGroup ExpandSingleItemListExpressionIntoItemsLeaveEscaped(string singleItemVectorExpression, XmlAttribute itemVectorAttribute, out Match itemVectorMatch)
		{
			ErrorUtilities.VerifyThrow(this.lookup != null, "Need items");
			if ((this.options & ExpanderOptions.ExpandItems) != ExpanderOptions.ExpandItems)
			{
				itemVectorMatch = null;
				return null;
			}
			return ItemExpander.ItemizeItemVector(singleItemVectorExpression, itemVectorAttribute, this.lookup, out itemVectorMatch);
		}

		// Token: 0x04000135 RID: 309
		private ReadOnlyLookup lookup;

		// Token: 0x04000136 RID: 310
		private BuildPropertyGroup properties;

		// Token: 0x04000137 RID: 311
		private Dictionary<string, string> itemMetadata;

		// Token: 0x04000138 RID: 312
		private string implicitMetadataItemType;

		// Token: 0x04000139 RID: 313
		private SpecificItemDefinitionLibrary specificItemDefinitionLibrary;

		// Token: 0x0400013A RID: 314
		private ExpanderOptions options;
	}
}
 
