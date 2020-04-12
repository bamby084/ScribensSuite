using System.Threading.Tasks;
using Microsoft.Office.Tools;
using PluginScribens.Common.Enums;

namespace PluginScribens.Common
{
    public interface IWindow
    {
        int Hwnd { get; set; }
        SolutionMode ActiveMode { get; set; }
        void ShowSolutions();
        void HideTaskPane();
        void RemoveTaskPane();
        CustomTaskPane GetTaskPane();
        bool AllowMode(SolutionMode mode);
        Task ShowUserInfo();
        void ReCheck();
    }
}
