using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Transactions;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;

namespace eFakturADM.DJPService
{
    partial class RequestDetailTransaksiService : ServiceBase
    {
        private bool isInProgress;
        System.Timers.Timer timer1 = new System.Timers.Timer();
        public RequestDetailTransaksiService()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            string logKey;
            try
            {
                Logger.WriteLog(out logKey, LogLevel.Info, "Starting Service...", MethodBase.GetCurrentMethod());

                DjpServiceConfiguration.LoadConfig();

                //delete log
                LogFPDigantiOutstandings.DeleteOldLog();

                double waktu = Convert.ToDouble(DjpServiceConfiguration.ServiceRequestDetailFakturPajakTimeInterval);
                timer1.Interval = waktu * 1000;
                timer1.Elapsed += timer1_Elapsed;
                timer1.Enabled = true;
                isInProgress = false;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(out logKey, LogLevel.Error, ex.Message, MethodBase.GetCurrentMethod(), ex);
            }
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            string logKey;
            Logger.WriteLog(out logKey, LogLevel.Info, "Stopping Service...", MethodBase.GetCurrentMethod());
        }

        void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (!isInProgress)
                {
                    isInProgress = true;
                    var dailythread = new System.Threading.Thread(DailyProcessJob);
                    dailythread.Start();
                    dailythread.Join();
                }
            }
            catch (Exception ex)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, "Daily Process Job Error with message :" + ex.Message, MethodBase.GetCurrentMethod(), ex);
            }
        }

        //private DateTime DailyLastProcessDateTime;
        private void DailyProcessJob()
        {
            try
            {
                RequestDetailTransaksiLib.DoJob();
                isInProgress = false;
                System.Threading.Thread.Sleep(DjpServiceConfiguration.ServiceRequestDetailFakturPajakProcessInterval);
            }
            catch (Exception ex)
            {
                string outlogkey;
                Logger.WriteLog(out outlogkey, LogLevel.Error, ex.Message, MethodBase.GetCurrentMethod(), ex);
                isInProgress = false;
                System.Threading.Thread.Sleep(DjpServiceConfiguration.ServiceRequestDetailFakturPajakProcessInterval);
            }
        }
        
    }
}
