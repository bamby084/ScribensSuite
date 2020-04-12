using System.Collections.ObjectModel;
using System.Linq;

namespace PluginScribens.Common
{
    public class WindowCollection : Collection<IWindow>
    {

    }

    public interface IWindowManager
    {
        IWindow GetWindow(int hWnd);
        IWindow AddNewWindow(int hWnd);
        void RemoveWindow(int hWnd);
    }

    public abstract class WindowManager: IWindowManager
    {
        private WindowCollection _windows;

        protected WindowManager()
        {
            _windows = new WindowCollection();
        }

        public IWindow GetWindow(int hWnd)
        {
            return _windows.FirstOrDefault(w => w.Hwnd == hWnd);
        }

        public IWindow AddNewWindow(int hWnd)
        {
            if (_windows.Any(w => w.Hwnd == hWnd))
                return _windows.FirstOrDefault(w => w.Hwnd == hWnd);

            IWindow newWindow = CreateWindow(hWnd);
            if (newWindow != null)
                _windows.Add(newWindow);

            return newWindow;
        }

        public abstract IWindow CreateWindow(int hWnd);
        
        public void RemoveWindow(int hWnd)
        {
            var window = _windows.FirstOrDefault(w => w.Hwnd == hWnd);
            if (window != null)
                _windows.Remove(window);
        }
    }
}
