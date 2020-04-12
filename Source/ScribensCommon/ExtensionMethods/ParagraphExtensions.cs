using System;
using System.Collections.Generic;


namespace PluginScribens.Common.ExtensionMethods
{
    public static class ParagraphExtensions
    {
        public static string Join(this IEnumerable<ParagraphInfo> paragraphs)
        {
            if (paragraphs == null)
                return null;

            string result = "";
            foreach(var paragraph in paragraphs)
            {
                var paragraphText = paragraph.Text;
                paragraphText = paragraphText.Replace("\r", "");
                paragraphText = paragraphText.Replace("\n", "");

                result += $"<p>{paragraphText}</p>";
            }

            return result;
        }
    }
}
