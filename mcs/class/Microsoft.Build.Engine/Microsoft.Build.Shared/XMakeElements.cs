using System;
using System.Collections;
using System.Xml;

namespace Microsoft.Build.Shared
{
	// Token: 0x02000117 RID: 279
	internal static class XMakeElements
	{
		// Token: 0x06000B79 RID: 2937 RVA: 0x0003C376 File Offset: 0x0003B376
		internal static bool IsValidTaskChildNode(XmlNode childNode)
		{
			return childNode.Name == "Output" || childNode.NodeType == XmlNodeType.Comment || childNode.NodeType == XmlNodeType.Whitespace;
		}

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06000B7A RID: 2938 RVA: 0x0003C3A0 File Offset: 0x0003B3A0
		internal static Hashtable IllegalItemPropertyNames
		{
			get
			{
				if (XMakeElements.illegalItemOrPropertyNamesHashtable == null)
				{
					XMakeElements.illegalItemOrPropertyNamesHashtable = new Hashtable(XMakeElements.illegalPropertyOrItemNames.Length);
					foreach (string key in XMakeElements.illegalPropertyOrItemNames)
					{
						XMakeElements.illegalItemOrPropertyNamesHashtable.Add(key, string.Empty);
					}
				}
				return XMakeElements.illegalItemOrPropertyNamesHashtable;
			}
		}

		// Token: 0x040005EF RID: 1519
		internal const string project = "Project";

		// Token: 0x040005F0 RID: 1520
		internal const string visualStudioProject = "VisualStudioProject";

		// Token: 0x040005F1 RID: 1521
		internal const string target = "Target";

		// Token: 0x040005F2 RID: 1522
		internal const string propertyGroup = "PropertyGroup";

		// Token: 0x040005F3 RID: 1523
		internal const string output = "Output";

		// Token: 0x040005F4 RID: 1524
		internal const string itemGroup = "ItemGroup";

		// Token: 0x040005F5 RID: 1525
		internal const string itemDefinitionGroup = "ItemDefinitionGroup";

		// Token: 0x040005F6 RID: 1526
		internal const string usingTask = "UsingTask";

		// Token: 0x040005F7 RID: 1527
		internal const string projectExtensions = "ProjectExtensions";

		// Token: 0x040005F8 RID: 1528
		internal const string onError = "OnError";

		// Token: 0x040005F9 RID: 1529
		internal const string error = "Error";

		// Token: 0x040005FA RID: 1530
		internal const string warning = "Warning";

		// Token: 0x040005FB RID: 1531
		internal const string message = "Message";

		// Token: 0x040005FC RID: 1532
		internal const string import = "Import";

		// Token: 0x040005FD RID: 1533
		internal const string choose = "Choose";

		// Token: 0x040005FE RID: 1534
		internal const string when = "When";

		// Token: 0x040005FF RID: 1535
		internal const string otherwise = "Otherwise";

		// Token: 0x04000600 RID: 1536
		internal static readonly char[] illegalTargetNameCharacters = new char[]
		{
			'$',
			'@',
			'(',
			')',
			'%',
			'*',
			'?',
			'.'
		};

		// Token: 0x04000601 RID: 1537
		internal static readonly string[] illegalPropertyOrItemNames = new string[]
		{
			"VisualStudioProject",
			"Target",
			"PropertyGroup",
			"Output",
			"ItemGroup",
			"UsingTask",
			"ProjectExtensions",
			"OnError",
			"Choose",
			"When",
			"Otherwise"
		};

		// Token: 0x04000602 RID: 1538
		private static Hashtable illegalItemOrPropertyNamesHashtable;
	}
}
