using Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools;
using PluginScribens.Common;

namespace PluginScribens_Word.ExtensionMethods
{
    public static class DocumentExtensions
    {
        public static CustomTaskPane GetTaskPane(this Document document)
        {
            var applicationWindow = Globals.ThisAddIn.Application.Windows.GetWindowByDocument(document);
            if (applicationWindow == null)
                return null;

            IWindow window = Plugin.Windows.GetWindow(applicationWindow.Hwnd);
            if (window == null)
                return null;

            return window.GetTaskPane();
        }
    }
}
