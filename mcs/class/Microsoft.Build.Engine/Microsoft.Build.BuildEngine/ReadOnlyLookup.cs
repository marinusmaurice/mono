using System;
using System.Collections;

namespace Microsoft.Build.BuildEngine
{
	// Token: 0x0200009D RID: 157
	internal class ReadOnlyLookup
	{
		// Token: 0x06000651 RID: 1617 RVA: 0x00022BE8 File Offset: 0x00021BE8
		internal ReadOnlyLookup(Lookup lookup)
		{
			this.lookup = lookup;
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x00022BF7 File Offset: 0x00021BF7
		internal ReadOnlyLookup(Hashtable items, BuildPropertyGroup properties)
		{
			this.lookup = new Lookup(items, properties, new ItemDefinitionLibrary(null));
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x00022C12 File Offset: 0x00021C12
		internal BuildItemGroup GetItems(string name)
		{
			return this.lookup.GetItems(name);
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x00022C20 File Offset: 0x00021C20
		internal BuildProperty GetProperty(string name)
		{
			return this.lookup.GetProperty(name);
		}

		// Token: 0x04000324 RID: 804
		private Lookup lookup;
	}
}
