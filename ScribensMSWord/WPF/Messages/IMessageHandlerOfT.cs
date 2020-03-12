
namespace PluginScribens_Word.WPF.Messages
{
    public interface IMessageHandler<TMessage>
        where TMessage: ViewModelMessage
    {
        void HandleMessage(TMessage message);
    }
}
