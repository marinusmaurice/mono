using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Build.Shared
{
	// Token: 0x02000113 RID: 275
	internal static class ResourceUtilities
	{
		// Token: 0x06000B45 RID: 2885 RVA: 0x0003A9E8 File Offset: 0x000399E8
		internal static string ExtractMessageCode(Regex messageCodePattern, string messageWithCode, out string code)
		{
			code = null;
			string result = messageWithCode;
			if (messageCodePattern == null)
			{
				messageCodePattern = ResourceUtilities.msbuildMessageCodePattern;
			}
			Match match = messageCodePattern.Match(messageWithCode);
			if (match.Success)
			{
				code = match.Groups["CODE"].Value;
				result = match.Groups["MESSAGE"].Value;
			}
			return result;
		}

		// Token: 0x06000B46 RID: 2886 RVA: 0x0003AA42 File Offset: 0x00039A42
		private static string GetHelpKeyword(string resourceName)
		{
			return "MSBuild." + resourceName;
		}

		// Token: 0x06000B47 RID: 2887 RVA: 0x0003AA4F File Offset: 0x00039A4F
		internal static string FormatResourceString(out string code, out string helpKeyword, string resourceName, params object[] args)
		{
			helpKeyword = ResourceUtilities.GetHelpKeyword(resourceName);
			return ResourceUtilities.ExtractMessageCode(null, ResourceUtilities.FormatString(AssemblyResources.GetString(resourceName), args), out code);
		}

		// Token: 0x06000B48 RID: 2888 RVA: 0x0003AA6C File Offset: 0x00039A6C
		internal static string FormatResourceString(string resourceName, params object[] args)
		{
			string text;
			string text2;
			return ResourceUtilities.FormatResourceString(out text, out text2, resourceName, args);
		}

		// Token: 0x06000B49 RID: 2889 RVA: 0x0003AA84 File Offset: 0x00039A84
		internal static string FormatString(string unformatted, params object[] args)
		{
			string result = unformatted;
			if (args != null && args.Length > 0)
			{
				result = string.Format(CultureInfo.CurrentCulture, unformatted, args);
			}
			return result;
		}

		// Token: 0x040005BB RID: 1467
		private static readonly Regex msbuildMessageCodePattern = new Regex("^\\s*(?<CODE>MSB\\d\\d\\d\\d):\\s*(?<MESSAGE>.*)$", RegexOptions.Singleline);
	}
}
 
