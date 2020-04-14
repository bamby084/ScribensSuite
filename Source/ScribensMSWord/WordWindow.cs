using System;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools;
using PluginScribens.Common;
using PluginScribens.Common.Enums;
using PluginScribens.Common.ExtensionMethods;
using PluginScribens.UI.Hosts;
using PluginScribens_Word.ExtensionMethods;
using Task = System.Threading.Tasks.Task;

namespace PluginScribens_Word
{
    public class WordWindow : IWindow
    {
        private const int TaskPaneWidth = 400;

        public WordWindow(int hWnd)
        {
            Hwnd = hWnd;
        }

        public int Hwnd { get; set; }

        public void ShowSolutions()
        {
            var taskPane = GetTaskPane();
            if (taskPane == null)
                taskPane = CreateTaskPane();

            taskPane.Visible = !taskPane.Visible;
            ((TaskPaneHost)taskPane.Control).TaskPane.Context.ShowSolutions();
        }

        public void HideTaskPane()
        {
            var taskPane = GetTaskPane();
            if (taskPane != null && taskPane.Visible)
            {
                taskPane.Visible = false;
            }
        }

        private CustomTaskPane CreateTaskPane()
        {
            var taskPaneHost = new TaskPaneHost();
            var applicationWindow = GetAssociatedWindow();
            if (applicationWindow != null)
            {
                taskPaneHost.TaskPane.Context.AssociatedDocument = applicationWindow.Document;
            }

            taskPaneHost.TaskPane.Context.WindowHost = this;
            taskPaneHost.TaskPane.Context.StartBackgroundChecker();

            var taskPane = Globals.ThisAddIn.CustomTaskPanes.Add(taskPaneHost, Plugin.GetString("TaskPane.Title"), GetAssociatedWindow());
            taskPane.VisibleChanged += OnTaskPaneVisibleChanged;
            taskPane.Width = TaskPaneWidth;

            return taskPane;
        }

        public void RemoveTaskPane()
        {
            var taskPane = GetTaskPane();
            if (taskPane != null)
            {
                taskPane.Visible = false;
                Globals.ThisAddIn.CustomTaskPanes.Remove(taskPane);
            }
        }

        public SolutionMode ActiveMode
        {
            get
            {
                var taskPane = GetTaskPane();
                if (taskPane == null)
                    return SolutionMode.None;

                return ((TaskPaneHost)taskPane.Control).TaskPane.Context.SolutionsViewModel.ActiveMode;
            }
            set
            {
                var taskPane = GetTaskPane();
                if (taskPane == null)
                    return;

                ((TaskPaneHost)taskPane.Control).TaskPane.Context.SolutionsViewModel.ActiveMode = value;
            }
        }

        public CustomTaskPane GetTaskPane()
        {
            return Globals.ThisAddIn.CustomTaskPanes.GetTaskPane(Hwnd);
        }

        public bool AllowMode(SolutionMode mode)
        {
            var taskPane = GetTaskPane();
            if (taskPane == null || !taskPane.Visible)
                return false;

            return ((TaskPaneHost)taskPane.Control).TaskPane.Context.SolutionsViewModel.AllowMode(mode);
        }

        public async Task ShowUserInfo()
        {
            var applicationWindow = GetAssociatedWindow();

            var taskPane = GetTaskPane();
            if (taskPane == null)
                taskPane = CreateTaskPane();

            if (!taskPane.Visible)
                taskPane.Visible = true;


            await ((TaskPaneHost)taskPane.Control).TaskPane.Context.ShowUserInfo();
        }

        public void ReCheck()
        {
            var taskPane = GetTaskPane();
            if (taskPane != null && taskPane.Visible)
            {
                ((TaskPaneHost)taskPane.Control).TaskPane.Context.SolutionsViewModel.ReCheckAll();
            }
        }

        private Window GetAssociatedWindow()
        {
            return Globals.ThisAddIn.Application.Windows.GetWindowByHwnd(Hwnd);
        }

        private void OnTaskPaneVisibleChanged(object sender, EventArgs e)
        {
            Globals.ThisAddIn.Ribbon.Invalidate();

            var taskPane = (CustomTaskPane)sender;
            ((TaskPaneHost)taskPane.Control).TaskPane.Context.IsBackgroundCheckerEnabled = taskPane.Visible;

            if (taskPane.Visible)
            {
                var window = GetAssociatedWindow();
                if (window != null)
                {
                    bool isSaved = window.Document.Saved;
                    window.Document.Content.NoProofing = 1;
                    window.Document.ShowSpellingErrors = false;
                    window.Document.ShowGrammaticalErrors = false;
                    window.Document.Saved = isSaved;
                }
            }
            else
            {
                var window = GetAssociatedWindow();
                if (window != null)
                {
                    bool isSaved = window.Document.Saved;
                    window.Document.Content.NoProofing = 0;
                    window.Document.ShowGrammaticalErrors = true;
                    window.Document.ShowSpellingErrors = true;
                    window.Document.Saved = isSaved;
                }

                if (!Globals.BackStageVisible)
                    ((TaskPaneHost)taskPane.Control).TaskPane.Context.SolutionsViewModel.Clear();
            }
        }
    }
}
