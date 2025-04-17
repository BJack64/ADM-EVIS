using System;
using System.Reflection;
using System.ServiceProcess;
using eFakturADM.Shared.Utility;
using WatcherOutboxLibrary;

namespace WatcherOutboxService
{
    public partial class OutboxService : ServiceBase
    {
        System.Timers.Timer timer1 = new System.Timers.Timer();
        public OutboxService()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            //Log.JustLog("Starting Service..", MethodBase.GetCurrentMethod());
            string logKey;
            Logger.WriteLog(out logKey, LogLevel.Info, "Starting Service..", MethodBase.GetCurrentMethod());
            //double waktu = Convert.ToDouble(ConfigurationManager.AppSettings["waktujeda"]);
            double waktu = Convert.ToDouble(WatcherOutboxConfiguration.WaktujedaWatcherOutboxService);
            timer1.Interval = waktu * 1000;
            timer1.Elapsed += new System.Timers.ElapsedEventHandler(timer1_Elapsed);
            timer1.Enabled = true;
        }

        void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Log.JustLog("Execute timer", MethodBase.GetCurrentMethod());
            try
            {
                timer1.Enabled = false;
                Watcher watcher = new Watcher();
                watcher.onRunning();
            }
            catch (Exception ex)
            {
                //Log.WriteLog("Error with message :" + ex.Message, MethodBase.GetCurrentMethod());
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, "Error with message :" + ex.Message, MethodBase.GetCurrentMethod());

            }
            timer1.Enabled = true;
        }

        protected override void OnStop()
        {
            string logKey;
            //Log.JustLog("Stopping Service", MethodBase.GetCurrentMethod());
            Logger.WriteLog(out logKey, LogLevel.Info, "Stopping Service..", MethodBase.GetCurrentMethod());
        }
    }
}
