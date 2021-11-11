using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Build.Shared
{
	// Token: 0x020000E9 RID: 233
	internal static class AssemblyResources
	{
		// Token: 0x060009D2 RID: 2514 RVA: 0x00032301 File Offset: 0x00031301
		internal static void RegisterMSBuildExeResources(ResourceManager manager)
		{
			ErrorUtilities.VerifyThrow(AssemblyResources.msbuildExeResourceManager == null, "Only one extra resource manager");
			AssemblyResources.msbuildExeResourceManager = manager;
		}

		// Token: 0x060009D3 RID: 2515 RVA: 0x0003231C File Offset: 0x0003131C
		internal static string GetString(string name)
		{
			string text = AssemblyResources.GetStringFromEngineResources(name);
			if (text == null)
			{
				text = AssemblyResources.GetStringFromMSBuildExeResources(name);
			}
			return text;
		}

		// Token: 0x060009D4 RID: 2516 RVA: 0x0003233C File Offset: 0x0003133C
		internal static string GetStringLookingInMSBuildExeResourcesFirst(string name)
		{
			string text = AssemblyResources.GetStringFromMSBuildExeResources(name);
			if (text == null)
			{
				text = AssemblyResources.GetStringFromEngineResources(name);
			}
			return text;
		}

		// Token: 0x060009D5 RID: 2517 RVA: 0x0003235C File Offset: 0x0003135C
		private static string GetStringFromEngineResources(string name)
		{
			string @string = AssemblyResources.resources.GetString(name, CultureInfo.CurrentUICulture);
			if (@string == null)
			{
				@string = AssemblyResources.sharedResources.GetString(name, CultureInfo.CurrentUICulture);
			}
			return @string;
		}

		// Token: 0x060009D6 RID: 2518 RVA: 0x00032390 File Offset: 0x00031390
		private static string GetStringFromMSBuildExeResources(string name)
		{
			string result = null;
			if (AssemblyResources.msbuildExeResourceManager != null)
			{
				result = AssemblyResources.msbuildExeResourceManager.GetString(name, CultureInfo.CurrentUICulture);
			}
			return result;
		}

		// Token: 0x040004EE RID: 1262
		private static ResourceManager msbuildExeResourceManager;

		// Token: 0x040004EF RID: 1263
		private static readonly ResourceManager resources = new ResourceManager("Microsoft.Build.Engine.Resources.Strings", Assembly.GetExecutingAssembly());

		// Token: 0x040004F0 RID: 1264
		private static readonly ResourceManager sharedResources = new ResourceManager("Microsoft.Build.Engine.Resources.Strings.shared", Assembly.GetExecutingAssembly());
	}
}
 
