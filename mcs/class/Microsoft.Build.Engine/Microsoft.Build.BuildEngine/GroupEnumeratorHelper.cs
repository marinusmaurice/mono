using System;
using System.Collections;
using Microsoft.Build.Shared;

namespace Microsoft.Build.BuildEngine
{
	// Token: 0x0200000A RID: 10
	internal sealed class GroupEnumeratorHelper : IEnumerable
	{
		// Token: 0x06000025 RID: 37 RVA: 0x0000287C File Offset: 0x0000187C
		internal GroupEnumeratorHelper(GroupingCollection groupingCollection, GroupEnumeratorHelper.ListType type)
		{
			ErrorUtilities.VerifyThrow(groupingCollection != null, "GroupingCollection is null");
			this.groupingCollection = groupingCollection;
			this.type = type;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002FC0 File Offset: 0x00001FC0
		public IEnumerator GetEnumerator()
		{
			foreach (object obj in this.groupingCollection)
			{
				IItemPropertyGrouping group = (IItemPropertyGrouping)obj;
				if (group is BuildItemGroup && (this.type == GroupEnumeratorHelper.ListType.ItemGroupsTopLevel || this.type == GroupEnumeratorHelper.ListType.ItemGroupsTopLevelAndChoose || this.type == GroupEnumeratorHelper.ListType.ItemGroupsAll))
				{
					yield return group;
				}
				else if (group is BuildPropertyGroup && (this.type == GroupEnumeratorHelper.ListType.PropertyGroupsTopLevel || this.type == GroupEnumeratorHelper.ListType.PropertyGroupsTopLevelAndChoose || this.type == GroupEnumeratorHelper.ListType.PropertyGroupsAll))
				{
					yield return group;
				}
				else if (group is Choose)
				{
					if (this.type == GroupEnumeratorHelper.ListType.ChoosesTopLevel || this.type == GroupEnumeratorHelper.ListType.ItemGroupsTopLevelAndChoose || this.type == GroupEnumeratorHelper.ListType.PropertyGroupsTopLevelAndChoose)
					{
						yield return group;
					}
					else if (this.type == GroupEnumeratorHelper.ListType.ItemGroupsAll || this.type == GroupEnumeratorHelper.ListType.PropertyGroupsAll)
					{
						Choose choose = (Choose)group;
						foreach (object obj2 in choose.Whens)
						{
							When when = (When)obj2;
							if (this.type == GroupEnumeratorHelper.ListType.ItemGroupsAll)
							{
								foreach (object obj3 in when.PropertyAndItemLists.ItemGroupsAll)
								{
									IItemPropertyGrouping nestedGroup = (IItemPropertyGrouping)obj3;
									yield return nestedGroup;
								}
							}
							else if (this.type == GroupEnumeratorHelper.ListType.PropertyGroupsAll)
							{
								foreach (object obj4 in when.PropertyAndItemLists.PropertyGroupsAll)
								{
									IItemPropertyGrouping nestedGroup2 = (IItemPropertyGrouping)obj4;
									yield return nestedGroup2;
								}
							}
						}
						if (choose.Otherwise != null)
						{
							if (this.type == GroupEnumeratorHelper.ListType.ItemGroupsAll)
							{
								foreach (object obj5 in choose.Otherwise.PropertyAndItemLists.ItemGroupsAll)
								{
									IItemPropertyGrouping nestedGroup3 = (IItemPropertyGrouping)obj5;
									yield return nestedGroup3;
								}
							}
							else if (this.type == GroupEnumeratorHelper.ListType.PropertyGroupsAll)
							{
								foreach (object obj6 in choose.Otherwise.PropertyAndItemLists.PropertyGroupsAll)
								{
									IItemPropertyGrouping nestedGroup4 = (IItemPropertyGrouping)obj6;
									yield return nestedGroup4;
								}
							}
						}
					}
				}
			}
			yield break;
		}

		// Token: 0x0400000E RID: 14
		private GroupingCollection groupingCollection;

		// Token: 0x0400000F RID: 15
		private GroupEnumeratorHelper.ListType type;

		// Token: 0x0200000B RID: 11
		internal enum ListType
		{
			// Token: 0x04000011 RID: 17
			PropertyGroupsTopLevelAndChoose,
			// Token: 0x04000012 RID: 18
			ItemGroupsTopLevelAndChoose,
			// Token: 0x04000013 RID: 19
			PropertyGroupsTopLevel,
			// Token: 0x04000014 RID: 20
			ItemGroupsTopLevel,
			// Token: 0x04000015 RID: 21
			PropertyGroupsAll,
			// Token: 0x04000016 RID: 22
			ItemGroupsAll,
			// Token: 0x04000017 RID: 23
			ChoosesTopLevel
		}
	}
}
