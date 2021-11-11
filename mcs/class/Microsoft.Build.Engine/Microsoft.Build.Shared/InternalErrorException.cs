using System;
using System.Runtime.Serialization;

namespace Microsoft.Build.Shared
{
	// Token: 0x02000109 RID: 265
	[Serializable]
	internal sealed class InternalErrorException : Exception
	{
		// Token: 0x06000AF3 RID: 2803 RVA: 0x00039E46 File Offset: 0x00038E46
		internal InternalErrorException()
		{
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x00039E4E File Offset: 0x00038E4E
		internal InternalErrorException(string message) : base("Internal MSBuild Error: " + message)
		{
			this.ShowAssertDialog(true);
		}

		// Token: 0x06000AF5 RID: 2805 RVA: 0x00039E68 File Offset: 0x00038E68
		internal InternalErrorException(string message, bool showAssert) : base("Internal MSBuild Error: " + message)
		{
			this.ShowAssertDialog(showAssert);
		}

		// Token: 0x06000AF6 RID: 2806 RVA: 0x00039E82 File Offset: 0x00038E82
		internal InternalErrorException(string message, Exception innerException) : base("Internal MSBuild Error: " + message, innerException)
		{
		}

		// Token: 0x06000AF7 RID: 2807 RVA: 0x00039E96 File Offset: 0x00038E96
		private InternalErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000AF8 RID: 2808 RVA: 0x00039EA0 File Offset: 0x00038EA0
		private void ShowAssertDialog(bool showAssert)
		{
		}
	}
}
 
