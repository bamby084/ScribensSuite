using System;
using System.Windows;
using System.Windows.Controls;
using SetupUI.Events;


namespace SetupUI.Views
{
    /// <summary>
    /// Interaction logic for SetupView.xaml
    /// </summary>
    public partial class SetupView : UserControl
    {
        public SetupView()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetupUI.Events.EventManager.PublishEvent(new BeginEvent(){FromScreen = SetupScreen.InstallationProgress});
        }
    }
}
