using PluginScribens.UI.ViewModels;

namespace PluginScribens.UI.Messages
{
    public class ShowErrorMessage: ViewModelMessage
    {
       public ErrorViewModel ErrorViewModel { get; set; }

        public ShowErrorMessage(ErrorViewModel errorViewModel)
        {
            ErrorViewModel = errorViewModel;
        }
    }
}
