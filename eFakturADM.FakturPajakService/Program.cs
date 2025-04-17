using System.ServiceProcess;

namespace eFakturADM.DJPService
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
            var svc = new RequestDetailTransaksiService();
            svc.OnDebug();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new RequestDetailTransaksiService() 
            };
            ServiceBase.Run(ServicesToRun);
#endif


        }
    }
}
