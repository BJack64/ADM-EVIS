using System;
using System.Linq;
using System.Reflection;
using eFakturADM.Shared.Utility;
using  WatcherLibrary;

using System.ServiceProcess;
using System.IO;

namespace WatcherService
{
    public partial class ServiceWatcher : ServiceBase
    {
        private bool isInProgress;
        public ServiceWatcher()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        System.Timers.Timer timer1 = new System.Timers.Timer();

        protected override void OnStart(string[] args)
        {
            //Log.JustLog("Starting Service... ", MethodBase.GetCurrentMethod());
            string logKey;
            Logger.WriteLog(out logKey, LogLevel.Info, "Starting Service...", MethodBase.GetCurrentMethod());
            double waktu = Convert.ToDouble(WatcherConfiguration.TimeSleep);
            timer1.Interval = waktu * 1000;
            timer1.Elapsed += timer1_Elapsed;
            timer1.Enabled = true;
            isInProgress = false;
        }

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
            var dirToProcess = new DirectoryInfo(WatcherConfiguration.DataFolder);
            var fileExt = WatcherConfiguration.FileExt.Split(',').ToList();
            int maxProcessFiles = int.Parse(WatcherConfiguration.MaxProcessFiles);

            try
            {
                foreach (var ext in fileExt)
                {
                    //isInProgress = false;

                    var filesToProcess = dirToProcess.GetFiles(string.Format("*.{0}", ext)).OrderBy(x => x.CreationTime).Take(maxProcessFiles).ToList();
                    if (filesToProcess.Count <= 0) continue;
                    //Log.JustLog("Start Process File with ext : " + ext + Environment.NewLine + "File to Process : " + filesToProcess.Count + " Files", MethodBase.GetCurrentMethod());

                    string logKey;
                    Logger.WriteLog(out logKey, LogLevel.Info, "Start Process File with ext : " + ext + Environment.NewLine + "File to Process : " + filesToProcess.Count + " Files", MethodBase.GetCurrentMethod());

                    foreach (FileInfo file in filesToProcess)
                    {
                        isInProgress = true;
                        //Log.JustLog("Start Process File : " + file.FullName, MethodBase.GetCurrentMethod());
                        Logger.WriteLog(out logKey, LogLevel.Info, "Start Process File : " + file.FullName, MethodBase.GetCurrentMethod());
                        var watchFileProcess = new Watcher();
                        watchFileProcess.StartReadFile(file.FullName);
                        //Log.JustLog("End Process File : " + file.FullName, MethodBase.GetCurrentMethod());
                        Logger.WriteLog(out logKey, LogLevel.Info, "End Process File : " + file.FullName, MethodBase.GetCurrentMethod());
                    }

                    System.Threading.Thread.Sleep(WatcherConfiguration.TimeSleep);
                }
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
