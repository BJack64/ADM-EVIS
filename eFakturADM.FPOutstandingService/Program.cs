using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace eFakturADM.FPOutstandingService
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
            var svc = new CheckOutstandingService();
            svc.OnDebug();
            Console.ReadLine();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new CheckOutstandingService() 
            };
            ServiceBase.Run(ServicesToRun);
#endif

        }
    }
}
