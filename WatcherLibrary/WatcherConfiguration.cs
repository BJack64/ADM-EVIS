using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;

namespace WatcherLibrary
{
    public class WatcherConfiguration
    {
        public static string FileExt
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.FileExtWatcherService).ConfigValue;
                //return ConfigurationManager.AppSettings["FileExt"];
            }
        }

        public static string DataFolder
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DataFolderWatcherService).ConfigValue;
                //return ConfigurationManager.AppSettings["DataFolder"];
            }
        }

        public static string ResultFolder
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.ResultFolderWatcherService).ConfigValue;
                //return ConfigurationManager.AppSettings["ResultFolder"];
            }
        }

        public static string LogFolder
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.LogFolderWatcherService).ConfigValue;
                //return ConfigurationManager.AppSettings["LogFolder"];
            }
        }

        public static string Delimiter
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DelimiterWatcherService).ConfigValue;
                //return ConfigurationManager.AppSettings["Delimiter"];
            }
        }

        public static int TimeSleep
        {
            get
            {
                return int.Parse(GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.TimeSleepWatcherService).ConfigValue);
                //return int.Parse(ConfigurationManager.AppSettings["TimeSleep"]);
            }
        }

        public static string TimeScheduler
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.TimeSchedulerWatcherService).ConfigValue;
                //return ConfigurationManager.AppSettings["TimeScheduler"];
            }
        }

        public static string ClientSettingsProviderServiceUri
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.ClientSettingsProviderServiceUriWatcherService).ConfigValue;
            }
        }

        public static string MaxProcessFiles
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.MaxProcessFilesCompareEvisVsSap).ConfigValue;
            }
        }

    }
}
