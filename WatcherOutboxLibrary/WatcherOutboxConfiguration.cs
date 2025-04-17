using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Shared.Utility;

namespace WatcherOutboxLibrary
{
    public class WatcherOutboxConfiguration
    {
        public static string DataFolderWatcherOutboxService
        {
            get
            {
                var toRet =
                    GeneralConfigs.GetByConfigKey(
                        EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.DataFolderWatcherOutboxService));
                return toRet.ConfigValue;
            }
        }

        public static string UserNameWatcherOutboxService
        {
            get
            {
                var toRet =
                    GeneralConfigs.GetByConfigKey(
                        EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.USERNAMEWatcherOutboxService));
                return toRet.ConfigValue;
            }
        }

        public static string PasswordWatcherOutboxService
        {
            get
            {
                var toRet =
                    GeneralConfigs.GetByConfigKey(
                        EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.PASSWORDWatcherOutboxService));
                return toRet.ConfigValue;
            }
        }

        public static string ServerAddessWatcherOutboxService
        {
            get
            {
                var toRet =
                    GeneralConfigs.GetByConfigKey(
                        EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.SERVER_ADDRESSWatcherOutboxService));
                return toRet.ConfigValue;
            }
        }

        public const string PrefixUploadPpnCredit = "UploadPPNCredit";


        public static int MaxCopyWatcherOutboxService
        {
            get
            {
                return int.Parse(GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.MaxCopyWatcherOutboxService).ConfigValue);
            }
        }

        public static string DestinationFolderWatcherOutboxService
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DestinationFolderWatcherOutboxService).ConfigValue;
            }
        }

        public static string WaktujedaWatcherOutboxService
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.waktujedaWatcherOutboxService).ConfigValue;
            }
        }

        public static string LogFolderWatcherOutboxService
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.LogFolderWatcherOutboxService).ConfigValue;
            }
        }

        public static string FileExtWatcherOutboxService
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.FileExtWatcherOutboxService).ConfigValue;
            }
        }

        public static string DelimiterWatcherOutboxService
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DelimiterWatcherOutboxService).ConfigValue;
            }
        }

        public static string ClientSettingsProviderServiceUriWatcherOutboxService
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.ClientSettingsProviderServiceUriWatcherOutboxService).ConfigValue;
            }
        }

        public static string TimeSleepWatcherOutboxService
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.TimeSleepWatcherOutboxService).ConfigValue;
            }
        }

        //public static string BackupFolderWatcherOutboxService
        //{
        //    get
        //    {
        //        return GeneralConfigs.GetById((int) ApplicationEnums.GeneralConfig.BackupFolderWatcherOutboxService).ConfigValue;
        //    }
        //}

        //public static string SapServerAddressWatcherOutboxService
        //{
        //    get
        //    {
        //        return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.SAPSERVER_ADDRESSWatcherOutboxService).ConfigValue;
        //    }
        //}

    }
}
