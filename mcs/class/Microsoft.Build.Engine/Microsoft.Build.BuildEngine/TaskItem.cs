using System;
using System.Collections;
using System.Security.Permissions;
using Microsoft.Build.Framework;
using Microsoft.Build.Shared;

namespace Microsoft.Build.BuildEngine
{
	// Token: 0x020000A7 RID: 167
	internal sealed class TaskItem : MarshalByRefObject, ITaskItem
	{
		// Token: 0x06000711 RID: 1809 RVA: 0x000262AD File Offset: 0x000252AD
		private TaskItem()
		{
		}

		// Token: 0x06000712 RID: 1810 RVA: 0x000262B5 File Offset: 0x000252B5
		internal TaskItem(string itemSpec)
		{
			ErrorUtilities.VerifyThrow(itemSpec != null, "Need to specify item-spec.");
			this.item = new BuildItem(null, itemSpec);
		}

		// Token: 0x06000713 RID: 1811 RVA: 0x000262DB File Offset: 0x000252DB
		internal TaskItem(BuildItem item)
		{
			ErrorUtilities.VerifyThrow(item != null, "Need to specify backing item.");
			this.item = item.VirtualClone();
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06000714 RID: 1812 RVA: 0x00026300 File Offset: 0x00025300
		// (set) Token: 0x06000715 RID: 1813 RVA: 0x0002630D File Offset: 0x0002530D
		public string ItemSpec
		{
			get
			{
				return this.item.FinalItemSpec;
			}
			set
			{
				ErrorUtilities.VerifyThrowArgumentNull(value, "ItemSpec");
				this.item.SetFinalItemSpecEscaped(EscapingUtilities.Escape(value));
			}
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06000716 RID: 1814 RVA: 0x0002632B File Offset: 0x0002532B
		public ICollection MetadataNames
		{
			get
			{
				return this.item.MetadataNames;
			}
		}

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06000717 RID: 1815 RVA: 0x00026338 File Offset: 0x00025338
		public int MetadataCount
		{
			get
			{
				return this.item.MetadataCount;
			}
		}

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06000718 RID: 1816 RVA: 0x00026345 File Offset: 0x00025345
		public ICollection CustomMetadataNames
		{
			get
			{
				return this.item.CustomMetadataNames;
			}
		}

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x06000719 RID: 1817 RVA: 0x00026352 File Offset: 0x00025352
		public int CustomMetadataCount
		{
			get
			{
				return this.item.CustomMetadataCount;
			}
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x0002635F File Offset: 0x0002535F
		public string GetMetadata(string metadataName)
		{
			ErrorUtilities.VerifyThrowArgumentNull(metadataName, "metadataName");
			return this.item.GetEvaluatedMetadata(metadataName);
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x00026378 File Offset: 0x00025378
		public void SetMetadata(string metadataName, string metadataValue)
		{
			ErrorUtilities.VerifyThrowArgumentLength(metadataName, "metadataName");
			ErrorUtilities.VerifyThrowArgumentNull(metadataValue, "metadataValue");
			this.item.SetMetadata(metadataName, EscapingUtilities.Escape(metadataValue));
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x000263A2 File Offset: 0x000253A2
		public void RemoveMetadata(string metadataName)
		{
			ErrorUtilities.VerifyThrowArgumentNull(metadataName, "metadataName");
			this.item.RemoveMetadata(metadataName);
		}

		// Token: 0x0600071D RID: 1821 RVA: 0x000263BC File Offset: 0x000253BC
		public void CopyMetadataTo(ITaskItem destinationItem)
		{
			ErrorUtilities.VerifyThrowArgumentNull(destinationItem, "destinationItem");
			foreach (object obj in this.item.GetAllCustomEvaluatedMetadata())
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				string metadataName = (string)dictionaryEntry.Key;
				string metadata = destinationItem.GetMetadata(metadataName);
				if (metadata == null || metadata.Length == 0)
				{
					destinationItem.SetMetadata(metadataName, EscapingUtilities.UnescapeAll((string)dictionaryEntry.Value));
				}
			}
			string metadata2 = destinationItem.GetMetadata("OriginalItemSpec");
			if (metadata2 == null || metadata2.Length == 0)
			{
				destinationItem.SetMetadata("OriginalItemSpec", this.ItemSpec);
			}
		}

		// Token: 0x0600071E RID: 1822 RVA: 0x00026488 File Offset: 0x00025488
		public IDictionary CloneCustomMetadata()
		{
			IDictionary dictionary = this.item.CloneCustomMetadata();
			string[] array = new string[dictionary.Count];
			dictionary.Keys.CopyTo(array, 0);
			foreach (string key in array)
			{
				string escapedString = (string)dictionary[key];
				bool flag;
				string value = EscapingUtilities.UnescapeAll(escapedString, out flag);
				if (flag)
				{
					dictionary[key] = value;
				}
			}
			return dictionary;
		}

		// Token: 0x0600071F RID: 1823 RVA: 0x000264FB File Offset: 0x000254FB
		public override string ToString()
		{
			return this.ItemSpec;
		}

		// Token: 0x06000720 RID: 1824 RVA: 0x00026503 File Offset: 0x00025503
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
		public override object InitializeLifetimeService()
		{
			return null;
		}

		// Token: 0x06000721 RID: 1825 RVA: 0x00026506 File Offset: 0x00025506
		public static explicit operator string(TaskItem taskItemToCast)
		{
			return taskItemToCast.ItemSpec;
		}

		// Token: 0x0400035F RID: 863
		internal BuildItem item;
	}
}
 
