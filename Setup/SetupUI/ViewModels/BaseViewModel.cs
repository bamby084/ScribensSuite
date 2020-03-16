using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using SetupUI.Events;

namespace SetupUI.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public BaseViewModel()
        {
            bool isEventHandler = this.GetType().GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>));

            if (isEventHandler)
                EventManager.Register(this);
        }

        ~BaseViewModel()
        {
            EventManager.UnRegister(this);
        }
    }
}
