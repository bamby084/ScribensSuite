using System.Drawing;
using System.Windows.Forms;
using PluginScribens.UI.Views;


namespace PluginScribens.UI.Hosts
{
    public partial class TaskPaneHost : UserControl
    {
        public TaskPaneHost()
        {
            InitializeComponent();
            this.Font = new Font(new FontFamily("Segoe UI"), 15, FontStyle.Regular, GraphicsUnit.Pixel);
        }

        public TaskPaneView TaskPane => taskPane;
    }
}
