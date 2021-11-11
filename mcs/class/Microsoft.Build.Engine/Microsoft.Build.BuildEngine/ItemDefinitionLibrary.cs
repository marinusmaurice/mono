using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Build.Shared;

namespace Microsoft.Build.BuildEngine
{
	// Token: 0x020000A3 RID: 163
	internal class ItemDefinitionLibrary
	{
		// Token: 0x060006F8 RID: 1784 RVA: 0x00025A74 File Offset: 0x00024A74
		internal ItemDefinitionLibrary(Project parentProject)
		{
			this.parentProject = parentProject;
			this.itemDefinitions = new List<ItemDefinitionLibrary.BuildItemDefinitionGroupXml>();
			this.itemDefinitionsDictionary = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x00025AA0 File Offset: 0x00024AA0
		internal void Add(XmlElement element)
		{
			ItemDefinitionLibrary.BuildItemDefinitionGroupXml item = new ItemDefinitionLibrary.BuildItemDefinitionGroupXml(element, this.parentProject);
			this.itemDefinitions.Add(item);
			this.evaluated = false;
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x00025AD0 File Offset: 0x00024AD0
		internal void Evaluate(BuildPropertyGroup evaluatedProperties)
		{
			this.itemDefinitionsDictionary.Clear();
			foreach (ItemDefinitionLibrary.BuildItemDefinitionGroupXml buildItemDefinitionGroupXml in this.itemDefinitions)
			{
				buildItemDefinitionGroupXml.Evaluate(evaluatedProperties, this.itemDefinitionsDictionary);
			}
			this.evaluated = true;
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x00025B3C File Offset: 0x00024B3C
		internal string GetDefaultMetadataValue(string itemType, string metadataName)
		{
			this.MustBeEvaluated();
			string result = null;
			Dictionary<string, string> dictionary;
			if (this.itemDefinitionsDictionary.TryGetValue(itemType, out dictionary))
			{
				dictionary.TryGetValue(metadataName, out result);
			}
			return result;
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x00025B6C File Offset: 0x00024B6C
		internal int GetDefaultedMetadataCount(string itemType)
		{
			this.MustBeEvaluated();
			Dictionary<string, string> dictionary;
			if (this.itemDefinitionsDictionary.TryGetValue(itemType, out dictionary))
			{
				return dictionary.Count;
			}
			return 0;
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x00025B98 File Offset: 0x00024B98
		internal ICollection<string> GetDefaultedMetadataNames(string itemType)
		{
			this.MustBeEvaluated();
			Dictionary<string, string> defaultedMetadata = this.GetDefaultedMetadata(itemType);
			if (defaultedMetadata != null)
			{
				return defaultedMetadata.Keys;
			}
			return null;
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x00025BC0 File Offset: 0x00024BC0
		internal Dictionary<string, string> GetDefaultedMetadata(string itemType)
		{
			this.MustBeEvaluated();
			Dictionary<string, string> result;
			if (this.itemDefinitionsDictionary.TryGetValue(itemType, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x00025BE6 File Offset: 0x00024BE6
		private void MustBeEvaluated()
		{
			ErrorUtilities.VerifyThrowNoAssert(this.evaluated, "Must be evaluated to query");
		}

		// Token: 0x04000347 RID: 839
		private Project parentProject;

		// Token: 0x04000348 RID: 840
		private List<ItemDefinitionLibrary.BuildItemDefinitionGroupXml> itemDefinitions;

		// Token: 0x04000349 RID: 841
		private Dictionary<string, Dictionary<string, string>> itemDefinitionsDictionary;

		// Token: 0x0400034A RID: 842
		private bool evaluated;

		// Token: 0x020000A4 RID: 164
		private class BuildItemDefinitionGroupXml
		{
			// Token: 0x06000700 RID: 1792 RVA: 0x00025BF8 File Offset: 0x00024BF8
			internal BuildItemDefinitionGroupXml(XmlElement element, Project parentProject)
			{
				ProjectXmlUtilities.VerifyThrowElementName(element, "ItemDefinitionGroup");
				ProjectXmlUtilities.VerifyThrowProjectValidNamespace(element);
				this.element = element;
				this.parentProject = parentProject;
				this.conditionAttribute = ProjectXmlUtilities.GetConditionAttribute(element, true);
				this.condition = ProjectXmlUtilities.GetAttributeValue(this.conditionAttribute);
			}

			// Token: 0x06000701 RID: 1793 RVA: 0x00025C48 File Offset: 0x00024C48
			internal void Evaluate(BuildPropertyGroup properties, Dictionary<string, Dictionary<string, string>> itemDefinitionsDictionary)
			{
				Expander expander = new Expander(properties);
				if (!Utilities.EvaluateCondition(this.condition, this.conditionAttribute, expander, ParserOptions.AllowProperties, this.parentProject))
				{
					return;
				}
				List<XmlElement> validChildElements = ProjectXmlUtilities.GetValidChildElements(this.element);
				foreach (XmlElement itemDefinitionElement in validChildElements)
				{
					this.EvaluateItemDefinitionElement(itemDefinitionElement, properties, itemDefinitionsDictionary);
				}
			}

			// Token: 0x06000702 RID: 1794 RVA: 0x00025CC8 File Offset: 0x00024CC8
			private void EvaluateItemDefinitionElement(XmlElement itemDefinitionElement, BuildPropertyGroup properties, Dictionary<string, Dictionary<string, string>> itemDefinitionsDictionary)
			{
				ProjectXmlUtilities.VerifyThrowProjectValidNameAndNamespace(itemDefinitionElement);
				XmlAttribute attribute = ProjectXmlUtilities.GetConditionAttribute(itemDefinitionElement, true);
				string attributeValue = ProjectXmlUtilities.GetAttributeValue(attribute);
				Dictionary<string, string> unqualifiedItemMetadata = null;
				string name = itemDefinitionElement.Name;
				itemDefinitionsDictionary.TryGetValue(name, out unqualifiedItemMetadata);
				Expander expander = new Expander(properties, name, unqualifiedItemMetadata);
				if (!Utilities.EvaluateCondition(attributeValue, attribute, expander, ParserOptions.AllowPropertiesAndItemMetadata, this.parentProject))
				{
					return;
				}
				List<XmlElement> validChildElements = ProjectXmlUtilities.GetValidChildElements(itemDefinitionElement);
				foreach (XmlElement itemDefinitionChildElement in validChildElements)
				{
					this.EvaluateItemDefinitionChildElement(itemDefinitionChildElement, properties, itemDefinitionsDictionary);
				}
			}

			// Token: 0x06000703 RID: 1795 RVA: 0x00025D6C File Offset: 0x00024D6C
			private void EvaluateItemDefinitionChildElement(XmlElement itemDefinitionChildElement, BuildPropertyGroup properties, Dictionary<string, Dictionary<string, string>> itemDefinitionsDictionary)
			{
				ProjectXmlUtilities.VerifyThrowProjectValidNameAndNamespace(itemDefinitionChildElement);
				ProjectErrorUtilities.VerifyThrowInvalidProject(!FileUtilities.IsItemSpecModifier(itemDefinitionChildElement.Name), itemDefinitionChildElement, "ItemSpecModifierCannotBeCustomMetadata", itemDefinitionChildElement.Name);
				ProjectErrorUtilities.VerifyThrowInvalidProject(XMakeElements.IllegalItemPropertyNames[itemDefinitionChildElement.Name] == null, itemDefinitionChildElement, "CannotModifyReservedItemMetadata", itemDefinitionChildElement.Name);
				XmlAttribute attribute = ProjectXmlUtilities.GetConditionAttribute(itemDefinitionChildElement, true);
				string attributeValue = ProjectXmlUtilities.GetAttributeValue(attribute);
				Dictionary<string, string> dictionary = null;
				string name = itemDefinitionChildElement.ParentNode.Name;
				itemDefinitionsDictionary.TryGetValue(name, out dictionary);
				Expander expander = new Expander(properties, name, dictionary);
				if (!Utilities.EvaluateCondition(attributeValue, attribute, expander, ParserOptions.AllowPropertiesAndItemMetadata, this.parentProject))
				{
					return;
				}
				string xmlNodeInnerContents = Utilities.GetXmlNodeInnerContents(itemDefinitionChildElement);
				bool flag = ItemExpander.ExpressionContainsItemVector(xmlNodeInnerContents);
				ProjectErrorUtilities.VerifyThrowInvalidProject(!flag, itemDefinitionChildElement, "MetadataDefinitionCannotContainItemVectorExpression", xmlNodeInnerContents, itemDefinitionChildElement.Name);
				string value = expander.ExpandAllIntoStringLeaveEscaped(xmlNodeInnerContents, itemDefinitionChildElement);
				if (dictionary == null)
				{
					dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
					itemDefinitionsDictionary.Add(name, dictionary);
				}
				dictionary[itemDefinitionChildElement.Name] = value;
			}

			// Token: 0x0400034B RID: 843
			private XmlElement element;

			// Token: 0x0400034C RID: 844
			private Project parentProject;

			// Token: 0x0400034D RID: 845
			private XmlAttribute conditionAttribute;

			// Token: 0x0400034E RID: 846
			private string condition;
		}
	}
}
