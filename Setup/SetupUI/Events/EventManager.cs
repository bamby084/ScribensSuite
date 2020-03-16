using System.Collections.Generic;
using SetupUI.ViewModels;

namespace SetupUI.Events
{
    public class EventManager
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

        public static void PublishEvent<TEvent>(TEvent @event) where TEvent: EventBase
        {
            foreach (var viewModel in _viewModels)
            {
                if (viewModel is IEventHandler<TEvent> eventHandler)
                    eventHandler.HandleEvent(@event);
            }
        }
    }
}
