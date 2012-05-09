﻿using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MyToolkit.Utilities
{
	public static class StringExtensions
	{
		public static string RemoveHtmlTags(this string text)
		{
			return Regex.Replace(text, "<[^>]*>", string.Empty);
		}

		public static string RemoveHtmlLinks(this string text)
		{
			return Regex.Replace(text, "<a[^>]*>[^<]*</a>", string.Empty);
		}

		public static string ConvertUTF8Characters(this string text)
		{
			return text.Replace("\u008B", "‹").Replace("\u009B", "›");
		}

		public static string TruncateWithoutChopping(this string text, int length)
		{
			if (text == null || text.Length < length)
				return text;

			var index = text.LastIndexOf(" ", length, StringComparison.Ordinal);
			return string.Format("{0}...", text.Substring(0, (index > 0) ? index : text.Length).Trim());
		}
	}
}
