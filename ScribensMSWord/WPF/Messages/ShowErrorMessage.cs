using ScribensMSWord.WPF.ViewModels;

namespace ScribensMSWord.WPF.Messages
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
