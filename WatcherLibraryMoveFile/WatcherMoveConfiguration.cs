using System.Configuration;
using System.Linq;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Utilities;
using eFakturADM.Logic.Core;

namespace WatcherLibraryMoveFile
{
    public class WatcherMoveConfiguration
    {
        public static string FileExt
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.FileExtWatcherInboxService).ConfigValue;
            }
        }

        public static string DataFolder
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DataFolderWatcherInboxService).ConfigValue;
            }
        }

        public static string UserName
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.USERNAMEWatcherInboxService).ConfigValue;
            }
        }

        public static string ServerAddress
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.SERVER_ADDRESSWatcherInboxService).ConfigValue;
            }
        }

        public static string Password
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PASSWORDWatcherInboxService).ConfigValue;
            }
        }

        public static string ErrorInbox
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.ErrorInboxWatcherInboxService).ConfigValue;
            }
        }

        public static string DataFolder2
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DataFolder2WatcherInboxService).ConfigValue;
            }
        }

        public static int TimeSleep
        {
            get
            {
                return int.Parse(GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.TimeSleepWatcherInboxService).ConfigValue);
            }
        }

        public static string LogFolder
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.LogFolderWatcherInboxService).ConfigValue;
            }
        }

        public static string Delimiter
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DelimiterWatcherInboxService).ConfigValue;
            }
        }

        public static string ClientSettingsProviderServiceUri
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.ClientSettingsProviderServiceUriWatcherInboxService).ConfigValue;
            }
        }

        public static int TimeScheduler
        {
            get
            {
                return int.Parse(GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.TimeSchedulerWatcherInboxService).ConfigValue);
            }
        }

        public static int MaxCopy
        {
            get
            {
                return int.Parse(GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.MaxCopyWatcherInboxService).ConfigValue);
            }
        }

        public static string BackupFolderWatcherInboxService
        {
            get
            {
                return
                    GeneralConfigs.GetById((int) ApplicationEnums.GeneralConfig.BackupFolderWatcherInboxService)
                        .ConfigValue;
            }
        }

    }
}
