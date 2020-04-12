using System;
using PluginScribens.Common.ExtensionMethods;

namespace PluginScribens.Common
{
    public class ParagraphInfo : IEquatable<ParagraphInfo>
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
