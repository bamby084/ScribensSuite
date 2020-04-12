using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PluginScribens.UI.Views
{
    /// <summary>
    /// Interaction logic for UserInfoView.xaml
    /// </summary>
    public partial class UserInfoView : UserControl
    {
        public UserInfoView()
        {
            InitializeComponent();
        }

        #region Events
        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
        #endregion
    }
}
