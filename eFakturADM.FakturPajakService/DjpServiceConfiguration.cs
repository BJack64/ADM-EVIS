using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;

namespace eFakturADM.DJPService
{
    public class DjpServiceConfiguration
    {

        #region ------------ Private Proprerties --------

        private static string _InternetProxy;
        private static int? _InternetProxyPort;
        private static bool? _UseDefaultCredential;
        private static int _DJPRequestTimeOutSetting;
        private static int _ServiceRequestDetailFakturPajakBatchItem;
        private static int _ServiceRequestDetailFakturPajakTimeInterval;
        private static int _ServiceRequestDetailFakturPajakDjpRequestInterval;
        private static int _ServiceRequestDetailFakturPajakProcessInterval;
        private static DateTime _ServiceRequestDetailFakturPajakStartAtTime;
        private static int _MinPelaporan;
        private static int _MaxPelaporan;

        #endregion

        public static void LoadConfig()
        {
            var cfgs = GeneralConfigs.GetAll();

            _InternetProxy = GetConfig<string>(cfgs, ApplicationEnums.GeneralConfig.InternetProxy, null);
            try
            {
                _InternetProxyPort = GetConfig(cfgs, ApplicationEnums.GeneralConfig.InternetProxyPort, 0);
            }
            catch (Exception)
            {
                _InternetProxyPort = null;
            }
            try
            {
                _UseDefaultCredential = GetConfig(cfgs, ApplicationEnums.GeneralConfig.UseDefaultCredential, false);
            }
            catch (Exception)
            {
                _UseDefaultCredential = null;
            }
            _DJPRequestTimeOutSetting = GetConfig(cfgs, ApplicationEnums.GeneralConfig.DJPRequestTimeOutSetting, 60000);
            _ServiceRequestDetailFakturPajakBatchItem =
                GetConfig(cfgs, ApplicationEnums.GeneralConfig.ServiceRequestDetailFakturPajakBatchItem, 100);
            _ServiceRequestDetailFakturPajakTimeInterval =
                GetConfig(cfgs, ApplicationEnums.GeneralConfig.ServiceRequestDetailFakturPajakTimeInterval, 1);
            _ServiceRequestDetailFakturPajakDjpRequestInterval =
                GetConfig(cfgs, ApplicationEnums.GeneralConfig.ServiceRequestDetailFakturPajakDjpRequestInterval, 2000);
            _ServiceRequestDetailFakturPajakProcessInterval =
                GetConfig(cfgs, ApplicationEnums.GeneralConfig.ServiceRequestDetailFakturPajakProcessInterval, 10000);

            var cfgServiceRequestDetailFakturPajakStartAt =
                GetConfig(cfgs, ApplicationEnums.GeneralConfig.ServiceRequestDetailFakturPajakStartAt, "now");
            if (string.IsNullOrEmpty(cfgServiceRequestDetailFakturPajakStartAt) ||
                cfgServiceRequestDetailFakturPajakStartAt.ToLower() == "now")
            {
                _ServiceRequestDetailFakturPajakStartAtTime = DateTime.Now;
            }
            else
            {
                var settingat = cfgServiceRequestDetailFakturPajakStartAt.Split(':');
                var hh = Convert.ToInt32(settingat[0]);
                var mm = Convert.ToInt32(settingat[1]);
                _ServiceRequestDetailFakturPajakStartAtTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hh, mm, 0);
            }

            var cfgFPDigantiOutstandingTglFaktur = GetConfig(cfgs,
                ApplicationEnums.GeneralConfig.FPDigantiOutstandingTglFaktur, "[-3]:[0]");

            var cfgdats = cfgFPDigantiOutstandingTglFaktur.Split(':').ToList();
            if (cfgdats.Count == 2)
            {
                string outlogkey;
                try
                {
                    _MinPelaporan = int.Parse(cfgdats[0].Replace("[", "").Replace("]", ""));
                }
                catch (Exception exx)
                {
                    Logger.WriteLog(out outlogkey, LogLevel.Error, exx.Message, MethodBase.GetCurrentMethod(), exx);
                    _MinPelaporan = -3;
                }
                try
                {
                    _MaxPelaporan = int.Parse(cfgdats[1].Replace("[", "").Replace("]", ""));
                }
                catch (Exception exx)
                {
                    Logger.WriteLog(out outlogkey, LogLevel.Error, exx.Message, MethodBase.GetCurrentMethod(), exx);
                    _MaxPelaporan = 0;
                }
            }
            else
            {
                _MinPelaporan = -3;
                _MaxPelaporan = 0;
            }

        }

        private static T GetConfig<T>(IEnumerable<GeneralConfig> cfgs,  ApplicationEnums.GeneralConfig econfig, T defaultvalue)
        {
            var cfg = cfgs.FirstOrDefault(c => c.GeneralConfigId == (int) econfig);
            if (cfg == null || cfg.GeneralConfigId == 0)
            {
                string outlogkey;
                Logger.WriteLog(out outlogkey, LogLevel.Info, "Use " +
                                                              "Default Config [" + EnumHelper.GetDescription(econfig) + "] : " + defaultvalue, MethodBase.GetCurrentMethod());
                return defaultvalue;
            }

            var val = cfg.ConfigValue;
            if (string.IsNullOrEmpty(val))
            {
                string outlogkey;
                Logger.WriteLog(out outlogkey, LogLevel.Info, "Use " +
                                                              "Default Config [" + EnumHelper.GetDescription(econfig) + "] : " + defaultvalue, MethodBase.GetCurrentMethod());
                return defaultvalue;
            }

            return typeof(T).IsEnum
                ? (T)Enum.Parse(typeof(T), val)
                : (T)Convert.ChangeType(val, typeof(T));
        }

        public const string Actor = "System";

        public static string InternetProxy { get { return _InternetProxy; } }
        public static int? InternetProxyPort { get { return _InternetProxyPort; } }
        public static bool? UseDefaultCredential { get { return _UseDefaultCredential; } }
        public static int DJPRequestTimeOutSetting { get { return _DJPRequestTimeOutSetting; } }
        public static int ServiceRequestDetailFakturPajakBatchItem { get
        {
            return _ServiceRequestDetailFakturPajakBatchItem;
        } }
        public static int ServiceRequestDetailFakturPajakTimeInterval { get
        {
            return _ServiceRequestDetailFakturPajakTimeInterval;
        } }
        public static int ServiceRequestDetailFakturPajakDjpRequestInterval { get
        {
            return _ServiceRequestDetailFakturPajakDjpRequestInterval;
        } }
        public static int ServiceRequestDetailFakturPajakProcessInterval { get
        {
            return _ServiceRequestDetailFakturPajakProcessInterval;
        } }
        public static DateTime ServiceRequestDetailFakturPajakStartAtTime { get
        {
            return _ServiceRequestDetailFakturPajakStartAtTime;
        } }
        public static int MinPelaporan { get { return _MinPelaporan; } }
        public static int MaxPelaporan { get { return _MaxPelaporan; } }
    }
}
