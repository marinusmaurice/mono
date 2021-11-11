using System;

namespace Microsoft.Build.BuildEngine
{
	// Token: 0x020000A5 RID: 165
	internal class SpecificItemDefinitionLibrary
	{
		// Token: 0x06000704 RID: 1796 RVA: 0x00025E5F File Offset: 0x00024E5F
		internal SpecificItemDefinitionLibrary(string itemType, ItemDefinitionLibrary itemDefinitionLibrary)
		{
			this.itemType = itemType;
			this.itemDefinitionLibrary = itemDefinitionLibrary;
		}

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06000705 RID: 1797 RVA: 0x00025E75 File Offset: 0x00024E75
		internal string ItemType
		{
			get
			{
				return this.itemType;
			}
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x00025E7D File Offset: 0x00024E7D
		internal string GetDefaultMetadataValue(string metadataName)
		{
			return this.itemDefinitionLibrary.GetDefaultMetadataValue(this.itemType, metadataName);
		}

		// Token: 0x0400034F RID: 847
		private string itemType;

		// Token: 0x04000350 RID: 848
		private ItemDefinitionLibrary itemDefinitionLibrary;
	}
}
