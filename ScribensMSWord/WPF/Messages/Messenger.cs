using System.Collections.Generic;
using System.Linq;
using ScribensMSWord.WPF.ViewModels;

namespace ScribensMSWord.WPF.Messages
{
    public class Messenger
    {
        private static List<BaseViewModel> _viewModels = new List<BaseViewModel>();
        public static void Register(BaseViewModel viewModel)
        {
            if (!_viewModels.Contains(viewModel))
                _viewModels.Add(viewModel);
        }

        public static void UnRegister(BaseViewModel viewModel)
        {
            if (_viewModels.Contains(viewModel))
                _viewModels.Remove(viewModel);
        }

        /// <summary>
        /// Send message to all view models in the same Word window
        /// </summary>
        /// <typeparam name="TMessage">Type of the message</typeparam>
        /// <param name="message">Message</param>
        /// <param name="owner">ViewModel which sends the message</param>
        public static void SendMessage<TMessage>(TMessage message, BaseViewModel owner)
            where TMessage: ViewModelMessage
        {
            var viewModelsInSameWindow = _viewModels.Where(vm => vm.WindowHost.Hwnd == owner.WindowHost.Hwnd);
            SendMessage(message, owner, viewModelsInSameWindow);
        }


        /// <summary>
        /// Broadcast the message to all view models in all windows
        /// </summary>
        /// <typeparam name="TMessage">Type of the message</typeparam>
        /// <param name="message">Message</param>
        /// <param name="owner">ViewModel which sends the message</param>
        public static void BroadCastMessage<TMessage>(TMessage message, BaseViewModel owner)
            where TMessage: ViewModelMessage
        {
            SendMessage(message, owner, _viewModels);
        }

        private static void SendMessage<TMessage>(TMessage message, BaseViewModel owner, IEnumerable<BaseViewModel> viewModels)
            where TMessage: ViewModelMessage
        {
            foreach (var viewModel in viewModels)
            {
                var messageHandler = viewModel as IMessageHandler<TMessage>;
                if (messageHandler != null && viewModel != owner)
                    messageHandler.HandleMessage(message);
            }
        }
    }
}
