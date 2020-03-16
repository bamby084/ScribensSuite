using System.Windows.Threading;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using SetupUI.ViewModels;
using SetupUI.Views;

namespace SetupUI
{
    public class Bootstrapper: BootstrapperApplication
    {
        public Dispatcher BootstrapDispatcher { get; private set; }

        protected override void Run()
        {
            this.Engine.Log(LogLevel.Verbose, "Launching custom UI");
            BootstrapDispatcher = Dispatcher.CurrentDispatcher;
            var mainViewModel = new MainViewModel(this);
            this.Engine.Detect();
            var mainView = new MainView();
            mainView.DataContext = mainViewModel;
            mainView.Closed += (sender, e) => BootstrapDispatcher.InvokeShutdown();
            mainView.Show();
            Dispatcher.Run();
            this.Engine.Quit(0);
        }
    }
}
