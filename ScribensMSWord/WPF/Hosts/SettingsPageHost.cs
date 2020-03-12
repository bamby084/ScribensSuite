using System.Windows.Forms;
using ScribensMSWord.WPF;

namespace ScribensMSWord
{
    public partial class SettingsPageHost : Form
    {
        public SettingsPageHost()
        {
            InitializeComponent();
            this.Text = Globals.GetString("SettingsWindow.Title");
            settingsPage.SaveCommand = new RelayCommand(Save);
            settingsPage.CancelCommand = new RelayCommand(Cancel);
        }

        private void Save(object param)
        {
            settingsPage.Save();
            Globals.ThisAddIn.RefreshRibbon();
            Globals.Windows.UpdateTaskPanes();
            this.Close();
        }

        private void Cancel(object param)
        {
            this.Close();
        }
    }
}
