using System;
using System.Reflection;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Shared.Utility;

namespace eFakturADM.Shared.TestConsole
{
    public class ConsoleConfiguration
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
    }
}
