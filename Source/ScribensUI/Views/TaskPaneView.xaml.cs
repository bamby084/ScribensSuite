using PluginScribens.UI.ViewModels;

namespace PluginScribens.UI.Views
{
    public partial class TaskPaneView
    {
        public TaskPaneViewModel Context
        { 
            get
            {
                TaskPaneViewModel taskPaneViewModel = null;
                Dispatcher.Invoke(() =>
                {
                    taskPaneViewModel = (TaskPaneViewModel)this.DataContext;
                });
                return taskPaneViewModel;
            }
        }

        public TaskPaneView()
        {
            InitializeComponent();
            this.DataContext = new TaskPaneViewModel();
        }
    }
}
