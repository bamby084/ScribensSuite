using SetupUI.Enums;

namespace SetupUI.Events
{
    public class InstallEvent: EventBase
    {
        public SetupAction Action { get; set; }
    }
}
