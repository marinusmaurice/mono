using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Build.Shared
{
	// Token: 0x020000FD RID: 253
	internal static class EscapingUtilities
	{
		// Token: 0x06000AA3 RID: 2723 RVA: 0x00037BB4 File Offset: 0x00036BB4
		internal static string UnescapeAll(string escapedString)
		{
			bool flag;
			return EscapingUtilities.UnescapeAll(escapedString, out flag);
		}

		// Token: 0x06000AA4 RID: 2724 RVA: 0x00037BCC File Offset: 0x00036BCC
		internal static string UnescapeAll(string escapedString, out bool escapingWasNecessary)
		{
			ErrorUtilities.VerifyThrow(escapedString != null, "Null strings not allowed.");
			escapingWasNecessary = false;
			int num = escapedString.IndexOf('%');
			if (num == -1)
			{
				return escapedString;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = 0;
			while (num != -1)
			{
				if (num <= escapedString.Length - 3 && Uri.IsHexDigit(escapedString[num + 1]) && Uri.IsHexDigit(escapedString[num + 2]))
				{
					stringBuilder.Append(escapedString, num2, num - num2);
					string s = escapedString.Substring(num + 1, 2);
					char value = (char)int.Parse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					stringBuilder.Append(value);
					num2 = num + 3;
					escapingWasNecessary = true;
				}
				num = escapedString.IndexOf('%', num + 1);
			}
			stringBuilder.Append(escapedString, num2, escapedString.Length - num2);
			return stringBuilder.ToString();
		}

		// Token: 0x06000AA5 RID: 2725 RVA: 0x00037C94 File Offset: 0x00036C94
		internal static string Escape(string unescapedString)
		{
			ErrorUtilities.VerifyThrow(unescapedString != null, "Null strings not allowed.");
			if (!EscapingUtilities.ContainsReservedCharacters(unescapedString))
			{
				return unescapedString;
			}
			StringBuilder stringBuilder = new StringBuilder(unescapedString);
			foreach (char value in EscapingUtilities.charsToEscape)
			{
				int num = Convert.ToInt32(value);
				string newValue = string.Format(CultureInfo.InvariantCulture, "%{0:x00}", new object[]
				{
					num
				});
				stringBuilder.Replace(value.ToString(CultureInfo.InvariantCulture), newValue);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000AA6 RID: 2726 RVA: 0x00037D28 File Offset: 0x00036D28
		private static bool ContainsReservedCharacters(string unescapedString)
		{
			return -1 != unescapedString.IndexOfAny(EscapingUtilities.charsToEscape);
		}

		// Token: 0x06000AA7 RID: 2727 RVA: 0x00037D3C File Offset: 0x00036D3C
		internal static bool ContainsEscapedWildcards(string escapedString)
		{
			return -1 != escapedString.IndexOf('%') && (-1 != escapedString.IndexOf("%2", StringComparison.Ordinal) || -1 != escapedString.IndexOf("%3", StringComparison.Ordinal)) && (-1 != escapedString.IndexOf("%2a", StringComparison.Ordinal) || -1 != escapedString.IndexOf("%2A", StringComparison.Ordinal) || -1 != escapedString.IndexOf("%3f", StringComparison.Ordinal) || -1 != escapedString.IndexOf("%3F", StringComparison.Ordinal));
		}

		// Token: 0x04000535 RID: 1333
		private static char[] charsToEscape = new char[]
		{
			'%',
			'*',
			'?',
			'@',
			'$',
			'(',
			')',
			';',
			'\''
		};
	}
}
 
