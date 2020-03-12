using Microsoft.Office.Interop.Word;

namespace ScribensMSWord.ExtensionMethods
{
    public static class RangeExtensions
    {
        public static bool IsValid(this Range range)
        {
            return range != null && !range.Text.IsNull();
        }
    }
}
