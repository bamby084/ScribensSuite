using PluginScribens_Word.WPF.ViewModels;

namespace PluginScribens_Word.WPF.Messages
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
