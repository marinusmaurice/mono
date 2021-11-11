using System;
using System.Diagnostics;

namespace Microsoft.Build.Shared
{
	// Token: 0x020000FC RID: 252
	internal static class ErrorUtilities
	{
		// Token: 0x06000A88 RID: 2696 RVA: 0x000378A4 File Offset: 0x000368A4
		internal static void LaunchMsBuildDebuggerOnFatalError()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("MSBuildLaunchDebuggerOnFatalError");
			if (!string.IsNullOrEmpty(environmentVariable))
			{
				Debugger.Launch();
			}
		}

		// Token: 0x06000A89 RID: 2697 RVA: 0x000378CA File Offset: 0x000368CA
		private static void ThrowInternalError(bool showAssert, string unformattedMessage, params object[] args)
		{
			throw new InternalErrorException(ResourceUtilities.FormatString(unformattedMessage, args), showAssert);
		}

		// Token: 0x06000A8A RID: 2698 RVA: 0x000378D9 File Offset: 0x000368D9
		internal static void ThrowInternalError(string message)
		{
			throw new InternalErrorException(message);
		}

		// Token: 0x06000A8B RID: 2699 RVA: 0x000378E1 File Offset: 0x000368E1
		internal static void VerifyThrowNoAssert(bool condition, string unformattedMessage)
		{
			if (!condition)
			{
				ErrorUtilities.ThrowInternalError(false, unformattedMessage, null);
			}
		}

		// Token: 0x06000A8C RID: 2700 RVA: 0x000378EE File Offset: 0x000368EE
		internal static void VerifyThrow(bool condition, string unformattedMessage)
		{
			if (!condition)
			{
				ErrorUtilities.ThrowInternalError(true, unformattedMessage, null);
			}
		}

		// Token: 0x06000A8D RID: 2701 RVA: 0x000378FC File Offset: 0x000368FC
		internal static void VerifyThrow(bool condition, string unformattedMessage, object arg0)
		{
			if (!condition)
			{
				ErrorUtilities.ThrowInternalError(true, unformattedMessage, new object[]
				{
					arg0
				});
			}
		}

		// Token: 0x06000A8E RID: 2702 RVA: 0x00037920 File Offset: 0x00036920
		internal static void VerifyThrowNoAssert(bool condition, string unformattedMessage, object arg0, object arg1)
		{
			if (!condition)
			{
				ErrorUtilities.ThrowInternalError(false, unformattedMessage, new object[]
				{
					arg0,
					arg1
				});
			}
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x00037948 File Offset: 0x00036948
		internal static void VerifyThrow(bool condition, string unformattedMessage, object arg0, object arg1)
		{
			if (!condition)
			{
				ErrorUtilities.ThrowInternalError(true, unformattedMessage, new object[]
				{
					arg0,
					arg1
				});
			}
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x00037970 File Offset: 0x00036970
		internal static void VerifyThrow(bool condition, string unformattedMessage, object arg0, object arg1, object arg2)
		{
			if (!condition)
			{
				ErrorUtilities.ThrowInternalError(true, unformattedMessage, new object[]
				{
					arg0,
					arg1,
					arg2
				});
			}
		}

		// Token: 0x06000A91 RID: 2705 RVA: 0x0003799C File Offset: 0x0003699C
		internal static void VerifyThrow(bool condition, string unformattedMessage, object arg0, object arg1, object arg2, object arg3)
		{
			if (!condition)
			{
				ErrorUtilities.ThrowInternalError(true, unformattedMessage, new object[]
				{
					arg0,
					arg1,
					arg2,
					arg3
				});
			}
		}

		// Token: 0x06000A92 RID: 2706 RVA: 0x000379CD File Offset: 0x000369CD
		private static void ThrowInvalidOperation(string resourceName, params object[] args)
		{
			throw new InvalidOperationException(ResourceUtilities.FormatResourceString(resourceName, args));
		}

		// Token: 0x06000A93 RID: 2707 RVA: 0x000379DB File Offset: 0x000369DB
		internal static void VerifyThrowInvalidOperation(bool condition, string resourceName)
		{
			if (!condition)
			{
				ErrorUtilities.ThrowInvalidOperation(resourceName, null);
			}
		}

		// Token: 0x06000A94 RID: 2708 RVA: 0x000379E8 File Offset: 0x000369E8
		internal static void VerifyThrowInvalidOperation(bool condition, string resourceName, object arg0)
		{
			if (!condition)
			{
				ErrorUtilities.ThrowInvalidOperation(resourceName, new object[]
				{
					arg0
				});
			}
		}

		// Token: 0x06000A95 RID: 2709 RVA: 0x00037A0C File Offset: 0x00036A0C
		internal static void VerifyThrowInvalidOperation(bool condition, string resourceName, object arg0, object arg1)
		{
			if (!condition)
			{
				ErrorUtilities.ThrowInvalidOperation(resourceName, new object[]
				{
					arg0,
					arg1
				});
			}
		}

		// Token: 0x06000A96 RID: 2710 RVA: 0x00037A34 File Offset: 0x00036A34
		internal static void VerifyThrowInvalidOperation(bool condition, string resourceName, object arg0, object arg1, object arg2)
		{
			if (!condition)
			{
				ErrorUtilities.ThrowInvalidOperation(resourceName, new object[]
				{
					arg0,
					arg1,
					arg2
				});
			}
		}

		// Token: 0x06000A97 RID: 2711 RVA: 0x00037A5F File Offset: 0x00036A5F
		private static void ThrowArgument(Exception innerException, string resourceName, params object[] args)
		{
			throw new ArgumentException(ResourceUtilities.FormatResourceString(resourceName, args), innerException);
		}

		// Token: 0x06000A98 RID: 2712 RVA: 0x00037A6E File Offset: 0x00036A6E
		internal static void VerifyThrowArgument(bool condition, string resourceName)
		{
			ErrorUtilities.VerifyThrowArgument(condition, null, resourceName);
		}

		// Token: 0x06000A99 RID: 2713 RVA: 0x00037A78 File Offset: 0x00036A78
		internal static void VerifyThrowArgument(bool condition, string resourceName, object arg0)
		{
			ErrorUtilities.VerifyThrowArgument(condition, null, resourceName, arg0);
		}

		// Token: 0x06000A9A RID: 2714 RVA: 0x00037A83 File Offset: 0x00036A83
		internal static void VerifyThrowArgument(bool condition, string resourceName, object arg0, object arg1)
		{
			ErrorUtilities.VerifyThrowArgument(condition, null, resourceName, arg0, arg1);
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x00037A8F File Offset: 0x00036A8F
		internal static void VerifyThrowArgument(bool condition, Exception innerException, string resourceName)
		{
			if (!condition)
			{
				ErrorUtilities.ThrowArgument(innerException, resourceName, null);
			}
		}

		// Token: 0x06000A9C RID: 2716 RVA: 0x00037A9C File Offset: 0x00036A9C
		internal static void VerifyThrowArgument(bool condition, Exception innerException, string resourceName, object arg0)
		{
			if (!condition)
			{
				ErrorUtilities.ThrowArgument(innerException, resourceName, new object[]
				{
					arg0
				});
			}
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x00037AC0 File Offset: 0x00036AC0
		internal static void VerifyThrowArgument(bool condition, Exception innerException, string resourceName, object arg0, object arg1)
		{
			if (!condition)
			{
				ErrorUtilities.ThrowArgument(innerException, resourceName, new object[]
				{
					arg0,
					arg1
				});
			}
		}

		// Token: 0x06000A9E RID: 2718 RVA: 0x00037AE8 File Offset: 0x00036AE8
		internal static void VerifyThrowArgumentOutOfRange(bool condition, string parameterName)
		{
			if (!condition)
			{
				throw new ArgumentOutOfRangeException(parameterName);
			}
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x00037AF4 File Offset: 0x00036AF4
		internal static void VerifyThrowArgumentLength(string parameter, string parameterName)
		{
			ErrorUtilities.VerifyThrowArgumentNull(parameter, parameterName);
			if (parameter.Length == 0)
			{
				throw new ArgumentException(ResourceUtilities.FormatResourceString("Shared.ParameterCannotHaveZeroLength", new object[]
				{
					parameterName
				}));
			}
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x00037B2C File Offset: 0x00036B2C
		internal static void VerifyThrowArgumentNull(object parameter, string parameterName)
		{
			ErrorUtilities.VerifyThrowArgumentNull(parameter, parameterName, "Shared.ParameterCannotBeNull");
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x00037B3C File Offset: 0x00036B3C
		internal static void VerifyThrowArgumentNull(object parameter, string parameterName, string resourceName)
		{
			if (parameter == null)
			{
				throw new System.ArgumentNullException((string)ResourceUtilities.FormatResourceString(resourceName, new object[]
				{
					parameterName
				}), (Exception)null);
			}
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x00037B68 File Offset: 0x00036B68
		internal static void VerifyThrowArgumentArraysSameLength(Array parameter1, Array parameter2, string parameter1Name, string parameter2Name)
		{
			ErrorUtilities.VerifyThrowArgumentNull(parameter1, parameter1Name);
			ErrorUtilities.VerifyThrowArgumentNull(parameter2, parameter2Name);
			if (parameter1.Length != parameter2.Length)
			{
				throw new ArgumentException(ResourceUtilities.FormatResourceString("Shared.ParametersMustHaveTheSameLength", new object[]
				{
					parameter1Name,
					parameter2Name
				}));
			}
		}
	}
}
 
