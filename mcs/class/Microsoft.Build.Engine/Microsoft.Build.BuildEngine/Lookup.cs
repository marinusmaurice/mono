using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Build.Shared;

namespace Microsoft.Build.BuildEngine
{
	// Token: 0x0200009B RID: 155
	internal class Lookup
	{
		// Token: 0x0600061B RID: 1563 RVA: 0x00021B45 File Offset: 0x00020B45
		internal Lookup(Hashtable itemsByName, BuildPropertyGroup properties, ItemDefinitionLibrary itemDefinitionLibrary) : this(itemsByName, new BuildItemGroup(), properties, itemDefinitionLibrary)
		{
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x00021B58 File Offset: 0x00020B58
		internal Lookup(Hashtable itemsByName, BuildItemGroup projectItems, BuildPropertyGroup properties, ItemDefinitionLibrary itemDefinitionLibrary)
		{
			this.lookupEntries = new LinkedList<LookupEntry>();
			base..ctor();
			ErrorUtilities.VerifyThrow(itemDefinitionLibrary != null, "Expect library");
			this.projectItems = projectItems;
			this.itemDefinitionLibrary = itemDefinitionLibrary;
			LookupEntry value = new LookupEntry(itemsByName, properties);
			this.lookupEntries.AddFirst(value);
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x00021BAC File Offset: 0x00020BAC
		private Lookup(Lookup that)
		{
			this.lookupEntries = new LinkedList<LookupEntry>();
			base..ctor();
			foreach (LookupEntry value in that.lookupEntries)
			{
				this.lookupEntries.AddLast(value);
			}
			this.projectItems = that.projectItems;
			this.itemDefinitionLibrary = that.itemDefinitionLibrary;
			this.cloneTable = that.cloneTable;
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x0600061E RID: 1566 RVA: 0x00021C3C File Offset: 0x00020C3C
		internal ReadOnlyLookup ReadOnlyLookup
		{
			get
			{
				if (this.readOnlyLookup == null)
				{
					this.readOnlyLookup = new ReadOnlyLookup(this);
				}
				return this.readOnlyLookup;
			}
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x0600061F RID: 1567 RVA: 0x00021C58 File Offset: 0x00020C58
		// (set) Token: 0x06000620 RID: 1568 RVA: 0x00021C6F File Offset: 0x00020C6F
		private Hashtable PrimaryTable
		{
			get
			{
				return this.lookupEntries.First.Value.Items;
			}
			set
			{
				this.lookupEntries.First.Value.Items = value;
			}
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06000621 RID: 1569 RVA: 0x00021C87 File Offset: 0x00020C87
		// (set) Token: 0x06000622 RID: 1570 RVA: 0x00021C9E File Offset: 0x00020C9E
		private Hashtable PrimaryAddTable
		{
			get
			{
				return this.lookupEntries.First.Value.Adds;
			}
			set
			{
				this.lookupEntries.First.Value.Adds = value;
			}
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06000623 RID: 1571 RVA: 0x00021CB6 File Offset: 0x00020CB6
		// (set) Token: 0x06000624 RID: 1572 RVA: 0x00021CCD File Offset: 0x00020CCD
		private Hashtable PrimaryRemoveTable
		{
			get
			{
				return this.lookupEntries.First.Value.Removes;
			}
			set
			{
				this.lookupEntries.First.Value.Removes = value;
			}
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000625 RID: 1573 RVA: 0x00021CE5 File Offset: 0x00020CE5
		// (set) Token: 0x06000626 RID: 1574 RVA: 0x00021CFC File Offset: 0x00020CFC
		private Dictionary<string, Dictionary<BuildItem, Dictionary<string, string>>> PrimaryModifyTable
		{
			get
			{
				return this.lookupEntries.First.Value.Modifies;
			}
			set
			{
				this.lookupEntries.First.Value.Modifies = value;
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06000627 RID: 1575 RVA: 0x00021D14 File Offset: 0x00020D14
		// (set) Token: 0x06000628 RID: 1576 RVA: 0x00021D2B File Offset: 0x00020D2B
		private BuildPropertyGroup PrimaryPropertySets
		{
			get
			{
				return this.lookupEntries.First.Value.PropertySets;
			}
			set
			{
				this.lookupEntries.First.Value.PropertySets = value;
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000629 RID: 1577 RVA: 0x00021D43 File Offset: 0x00020D43
		// (set) Token: 0x0600062A RID: 1578 RVA: 0x00021D5F File Offset: 0x00020D5F
		private Hashtable SecondaryTable
		{
			get
			{
				return this.lookupEntries.First.Next.Value.Items;
			}
			set
			{
				this.lookupEntries.First.Next.Value.Items = value;
			}
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x0600062B RID: 1579 RVA: 0x00021D7C File Offset: 0x00020D7C
		// (set) Token: 0x0600062C RID: 1580 RVA: 0x00021D98 File Offset: 0x00020D98
		private Hashtable SecondaryAddTable
		{
			get
			{
				return this.lookupEntries.First.Next.Value.Adds;
			}
			set
			{
				this.lookupEntries.First.Next.Value.Adds = value;
			}
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x0600062D RID: 1581 RVA: 0x00021DB5 File Offset: 0x00020DB5
		// (set) Token: 0x0600062E RID: 1582 RVA: 0x00021DD1 File Offset: 0x00020DD1
		private Hashtable SecondaryRemoveTable
		{
			get
			{
				return this.lookupEntries.First.Next.Value.Removes;
			}
			set
			{
				this.lookupEntries.First.Next.Value.Removes = value;
			}
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x0600062F RID: 1583 RVA: 0x00021DEE File Offset: 0x00020DEE
		// (set) Token: 0x06000630 RID: 1584 RVA: 0x00021E0A File Offset: 0x00020E0A
		private Dictionary<string, Dictionary<BuildItem, Dictionary<string, string>>> SecondaryModifyTable
		{
			get
			{
				return this.lookupEntries.First.Next.Value.Modifies;
			}
			set
			{
				this.lookupEntries.First.Next.Value.Modifies = value;
			}
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06000631 RID: 1585 RVA: 0x00021E27 File Offset: 0x00020E27
		// (set) Token: 0x06000632 RID: 1586 RVA: 0x00021E43 File Offset: 0x00020E43
		private BuildPropertyGroup SecondaryProperties
		{
			get
			{
				return this.lookupEntries.First.Next.Value.Properties;
			}
			set
			{
				this.lookupEntries.First.Next.Value.Properties = value;
			}
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06000633 RID: 1587 RVA: 0x00021E60 File Offset: 0x00020E60
		// (set) Token: 0x06000634 RID: 1588 RVA: 0x00021E7C File Offset: 0x00020E7C
		private BuildPropertyGroup SecondaryPropertySets
		{
			get
			{
				return this.lookupEntries.First.Next.Value.PropertySets;
			}
			set
			{
				this.lookupEntries.First.Next.Value.PropertySets = value;
			}
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x00021E9C File Offset: 0x00020E9C
		internal List<string> GetPropertyOverrideMessages(Hashtable lookupHash)
		{
			List<string> list = null;
			if (this.PrimaryPropertySets != null)
			{
				foreach (object obj in this.PrimaryPropertySets)
				{
					BuildProperty buildProperty = (BuildProperty)obj;
					string name = buildProperty.Name;
					if (lookupHash.ContainsKey(name))
					{
						if (list == null)
						{
							list = new List<string>();
						}
						list.Add(ResourceUtilities.FormatResourceString("PropertyOutputOverridden", new object[]
						{
							name,
							lookupHash[name],
							buildProperty.FinalValueEscaped
						}));
					}
					lookupHash[name] = buildProperty.FinalValueEscaped;
				}
			}
			return list;
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x00021F5C File Offset: 0x00020F5C
		internal Lookup Clone()
		{
			return new Lookup(this);
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x00021F64 File Offset: 0x00020F64
		internal LookupEntry EnterScope()
		{
			LookupEntry lookupEntry = new LookupEntry(null, null);
			this.lookupEntries.AddFirst(lookupEntry);
			return lookupEntry;
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x00021F88 File Offset: 0x00020F88
		internal void LeaveScope()
		{
			this.MustBeOwningThread();
			ErrorUtilities.VerifyThrowNoAssert(this.lookupEntries.Count >= 2, "Too many calls to Leave().");
			if (this.lookupEntries.Count == 2)
			{
				this.MergeScopeIntoLastScope();
			}
			else
			{
				this.MergeScopeIntoNotLastScope();
			}
			this.cloneTable = null;
			this.lookupEntries.RemoveFirst();
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x00021FE4 File Offset: 0x00020FE4
		private void MergeScopeIntoNotLastScope()
		{
			if (this.PrimaryAddTable != null)
			{
				if (this.SecondaryAddTable == null)
				{
					this.SecondaryAddTable = this.PrimaryAddTable;
				}
				else
				{
					foreach (object obj in this.PrimaryAddTable)
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
						this.ImportItemsIntoTable(this.SecondaryAddTable, (string)dictionaryEntry.Key, (BuildItemGroup)dictionaryEntry.Value);
					}
				}
			}
			if (this.PrimaryRemoveTable != null)
			{
				if (this.SecondaryRemoveTable == null)
				{
					this.SecondaryRemoveTable = this.PrimaryRemoveTable;
				}
				else
				{
					foreach (object obj2 in this.PrimaryRemoveTable)
					{
						DictionaryEntry dictionaryEntry2 = (DictionaryEntry)obj2;
						this.ImportItemsIntoTable(this.SecondaryRemoveTable, (string)dictionaryEntry2.Key, (BuildItemGroup)dictionaryEntry2.Value);
					}
				}
			}
			if (this.PrimaryModifyTable != null)
			{
				if (this.SecondaryModifyTable == null)
				{
					this.SecondaryModifyTable = this.PrimaryModifyTable;
				}
				else
				{
					foreach (KeyValuePair<string, Dictionary<BuildItem, Dictionary<string, string>>> keyValuePair in this.PrimaryModifyTable)
					{
						Dictionary<BuildItem, Dictionary<string, string>> modifiesOfType;
						if (this.SecondaryModifyTable.TryGetValue(keyValuePair.Key, out modifiesOfType))
						{
							using (Dictionary<BuildItem, Dictionary<string, string>>.Enumerator enumerator4 = keyValuePair.Value.GetEnumerator())
							{
								while (enumerator4.MoveNext())
								{
									KeyValuePair<BuildItem, Dictionary<string, string>> modify = enumerator4.Current;
									this.MergeModificationsIntoModificationTable(modifiesOfType, modify, Lookup.ModifyMergeType.SecondWins);
								}
								continue;
							}
						}
						this.SecondaryModifyTable.Add(keyValuePair.Key, keyValuePair.Value);
					}
				}
			}
			if (this.PrimaryPropertySets != null)
			{
				if (this.SecondaryPropertySets == null)
				{
					this.SecondaryPropertySets = this.PrimaryPropertySets;
					return;
				}
				this.SecondaryPropertySets.ImportProperties(this.PrimaryPropertySets);
			}
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x00022210 File Offset: 0x00021210
		private void MergeScopeIntoLastScope()
		{
			if (this.PrimaryAddTable != null)
			{
				foreach (object obj in this.PrimaryAddTable)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					this.SecondaryTable = Utilities.CreateTableIfNecessary(this.SecondaryTable);
					this.ImportItemsIntoTable(this.SecondaryTable, (string)dictionaryEntry.Key, (BuildItemGroup)dictionaryEntry.Value);
					this.projectItems.ImportItems((BuildItemGroup)dictionaryEntry.Value);
				}
			}
			if (this.PrimaryRemoveTable != null)
			{
				foreach (object obj2 in this.PrimaryRemoveTable)
				{
					DictionaryEntry dictionaryEntry2 = (DictionaryEntry)obj2;
					this.SecondaryTable = Utilities.CreateTableIfNecessary(this.SecondaryTable);
					this.RemoveItemsFromTableWithBackup(this.SecondaryTable, (string)dictionaryEntry2.Key, (BuildItemGroup)dictionaryEntry2.Value);
					this.projectItems.RemoveItemsWithBackup((BuildItemGroup)dictionaryEntry2.Value);
				}
			}
			if (this.PrimaryModifyTable != null)
			{
				foreach (KeyValuePair<string, Dictionary<BuildItem, Dictionary<string, string>>> keyValuePair in this.PrimaryModifyTable)
				{
					this.SecondaryTable = Utilities.CreateTableIfNecessary(this.SecondaryTable);
					this.ApplyModificationsToTable(this.SecondaryTable, keyValuePair.Key, keyValuePair.Value);
				}
			}
			if (this.PrimaryPropertySets != null)
			{
				this.SecondaryProperties = this.CreatePropertyGroupIfNecessary(this.SecondaryProperties);
				this.SecondaryProperties.ImportProperties(this.PrimaryPropertySets);
			}
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x000223F8 File Offset: 0x000213F8
		internal BuildProperty GetProperty(string name)
		{
			foreach (LookupEntry lookupEntry in this.lookupEntries)
			{
				if (lookupEntry.PropertySets != null)
				{
					BuildProperty buildProperty = lookupEntry.PropertySets[name];
					if (buildProperty != null)
					{
						return buildProperty;
					}
				}
				if (lookupEntry.Properties != null)
				{
					BuildProperty buildProperty2 = lookupEntry.Properties[name];
					if (buildProperty2 != null)
					{
						return buildProperty2;
					}
				}
				if (lookupEntry.TruncateLookupsAtThisScope)
				{
					break;
				}
			}
			return null;
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x00022488 File Offset: 0x00021488
		internal BuildItemGroup GetItems(string name)
		{
			BuildItemGroup buildItemGroup = null;
			BuildItemGroup buildItemGroup2 = null;
			Dictionary<BuildItem, Dictionary<string, string>> dictionary = null;
			BuildItemGroup buildItemGroup3 = null;
			foreach (LookupEntry lookupEntry in this.lookupEntries)
			{
				if (lookupEntry.Adds != null)
				{
					BuildItemGroup buildItemGroup4 = (BuildItemGroup)lookupEntry.Adds[name];
					if (buildItemGroup4 != null)
					{
						buildItemGroup = this.CreateItemGroupIfNecessary(buildItemGroup);
						buildItemGroup.ImportItems(buildItemGroup4);
					}
				}
				if (lookupEntry.Removes != null)
				{
					BuildItemGroup buildItemGroup5 = (BuildItemGroup)lookupEntry.Removes[name];
					if (buildItemGroup5 != null)
					{
						buildItemGroup2 = this.CreateItemGroupIfNecessary(buildItemGroup2);
						buildItemGroup2.ImportItems(buildItemGroup5);
					}
				}
				Dictionary<BuildItem, Dictionary<string, string>> dictionary2;
				if (lookupEntry.Modifies != null && lookupEntry.Modifies.TryGetValue(name, out dictionary2))
				{
					if (dictionary == null)
					{
						dictionary = new Dictionary<BuildItem, Dictionary<string, string>>();
					}
					foreach (KeyValuePair<BuildItem, Dictionary<string, string>> modify in dictionary2)
					{
						this.MergeModificationsIntoModificationTable(dictionary, modify, Lookup.ModifyMergeType.FirstWins);
					}
				}
				if (lookupEntry.Items != null)
				{
					buildItemGroup3 = (BuildItemGroup)lookupEntry.Items[name];
					if (buildItemGroup3 != null)
					{
						break;
					}
				}
				if (lookupEntry.TruncateLookupsAtThisScope)
				{
					break;
				}
			}
			if ((buildItemGroup == null || buildItemGroup.Count == 0) && (buildItemGroup2 == null || buildItemGroup2.Count == 0) && (dictionary == null || dictionary.Count == 0))
			{
				if (buildItemGroup3 == null)
				{
					buildItemGroup3 = new BuildItemGroup();
				}
				return buildItemGroup3;
			}
			BuildItemGroup buildItemGroup6 = new BuildItemGroup();
			if (buildItemGroup3 != null)
			{
				buildItemGroup6.ImportItems(buildItemGroup3);
			}
			if (buildItemGroup != null)
			{
				buildItemGroup6.ImportItems(buildItemGroup);
			}
			if (buildItemGroup2 != null)
			{
				buildItemGroup6.RemoveItems(buildItemGroup2);
			}
			if (dictionary != null)
			{
				this.ApplyModifies(buildItemGroup6, dictionary);
			}
			return buildItemGroup6;
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x00022650 File Offset: 0x00021650
		internal void PopulateWithItems(string name, BuildItemGroup group)
		{
			this.MustBeOwningThread();
			this.PrimaryTable = Utilities.CreateTableIfNecessary(this.PrimaryTable);
			BuildItemGroup buildItemGroup = (BuildItemGroup)this.PrimaryTable[name];
			ErrorUtilities.VerifyThrow(buildItemGroup == null, "Cannot add an itemgroup of this type.");
			this.PrimaryTable[name] = group;
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x000226A1 File Offset: 0x000216A1
		internal void PopulateWithItem(BuildItem item)
		{
			this.MustBeOwningThread();
			this.PrimaryTable = Utilities.CreateTableIfNecessary(this.PrimaryTable);
			this.ImportItemIntoTable(this.PrimaryTable, item);
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x000226C7 File Offset: 0x000216C7
		internal void SetProperty(BuildProperty property)
		{
			this.MustBeOwningThread();
			ErrorUtilities.VerifyThrow(property.Type == PropertyType.OutputProperty, "Expected output property");
			this.MustNotBeOuterScope();
			this.PrimaryPropertySets = this.CreatePropertyGroupIfNecessary(this.PrimaryPropertySets);
			this.PrimaryPropertySets.SetProperty(property);
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x00022708 File Offset: 0x00021708
		internal void AddNewItems(BuildItemGroup group)
		{
			this.MustBeOwningThread();
			this.MustNotBeOuterScope();
			if (group.Count == 0)
			{
				return;
			}
			foreach (object obj in group)
			{
				BuildItem buildItem = (BuildItem)obj;
				ErrorUtilities.VerifyThrow(!buildItem.IsPartOfProjectManifest, "Cannot dynamically add manifest items");
				buildItem.ItemDefinitionLibrary = this.itemDefinitionLibrary;
			}
			this.PrimaryAddTable = Utilities.CreateTableIfNecessary(this.PrimaryAddTable);
			this.ImportItemsIntoTable(this.PrimaryAddTable, group[0].Name, group);
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x000227B4 File Offset: 0x000217B4
		internal void AddNewItem(BuildItem item)
		{
			this.MustBeOwningThread();
			ErrorUtilities.VerifyThrow(!item.IsPartOfProjectManifest, "Cannot dynamically add manifest items");
			this.MustNotBeOuterScope();
			item.ItemDefinitionLibrary = this.itemDefinitionLibrary;
			this.PrimaryAddTable = Utilities.CreateTableIfNecessary(this.PrimaryAddTable);
			this.ImportItemIntoTable(this.PrimaryAddTable, item);
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x0002280C File Offset: 0x0002180C
		internal void RemoveItems(List<BuildItem> items)
		{
			this.MustBeOwningThread();
			foreach (BuildItem item in items)
			{
				this.RemoveItem(item);
			}
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x00022860 File Offset: 0x00021860
		internal void RemoveItem(BuildItem item)
		{
			this.MustBeOwningThread();
			this.MustNotBeOuterScope();
			item = this.RetrieveOriginalFromCloneTable(item);
			this.PrimaryRemoveTable = Utilities.CreateTableIfNecessary(this.PrimaryRemoveTable);
			this.ImportItemIntoTable(this.PrimaryRemoveTable, item);
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x00022898 File Offset: 0x00021898
		internal void ModifyItems(string name, BuildItemGroup group, Dictionary<string, string> metadataChanges)
		{
			this.MustBeOwningThread();
			this.MustNotBeOuterScope();
			if (metadataChanges.Count == 0)
			{
				return;
			}
			this.PrimaryModifyTable = this.CreateTableIfNecessary(this.PrimaryModifyTable);
			Dictionary<BuildItem, Dictionary<string, string>> dictionary;
			if (!this.PrimaryModifyTable.TryGetValue(name, out dictionary))
			{
				dictionary = new Dictionary<BuildItem, Dictionary<string, string>>();
				this.PrimaryModifyTable[name] = dictionary;
			}
			foreach (object obj in group)
			{
				BuildItem item = (BuildItem)obj;
				BuildItem key = this.RetrieveOriginalFromCloneTable(item);
				KeyValuePair<BuildItem, Dictionary<string, string>> modify = new KeyValuePair<BuildItem, Dictionary<string, string>>(key, metadataChanges);
				this.MergeModificationsIntoModificationTable(dictionary, modify, Lookup.ModifyMergeType.SecondWins);
			}
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x00022950 File Offset: 0x00021950
		private void ApplyModifies(BuildItemGroup result, Dictionary<BuildItem, Dictionary<string, string>> allModifies)
		{
			if (this.cloneTable == null)
			{
				this.cloneTable = new Dictionary<BuildItem, BuildItem>();
			}
			foreach (KeyValuePair<BuildItem, Dictionary<string, string>> keyValuePair in allModifies)
			{
				BuildItem buildItem = result.ModifyItemAfterCloningUsingVirtualMetadata(keyValuePair.Key, keyValuePair.Value);
				if (buildItem != null)
				{
					this.cloneTable[buildItem] = keyValuePair.Key;
				}
			}
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x000229D8 File Offset: 0x000219D8
		private BuildItem RetrieveOriginalFromCloneTable(BuildItem item)
		{
			BuildItem buildItem;
			if (this.cloneTable != null && this.cloneTable.TryGetValue(item, out buildItem))
			{
				item = buildItem;
			}
			return item;
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x00022A04 File Offset: 0x00021A04
		private void ImportItemsIntoTable(Hashtable table, string name, BuildItemGroup group)
		{
			BuildItemGroup buildItemGroup = (BuildItemGroup)table[name];
			if (buildItemGroup == null)
			{
				table[name] = group;
				return;
			}
			buildItemGroup.ImportItems(group);
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x00022A34 File Offset: 0x00021A34
		private void RemoveItemsFromTableWithBackup(Hashtable table, string name, BuildItemGroup group)
		{
			BuildItemGroup buildItemGroup = (BuildItemGroup)table[name];
			if (buildItemGroup != null)
			{
				buildItemGroup.RemoveItemsWithBackup(group);
			}
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x00022A58 File Offset: 0x00021A58
		private void ApplyModificationsToTable(Hashtable table, string name, Dictionary<BuildItem, Dictionary<string, string>> modify)
		{
			BuildItemGroup buildItemGroup = (BuildItemGroup)table[name];
			if (buildItemGroup != null)
			{
				buildItemGroup.ModifyItemsUsingVirtualMetadata(modify);
			}
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x00022A7C File Offset: 0x00021A7C
		private void MergeModificationsIntoModificationTable(Dictionary<BuildItem, Dictionary<string, string>> modifiesOfType, KeyValuePair<BuildItem, Dictionary<string, string>> modify, Lookup.ModifyMergeType mergeType)
		{
			Dictionary<string, string> dictionary;
			if (modifiesOfType.TryGetValue(modify.Key, out dictionary))
			{
				using (Dictionary<string, string>.Enumerator enumerator = modify.Value.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, string> keyValuePair = enumerator.Current;
						if (mergeType == Lookup.ModifyMergeType.SecondWins)
						{
							dictionary[keyValuePair.Key] = keyValuePair.Value;
						}
						else if (!dictionary.ContainsKey(keyValuePair.Key))
						{
							dictionary[keyValuePair.Key] = keyValuePair.Value;
						}
					}
					return;
				}
			}
			modifiesOfType.Add(modify.Key, modify.Value);
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x00022B2C File Offset: 0x00021B2C
		private void ImportItemIntoTable(Hashtable table, BuildItem item)
		{
			BuildItemGroup buildItemGroup = (BuildItemGroup)table[item.Name];
			if (buildItemGroup == null)
			{
				buildItemGroup = new BuildItemGroup();
				table.Add(item.Name, buildItemGroup);
			}
			buildItemGroup.AddItem(item);
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x00022B68 File Offset: 0x00021B68
		private Dictionary<string, Dictionary<BuildItem, Dictionary<string, string>>> CreateTableIfNecessary(Dictionary<string, Dictionary<BuildItem, Dictionary<string, string>>> table)
		{
			if (table == null)
			{
				return new Dictionary<string, Dictionary<BuildItem, Dictionary<string, string>>>(StringComparer.OrdinalIgnoreCase);
			}
			return table;
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x00022B79 File Offset: 0x00021B79
		private BuildPropertyGroup CreatePropertyGroupIfNecessary(BuildPropertyGroup properties)
		{
			if (properties == null)
			{
				return new BuildPropertyGroup();
			}
			return properties;
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x00022B85 File Offset: 0x00021B85
		private BuildItemGroup CreateItemGroupIfNecessary(BuildItemGroup items)
		{
			if (items == null)
			{
				return new BuildItemGroup();
			}
			return items;
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x00022B94 File Offset: 0x00021B94
		private void MustBeOwningThread()
		{
			int threadIdThatEnteredScope = this.lookupEntries.First.Value.ThreadIdThatEnteredScope;
			ErrorUtilities.VerifyThrowNoAssert(threadIdThatEnteredScope == Thread.CurrentThread.ManagedThreadId, "Only the thread that entered a scope may modify or leave it");
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x00022BCE File Offset: 0x00021BCE
		private void MustNotBeOuterScope()
		{
			ErrorUtilities.VerifyThrowNoAssert(this.lookupEntries.Count > 1, "Operation in outer scope not supported");
		}

		// Token: 0x0400031C RID: 796
		private LinkedList<LookupEntry> lookupEntries;

		// Token: 0x0400031D RID: 797
		private BuildItemGroup projectItems;

		// Token: 0x0400031E RID: 798
		private Dictionary<BuildItem, BuildItem> cloneTable;

		// Token: 0x0400031F RID: 799
		private ReadOnlyLookup readOnlyLookup;

		// Token: 0x04000320 RID: 800
		private ItemDefinitionLibrary itemDefinitionLibrary;

		// Token: 0x0200009C RID: 156
		private enum ModifyMergeType
		{
			// Token: 0x04000322 RID: 802
			FirstWins = 1,
			// Token: 0x04000323 RID: 803
			SecondWins
		}
	}
}
