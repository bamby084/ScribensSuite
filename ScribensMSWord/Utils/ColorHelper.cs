using Microsoft.Office.Interop.Word;
using System.Windows.Media;

namespace ScribensMSWord.Utils
{
    public class ColorHelper
    {
        public static WdColor ConvertHexColorToWordColor(string hex)
        {
            var color = (Color)ColorConverter.ConvertFromString(hex);
            return (WdColor)(color.R + 0x100 * color.G + 0x10000 * color.B);
        }
    }
}
