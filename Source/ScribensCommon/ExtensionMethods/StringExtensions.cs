using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PluginScribens.Common.ExtensionMethods
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string source, string target)
        {
            if (source == null)
                return false;

            return source.Equals(target, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsNull(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        public static T ToEnum<T>(this string source)
            where T : Enum
        {
            if (string.IsNullOrEmpty(source))
                return default(T);

            return (T)Enum.Parse(typeof(T), source);
        }

        public static DateTime? ToDateTime(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            if (DateTime.TryParse(source, out var result))
                return result;

            return DateTime.MinValue;
        }

        public static string Encode(this string source)
        {
            int maxLengthAllowed = 65519;
            StringBuilder sb = new StringBuilder();
            int loops = source.Length / maxLengthAllowed;

            for (int i = 0; i <= loops; i++)
            {
                sb.Append(Uri.EscapeDataString(i < loops
                    ? source.Substring(maxLengthAllowed * i, maxLengthAllowed)
                    : source.Substring(maxLengthAllowed * i)));
            }

            return sb.ToString();
        }

        public static List<ParagraphInfo> ToParagraphs(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return new List<ParagraphInfo>();

            var paragraphs = Regex.Split(source, @"(?<=[\r])");
            var result = new List<ParagraphInfo>();

            for (int i = 0; i < paragraphs.Length; i++)
            {
                if (!paragraphs[i].IsNull())
                {
                    result.Add(new ParagraphInfo()
                    {
                        Index = i + 1, //MS Word uses 1-based index
                        Text = paragraphs[i]
                    });
                }
            }

            return result;
        }

        public static string RemoveSpecialCharacters(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            return source.Replace('\u2013', '-').Replace('\u2014', '-').Replace('\u2015', '-')
                .Replace('\u2017', '_')
                .Replace('\u2018', '\'').Replace('\u2019', '\'').Replace('\u201b', '\'').Replace('\u2032', '\'')
                .Replace('\u201a', ',')
                .Replace('\u201c', '\"').Replace('\u201d', '\"').Replace('\u201e', '\"').Replace('\u2033', '\"')
                .Replace("\u2026", "...")
                .Replace('\u00a0', ' ')
                .Replace('\v', '\r')
                .Replace("\a", null);
        }
    }
}
