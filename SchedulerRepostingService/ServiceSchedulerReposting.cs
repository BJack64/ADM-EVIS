using System;
using System.Linq;
using System.Reflection;
using eFakturADM.Shared.Utility;
using WatcherLibrary;

using System.ServiceProcess;
using System.IO;
using eFakturADM.Logic.Collections;
using eFakturADM.ExternalLib;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using Newtonsoft.Json;
using eFakturADM.DJPService;
using System.Net;
using eFakturADM.DJPLib;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SchedulerRepostingService
{
    public partial class ServiceSchedulerReposting : ServiceBase
    {
        static bool isInProgress;

        public ServiceSchedulerReposting()
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
            string logKey;
            Logger.WriteLog(out logKey, LogLevel.Info, "Starting Service...", MethodBase.GetCurrentMethod());
            double waktu = Convert.ToDouble(WatcherConfiguration.TimeSleep);
            timer1.Interval = waktu * 1000;
            timer1.Elapsed += timer1_Elapsed;
            timer1.Enabled = true;
            isInProgress = false;

            StartProcessTask();
        }

        void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string logKey;
            if (isInProgress) return;
            try
            {
                if (!isInProgress)
                {
                    isInProgress = true;
                    StartProcessTask();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(out logKey, LogLevel.Error, "Error with message :" + ex.Message, MethodBase.GetCurrentMethod(), ex);
            }
            Logger.WriteLog(out logKey, LogLevel.Info, "End Execute timer", MethodBase.GetCurrentMethod());
            
        }

        protected override void OnStop()
        {
            string logKey;
            Logger.WriteLog(out logKey, LogLevel.Info, "Stopping Service...", MethodBase.GetCurrentMethod());
        }

        private static Task TaskRespostingDate(long Id, string Source, string Url, string Method, string Payload) {

            var post = new ExternalLib();

            var MsgError = "";
            var eStatus = WebExceptionStatus.Success;
            var logkey = "";

            if (Source.ToLower() == "ecm")
            {
                var result = post.RequestV3(ApplicationEnums.EnumLogApiAction.RepostingDate, Url, Method, Payload, out MsgError, out eStatus, out logkey);
                LogPostingTanggalLaporans.SetStatus(Id, result.IsSuccess, MsgError);
            }
            else if (Source.ToLower() == "delvi")
            {
                var result = post.RequestV3(ApplicationEnums.EnumLogApiAction.RepostingDate, Url, Method, Payload, out MsgError, out eStatus, out logkey, false);
                LogPostingTanggalLaporans.SetStatus(Id, result.IsSuccess, MsgError);
            } 

            return Task.CompletedTask;
        }

        private static Task TaskSendFaktur(long Id, string Source, string Url, string Method, string Payload)
        {

            var post = new ExternalLib();

            var MsgError = "";
            var eStatus = WebExceptionStatus.Success;
            var logkey = "";

            if (Source.ToLower() == "delvi")
            {
                var result = post.RequestV3(ApplicationEnums.EnumLogApiAction.SendFaktur, Url, Method, Payload, out MsgError, out eStatus, out logkey, false);
                LogPostingTanggalLaporans.SetStatus(Id, result.IsSuccess, MsgError);
            }

            return Task.CompletedTask;
        }

        private static Task TaskHitDJP(long Id, string Source, string Url, string FPdjpID, string CreatedBy) {
            var msgError = "";
            WebExceptionStatus eStatus;
            var logKey = "";
            if (Source.ToLower() == "delvi")
            {
                DjpServiceConfiguration.LoadConfig();

                var inetProxy = DjpServiceConfiguration.InternetProxy;
                var inetProxyPort = DjpServiceConfiguration.InternetProxyPort;
                var inetProxyUseCredential = DjpServiceConfiguration.UseDefaultCredential;
                var itimeoutsetting = DjpServiceConfiguration.DJPRequestTimeOutSetting;
                var reqinterval = DjpServiceConfiguration.ServiceRequestDetailFakturPajakDjpRequestInterval;
                bool isUseProxy = !string.IsNullOrEmpty(inetProxy);

                ValidateFakturLib.ValidateFakturObjectAPI(Url, Source, CreatedBy, FPdjpID,
                        itimeoutsetting, isUseProxy, inetProxy, inetProxyPort
                        , inetProxyUseCredential, out msgError, out eStatus, out logKey, false, true,Id);

                var resultHit = eStatus == WebExceptionStatus.Success ? true : false;

                LogPostingTanggalLaporans.SetStatus(Id, resultHit, msgError);
            }
            return Task.CompletedTask;
        }

        private static void StartProcessTask()
        {
            var logKey = "";
            Logger.WriteLog(out logKey, LogLevel.Info, "Start Process Async", MethodBase.GetCurrentMethod());
            //adding the code
            //isInProgress = true;
            string getTime = GeneralConfigs.GetByConfigKey(EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.SchedulerReposting)).ConfigValue;
            TimeSpan timeCurrent = DateTime.Now.TimeOfDay;
            TimeSpan timeConfig = new TimeSpan();

            if (!String.IsNullOrEmpty(getTime))
                timeConfig = DateTime.Parse(string.Concat(timeCurrent.ToString("yyyy-MM-dd"), " ", getTime)).TimeOfDay;
            else
                timeConfig = timeCurrent;

            if (TimeSpan.Compare(timeConfig, timeCurrent) == 0)
            {
                //DJP Error, Hit Faktur, Resposting Date
                List<Task> listOfTask = new List<Task>();

                var getData = LogPostingTanggalLaporans.GetPendingData(EnumHelper.GetDescription(ApplicationEnums.EnumLogApiAction.DJPError), false);
                for (int i = 0; i < getData.ToList().Count; i++)
                {
                    var data = getData[i];
                    listOfTask.Add(TaskHitDJP(data.Id, data.Source, data.Url, data.FPdjpID, data.CreatedBy));
                }

                getData = LogPostingTanggalLaporans.GetPendingData(EnumHelper.GetDescription(ApplicationEnums.EnumLogApiAction.RepostingDate));
                for (int i = 0; i < getData.ToList().Count; i++)
                {
                    var data = getData[i];
                    listOfTask.Add(TaskRespostingDate(data.Id, data.Source, data.Url, data.Method, data.Payload));
                }

                getData = LogPostingTanggalLaporans.GetPendingData(EnumHelper.GetDescription(ApplicationEnums.EnumLogApiAction.SendFaktur));
                for (int i = 0; i < getData.ToList().Count; i++)
                {
                    var data = getData[i];
                    listOfTask.Add(TaskSendFaktur(data.Id, data.Source, data.Url, data.Method, data.Payload));
                }

                Task.WhenAll(listOfTask);
            }
            Logger.WriteLog(out logKey, LogLevel.Info, "End Process Async", MethodBase.GetCurrentMethod());
            isInProgress = false;
        }



        [Obsolete("Diganti dengan thread")]
        private void StartProcess()
        {
            //adding the code
            isInProgress = true;
            string getTime = GeneralConfigs.GetByConfigKey(EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.SchedulerReposting)).ConfigValue;
            TimeSpan timeCurrent = DateTime.Now.TimeOfDay;
            TimeSpan timeConfig = new TimeSpan();
            if (!String.IsNullOrEmpty(getTime))
                timeConfig = DateTime.Parse(string.Concat(timeCurrent.ToString("yyyy-MM-dd"), " ", getTime)).TimeOfDay;
            else
                timeConfig = timeCurrent;

            if (TimeSpan.Compare(timeConfig, timeCurrent) == 0)
            {
                //DJP Error, Hit Faktur, Resposting Date
                var getData = LogPostingTanggalLaporans.GetPendingData(EnumHelper.GetDescription(ApplicationEnums.EnumLogApiAction.RepostingDate));
                for (int i = 0; i < getData.ToList().Count; i++)
                {
                    var data = getData[i];
                    var post = new ExternalLib();

                    var msgError = "";
                    WebExceptionStatus eStatus;
                    var logkey = "";
                    if (data.Source.ToLower() == "ecm")
                    {
                        var result = post.RequestV3(ApplicationEnums.EnumLogApiAction.RepostingDate, data.Url, data.Method, data.Payload, out msgError, out eStatus, out logkey);
                        LogPostingTanggalLaporans.SetStatus(data.Id, result.IsSuccess, msgError);
                    }
                    else if (data.Source.ToLower() == "delvi")
                    {
                        var result = post.RequestV3(ApplicationEnums.EnumLogApiAction.RepostingDate,data.Url, data.Method, data.Payload, out msgError, out eStatus, out logkey,false);
                        LogPostingTanggalLaporans.SetStatus(data.Id, result.IsSuccess , msgError);
                    }
                }
                getData = LogPostingTanggalLaporans.GetPendingData(EnumHelper.GetDescription(ApplicationEnums.EnumLogApiAction.SendFaktur));
                for (int i = 0; i < getData.ToList().Count; i++)
                {
                    var data = getData[i];
                    var post = new ExternalLib();

                    var msgError = "";
                    System.Net.WebExceptionStatus eStatus;
                    var logkey = "";
                    if (data.Source.ToLower() == "delvi")
                    {
                        var result = post.RequestV3(ApplicationEnums.EnumLogApiAction.SendFaktur,data.Url, data.Method, data.Payload, out msgError, out eStatus, out logkey,false);
                        LogPostingTanggalLaporans.SetStatus(data.Id, result.IsSuccess, msgError);
                    }
                }
                getData = LogPostingTanggalLaporans.GetPendingData(EnumHelper.GetDescription(ApplicationEnums.EnumLogApiAction.DJPError), false);
                for (int i = 0; i < getData.ToList().Count; i++)
                {
                    var data = getData[i];
                    var msgError = "";
                    if (data.Source.ToLower() == "delvi")
                    {
                        var resultHit = HitDJP(data.Url, data.FPdjpID, data.Source, data.CreatedBy, out msgError);
                        LogPostingTanggalLaporans.SetStatus(data.Id, resultHit,msgError);
                    }
                }
            }




            isInProgress = false;
        }

        [Obsolete("Diganti dengan thread")]
        public bool HitDJP(string Url, string FPdjpID, string Source, string userName, out string msgError )
        {
            string errMsg = string.Empty;
            bool result = false;

            WebExceptionStatus eStatus;
            string logKey;

            DjpServiceConfiguration.LoadConfig();

            var inetProxy = DjpServiceConfiguration.InternetProxy;
            var inetProxyPort = DjpServiceConfiguration.InternetProxyPort;
            var inetProxyUseCredential = DjpServiceConfiguration.UseDefaultCredential;
            var itimeoutsetting = DjpServiceConfiguration.DJPRequestTimeOutSetting;
            var reqinterval = DjpServiceConfiguration.ServiceRequestDetailFakturPajakDjpRequestInterval;
            bool isUseProxy = !string.IsNullOrEmpty(inetProxy);

            ValidateFakturLib.ValidateFakturObjectAPI(Url, Source, userName, FPdjpID,
                    itimeoutsetting, isUseProxy, inetProxy, inetProxyPort
                    , inetProxyUseCredential, out msgError, out eStatus, out logKey, false,true);

            result = eStatus == WebExceptionStatus.Success ? true : false;
            return result;
        }

    }
}
