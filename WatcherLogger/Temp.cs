using System.Configuration;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;

namespace WatcherLogger
{
    public static class Temp
    {
        static ConnectionStringSettings _SQLServerConnectionStr = new ConnectionStringSettings("SQLServerConnectionStr", ConfigurationManager.AppSettings["eFakturADM.Connection.String"], "System.Data.SqlClient");
        public static ConnectionStringSettings SQLServerConnectionStr
        {
            get
            {
                return _SQLServerConnectionStr;
            }
            set
            {
                _SQLServerConnectionStr = value;
            }
        }

        public static string LogFolder
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.LogFolderWatcherService).ConfigValue;
            }
        }
        
    }
}
