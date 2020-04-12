using Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools;
using System.Linq;

namespace PluginScribens_Word.ExtensionMethods
{
    public static class TaskPaneExtensions
    {
        public static CustomTaskPane GetTaskPane(this CustomTaskPaneCollection taskPanes, int hWnd)
        {
            return taskPanes.Where(tp=>tp.Window != null)
                .FirstOrDefault(tp => ((Window)tp.Window).Hwnd == hWnd && tp.Control is TaskPaneHost);
        }
    }
}
