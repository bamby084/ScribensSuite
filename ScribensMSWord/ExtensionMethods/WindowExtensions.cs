using System;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using ScribensMSWord.Utils;
using System.Linq;

namespace ScribensMSWord.ExtensionMethods
{
    public static class WindowExtensions
    {
        public static Window GetWindowByDocument(this Windows windows, Document document)
        {
            try
            {
                return windows.Cast<Window>().FirstOrDefault(w => w.Document.DocID == document.DocID);
            }
            catch
            {
                return null;
            }
        }

        public static Window GetWindowByHwnd(this Windows windows, int hWnd)
        {
            try
            {
                return windows.Cast<Window>().FirstOrDefault(w => w.Hwnd == hWnd);
            }
            catch
            {
                return null;
            }
        }

        public static IWindow GetWindow(this IRibbonControl control)
        {
            var applicationWindow = control.Context as Window;
            if (applicationWindow == null)
                return null;

            IWindow window = Globals.Windows.GetWindow(applicationWindow.Hwnd);
            return window;
        }
    }
}
