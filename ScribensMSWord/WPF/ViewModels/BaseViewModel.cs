﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using ScribensMSWord.Utils;
using ScribensMSWord.WPF.Messages;

namespace ScribensMSWord.WPF.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual IWindow WindowHost { get; set; }

        public BaseViewModel()
        {
            bool isMessageHandler = this.GetType().GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>));

            if (isMessageHandler)
                Messenger.Register(this);
        }

        ~BaseViewModel()
        {
            Messenger.UnRegister(this);
        }
    }
}
