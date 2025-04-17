using System.ComponentModel;

namespace WatcherLibrary
{
    public class WatcherEnums
    {
        public enum ProcessStatusEnum
        {
            [Description("Success")]
            Success = 1,
            [Description("Failed")]
            Failed = 2
        }
    }
}
