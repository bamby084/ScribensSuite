using PluginScribens_Word.WPF.ViewModels;

namespace PluginScribens_Word.WPF.Views
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
