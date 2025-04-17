using System;
using System.Configuration;
using System.Reflection;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Shared.Utility;

namespace eFakturADM.Web.Helpers
{
    public class WebConfiguration
    {
        public static string InternetProxy
        {

            get
            {
                try
                {
                    return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.InternetProxy).ConfigValue;
                }
                catch (Exception exception)
                {
                    string outLogKey;
                    Logger.WriteLog(out outLogKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                    throw;
                }
            }
        }

        public static int? InternetProxyPort
        {
            get
            {
                try
                {
                    var cfg = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.InternetProxyPort).ConfigValue;
                    if (string.IsNullOrEmpty(cfg))
                    {
                        return null;
                    }
                    return Convert.ToInt32(cfg);
                }
                catch (Exception exception)
                {
                    string outLogKey;
                    Logger.WriteLog(out outLogKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                    throw;
                }
                
            }
        }

        public static bool? UseDefaultCredential
        {
            get
            {
                try
                {
                    var cfg = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.UseDefaultCredential).ConfigValue;
                    if (string.IsNullOrEmpty(cfg))
                    {
                        return null;
                    }
                    return Convert.ToBoolean(cfg);
                }
                catch (Exception exception)
                {
                    string outLogKey;
                    Logger.WriteLog(out outLogKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                    throw;
                }
                
            }
        }


        public static int YearSystemTest
        {
            get
            {
                var cfg = ConfigurationManager.AppSettings["YearSystemTest"];
                if (!string.IsNullOrEmpty(cfg) && cfg.ToLower() != "now")
                {
                    try
                    {
                        return int.Parse(cfg);
                    }
                    catch (Exception)
                    {
                        return DateTime.Now.Year;
                    }
                }
                else
                {
                    return DateTime.Now.Year;
                }
            }
        }

        public static int MonthSystemTest
        {
            get
            {
                var cfg = ConfigurationManager.AppSettings["MonthSystemTest"];
                if (!string.IsNullOrEmpty(cfg) && cfg.ToLower() != "now")
                {
                    try
                    {
                        return int.Parse(cfg);
                    }
                    catch (Exception)
                    {
                        return DateTime.Now.Month;
                    }
                }
                else
                {
                    return DateTime.Now.Month;
                }
            }
        }

        public static int DaySystemTest
        {
            get
            {
                var cfg = ConfigurationManager.AppSettings["DaySystemTest"];
                if (!string.IsNullOrEmpty(cfg) && cfg.ToLower() != "now")
                {
                    try
                    {
                        return int.Parse(cfg);
                    }
                    catch (Exception)
                    {
                        return DateTime.Now.Day;
                    }
                }
                else
                {
                    return DateTime.Now.Day;
                }
            }
        }

        public static string EcmApiUrl
        {

            get
            {
                try
                {
                    return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.EcmApiUrl).ConfigValue;
                }
                catch (Exception exception)
                {
                    string outLogKey;
                    Logger.WriteLog(out outLogKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                    throw;
                }
            }
        }

        public static string EcmApiUsername
        {

            get
            {
                try
                {
                    return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.EcmApiUsername).ConfigValue;
                }
                catch (Exception exception)
                {
                    string outLogKey;
                    Logger.WriteLog(out outLogKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                    throw;
                }
            }
        }

        public static string EcmApiPassword
        {

            get
            {
                try
                {
                    return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.EcmApiPassword).ConfigValue;
                }
                catch (Exception exception)
                {
                    string outLogKey;
                    Logger.WriteLog(out outLogKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                    throw;
                }
            }
        }

        public static string EcmTempFolder
        {

            get
            {
                try
                {
                    return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.EcmTempFolder).ConfigValue;
                }
                catch (Exception exception)
                {
                    string outLogKey;
                    Logger.WriteLog(out outLogKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                    throw;
                }
            }
        }
    }
}