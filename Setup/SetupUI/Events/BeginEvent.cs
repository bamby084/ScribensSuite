
namespace SetupUI.Events
{
    public enum SetupScreen
    {
        Welcome,
        ProductSelection,
        InstallationProgress
    }

    public class BeginEvent: EventBase
    {
        public SetupScreen FromScreen { get; set; }
    }
}
