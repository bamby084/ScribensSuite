using System;
using System.Collections.Generic;


namespace ScribensMSWord.ExtensionMethods
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

    public class ParagraphInfo: IEquatable<ParagraphInfo>
    {
        public int Index { get; set; } 

        public string Text { get; set; }

        public bool Equals(ParagraphInfo other)
        {
            if (other == null || Text.IsNull())
                return false;

            return Index == other.Index && Text.EqualsIgnoreCase(other.Text);
        }

        public override bool Equals(object obj)
        {
            return Equals((ParagraphInfo)obj);
        }

        public override int GetHashCode()
        {
            if (Text.IsNull())
                return Index.GetHashCode();

            return Index.GetHashCode() ^ Text.ToLower().GetHashCode();
        }
    }
}
