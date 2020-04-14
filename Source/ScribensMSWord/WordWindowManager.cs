using PluginScribens.Common;

namespace PluginScribens_Word
{
    public class WordWindowManager: WindowManager
    {
        public override IWindow CreateWindow(int hWnd)
        {
            return new WordWindow(hWnd);
        }
    }
}
