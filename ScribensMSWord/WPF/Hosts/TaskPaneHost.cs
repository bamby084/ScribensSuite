using System.Drawing;
using System.Windows.Forms;
using ScribensMSWord.WPF.Views;

namespace ScribensMSWord
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
