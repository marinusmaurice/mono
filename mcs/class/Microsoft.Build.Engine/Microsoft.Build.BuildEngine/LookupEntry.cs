using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Build.BuildEngine
{
	// Token: 0x0200009E RID: 158
	internal class LookupEntry
	{
		// Token: 0x06000655 RID: 1621 RVA: 0x00022C30 File Offset: 0x00021C30
		internal LookupEntry(Hashtable items, BuildPropertyGroup properties)
		{
			this.items = items;
			this.adds = null;
			this.removes = null;
			this.modifies = null;
			this.properties = properties;
			this.propertySets = null;
			this.threadIdThatEnteredScope = Thread.CurrentThread.ManagedThreadId;
			this.truncateLookupsAtThisScope = false;
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000656 RID: 1622 RVA: 0x00022C84 File Offset: 0x00021C84
		// (set) Token: 0x06000657 RID: 1623 RVA: 0x00022C8C File Offset: 0x00021C8C
		internal Hashtable Items
		{
			get
			{
				return this.items;
			}
			set
			{
				this.items = value;
			}
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06000658 RID: 1624 RVA: 0x00022C95 File Offset: 0x00021C95
		// (set) Token: 0x06000659 RID: 1625 RVA: 0x00022C9D File Offset: 0x00021C9D
		internal Hashtable Adds
		{
			get
			{
				return this.adds;
			}
			set
			{
				this.adds = value;
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x0600065A RID: 1626 RVA: 0x00022CA6 File Offset: 0x00021CA6
		// (set) Token: 0x0600065B RID: 1627 RVA: 0x00022CAE File Offset: 0x00021CAE
		internal Hashtable Removes
		{
			get
			{
				return this.removes;
			}
			set
			{
				this.removes = value;
			}
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x0600065C RID: 1628 RVA: 0x00022CB7 File Offset: 0x00021CB7
		// (set) Token: 0x0600065D RID: 1629 RVA: 0x00022CBF File Offset: 0x00021CBF
		internal Dictionary<string, Dictionary<BuildItem, Dictionary<string, string>>> Modifies
		{
			get
			{
				return this.modifies;
			}
			set
			{
				this.modifies = value;
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x0600065E RID: 1630 RVA: 0x00022CC8 File Offset: 0x00021CC8
		// (set) Token: 0x0600065F RID: 1631 RVA: 0x00022CD0 File Offset: 0x00021CD0
		internal BuildPropertyGroup Properties
		{
			get
			{
				return this.properties;
			}
			set
			{
				this.properties = value;
			}
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x06000660 RID: 1632 RVA: 0x00022CD9 File Offset: 0x00021CD9
		// (set) Token: 0x06000661 RID: 1633 RVA: 0x00022CE1 File Offset: 0x00021CE1
		internal BuildPropertyGroup PropertySets
		{
			get
			{
				return this.propertySets;
			}
			set
			{
				this.propertySets = value;
			}
		}

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06000662 RID: 1634 RVA: 0x00022CEA File Offset: 0x00021CEA
		internal int ThreadIdThatEnteredScope
		{
			get
			{
				return this.threadIdThatEnteredScope;
			}
		}

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06000663 RID: 1635 RVA: 0x00022CF2 File Offset: 0x00021CF2
		// (set) Token: 0x06000664 RID: 1636 RVA: 0x00022CFA File Offset: 0x00021CFA
		internal bool TruncateLookupsAtThisScope
		{
			get
			{
				return this.truncateLookupsAtThisScope;
			}
			set
			{
				this.truncateLookupsAtThisScope = value;
			}
		}

		// Token: 0x04000325 RID: 805
		private Hashtable items;

		// Token: 0x04000326 RID: 806
		private Hashtable adds;

		// Token: 0x04000327 RID: 807
		private Hashtable removes;

		// Token: 0x04000328 RID: 808
		private Dictionary<string, Dictionary<BuildItem, Dictionary<string, string>>> modifies;

		// Token: 0x04000329 RID: 809
		private BuildPropertyGroup properties;

		// Token: 0x0400032A RID: 810
		private BuildPropertyGroup propertySets;

		// Token: 0x0400032B RID: 811
		private int threadIdThatEnteredScope;

		// Token: 0x0400032C RID: 812
		private bool truncateLookupsAtThisScope;
	}
}
