using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SetupUI.Views
{
    /// <summary>
    /// Interaction logic for LicenseAgreementView.xaml
    /// </summary>
    public partial class LicenseAgreementView : UserControl
    {
        public LicenseAgreementView()
        {
            InitializeComponent();

            using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resource.License)))
            {
                LicenseTextBox.Selection.Load(memStream, DataFormats.Rtf);
            }
        }
    }
}
