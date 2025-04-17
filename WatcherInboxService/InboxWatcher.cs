using System;
using System.Reflection;
using System.ServiceProcess;
using eFakturADM.Shared.Utility;
using WatcherLibraryMoveFile;

namespace WatcherInboxService
{
    partial class InboxWatcher : ServiceBase
    {
        private bool isInProgress;
        public InboxWatcher()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            //Log.JustLog("Starting Service... ", MethodBase.GetCurrentMethod());
            string logKey = "";
            Logger.WriteLog(out logKey, LogLevel.Info, "Starting Service...", MethodBase.GetCurrentMethod());
            double waktu = Convert.ToDouble(WatcherMoveConfiguration.TimeSleep);
            timer1.Interval = waktu * 1000;
            timer1.Elapsed += timer1_Elapsed;
            timer1.Enabled = true;
            isInProgress = false;
        }

        System.Timers.Timer timer1 = new System.Timers.Timer();

        void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Log.JustLog("Execute timer", MethodBase.GetCurrentMethod());
            if (isInProgress) return;
            try
            {
                timer1.Enabled = false;
                StartProcessFileInFolder();
            }
            catch (Exception ex)
            {
                //Log.WriteLog("Error with message :" + ex.Message, MethodBase.GetCurrentMethod());
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, "Error with message :" + ex.Message, MethodBase.GetCurrentMethod(), ex);
            }
            //Log.JustLog("End Execute timer", MethodBase.GetCurrentMethod());
            timer1.Enabled = true;
        }

        protected override void OnStop()
        {
            //Log.JustLog("Stopping Service...", MethodBase.GetCurrentMethod());
            string logKey;
            Logger.WriteLog(out logKey, LogLevel.Info, "Stopping Service...", MethodBase.GetCurrentMethod());
        }

        private void StartProcessFileInFolder()
        {

            try
            {
                var watcher = new WatcherMove();
                watcher.onRunning();

                isInProgress = false;
            }
            catch (Exception exception)
            {
                //Log.WriteLog("Error (StartProcessFileInFolder) : " + exception.Message, MethodBase.GetCurrentMethod());
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, "Error (StartProcessFileInFolder) : " + exception.Message, MethodBase.GetCurrentMethod(), exception);
                isInProgress = false;
                throw;
            }

        }

    }
}
