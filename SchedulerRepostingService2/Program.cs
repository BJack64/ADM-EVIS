using System;
using System.ServiceProcess;

namespace SchedulerRepostingService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
#if DEBUG
            var svc = new ServiceSchedulerReposting();
            svc.OnDebug();
            Console.ReadLine();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new ServiceSchedulerReposting() 
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
