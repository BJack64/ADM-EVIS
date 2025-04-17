using eFakturADM.SchedulerService.Scheduler;

namespace eFakturADM.SchedulerService
{
    class Program
    {
        static void Main(string[] args)
        {
            CleanTemporaryFileScheduler.Run();
        }
    }
}
