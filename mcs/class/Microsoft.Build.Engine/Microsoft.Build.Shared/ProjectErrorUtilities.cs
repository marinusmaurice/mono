using System;
using System.Xml;
using Microsoft.Build.BuildEngine;

namespace Microsoft.Build.Shared
{
	// Token: 0x0200010D RID: 269
	internal static class ProjectErrorUtilities
	{
		// Token: 0x06000B10 RID: 2832 RVA: 0x0003A175 File Offset: 0x00039175
		internal static void VerifyThrowInvalidProject(bool condition, XmlNode xmlNode, string resourceName)
		{
			ProjectErrorUtilities.VerifyThrowInvalidProject(condition, null, xmlNode, resourceName);
		}

		// Token: 0x06000B11 RID: 2833 RVA: 0x0003A180 File Offset: 0x00039180
		internal static void ThrowInvalidProject(XmlNode xmlNode, string resourceName, object arg0)
		{
			ProjectErrorUtilities.VerifyThrowInvalidProject(false, null, xmlNode, resourceName, arg0);
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x0003A18C File Offset: 0x0003918C
		internal static void VerifyThrowInvalidProject(bool condition, XmlNode xmlNode, string resourceName, object arg0)
		{
			ProjectErrorUtilities.VerifyThrowInvalidProject(condition, null, xmlNode, resourceName, arg0);
		}

		// Token: 0x06000B13 RID: 2835 RVA: 0x0003A198 File Offset: 0x00039198
		internal static void ThrowInvalidProject(XmlNode xmlNode, string resourceName, object arg0, object arg1)
		{
			ProjectErrorUtilities.VerifyThrowInvalidProject(false, null, xmlNode, resourceName, arg0, arg1);
		}

		// Token: 0x06000B14 RID: 2836 RVA: 0x0003A1A5 File Offset: 0x000391A5
		internal static void VerifyThrowInvalidProject(bool condition, XmlNode xmlNode, string resourceName, object arg0, object arg1)
		{
			ProjectErrorUtilities.VerifyThrowInvalidProject(condition, null, xmlNode, resourceName, arg0, arg1);
		}

		// Token: 0x06000B15 RID: 2837 RVA: 0x0003A1B3 File Offset: 0x000391B3
		internal static void VerifyThrowInvalidProject(bool condition, XmlNode xmlNode, string resourceName, object arg0, object arg1, object arg2)
		{
			ProjectErrorUtilities.VerifyThrowInvalidProject(condition, null, xmlNode, resourceName, arg0, arg1, arg2);
		}

		// Token: 0x06000B16 RID: 2838 RVA: 0x0003A1C3 File Offset: 0x000391C3
		internal static void VerifyThrowInvalidProject(bool condition, XmlNode xmlNode, string resourceName, object arg0, object arg1, object arg2, object arg3)
		{
			ProjectErrorUtilities.VerifyThrowInvalidProject(condition, null, xmlNode, resourceName, arg0, arg1, arg2, arg3);
		}

		// Token: 0x06000B17 RID: 2839 RVA: 0x0003A1D5 File Offset: 0x000391D5
		internal static void VerifyThrowInvalidProject(bool condition, string errorSubCategoryResourceName, XmlNode xmlNode, string resourceName)
		{
			if (!condition)
			{
				ProjectErrorUtilities.ThrowInvalidProject(errorSubCategoryResourceName, xmlNode, resourceName, null);
			}
		}

		// Token: 0x06000B18 RID: 2840 RVA: 0x0003A1E4 File Offset: 0x000391E4
		internal static void VerifyThrowInvalidProject(bool condition, string errorSubCategoryResourceName, XmlNode xmlNode, string resourceName, object arg0)
		{
			if (!condition)
			{
				ProjectErrorUtilities.ThrowInvalidProject(errorSubCategoryResourceName, xmlNode, resourceName, new object[]
				{
					arg0
				});
			}
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x0003A20C File Offset: 0x0003920C
		internal static void VerifyThrowInvalidProject(bool condition, string errorSubCategoryResourceName, XmlNode xmlNode, string resourceName, object arg0, object arg1)
		{
			if (!condition)
			{
				ProjectErrorUtilities.ThrowInvalidProject(errorSubCategoryResourceName, xmlNode, resourceName, new object[]
				{
					arg0,
					arg1
				});
			}
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x0003A238 File Offset: 0x00039238
		internal static void VerifyThrowInvalidProject(bool condition, string errorSubCategoryResourceName, XmlNode xmlNode, string resourceName, object arg0, object arg1, object arg2)
		{
			if (!condition)
			{
				ProjectErrorUtilities.ThrowInvalidProject(errorSubCategoryResourceName, xmlNode, resourceName, new object[]
				{
					arg0,
					arg1,
					arg2
				});
			}
		}

		// Token: 0x06000B1B RID: 2843 RVA: 0x0003A268 File Offset: 0x00039268
		internal static void VerifyThrowInvalidProject(bool condition, string errorSubCategoryResourceName, XmlNode xmlNode, string resourceName, object arg0, object arg1, object arg2, object arg3)
		{
			if (!condition)
			{
				ProjectErrorUtilities.ThrowInvalidProject(errorSubCategoryResourceName, xmlNode, resourceName, new object[]
				{
					arg0,
					arg1,
					arg2,
					arg3
				});
			}
		}

		// Token: 0x06000B1C RID: 2844 RVA: 0x0003A29C File Offset: 0x0003929C
		private static void ThrowInvalidProject(string errorSubCategoryResourceName, XmlNode xmlNode, string resourceName, params object[] args)
		{
			string errorSubcategory = null;
			if (errorSubCategoryResourceName != null)
			{
				errorSubcategory = AssemblyResources.GetString(errorSubCategoryResourceName);
			}
			string errorCode;
			string helpKeyword;
			string message = ResourceUtilities.FormatResourceString(out errorCode, out helpKeyword, resourceName, args);
			throw new InvalidProjectFileException(xmlNode, message, errorSubcategory, errorCode, helpKeyword);
		}
	}
}
