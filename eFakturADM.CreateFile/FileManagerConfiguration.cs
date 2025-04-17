using System;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Shared.Utility;

namespace eFakturADM.FileManager
{
    public class FileManagerConfiguration
    {

        public static string RepoDomain
        {
            get
            {
                var toRet =
                    GeneralConfigs.GetByConfigKey(
                        EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.EvisShareFolderDomain));
                return toRet.ConfigValue;
            }
        }

        public static string RepoRootPath
        {
            get
            {
                var toRet =
                    GeneralConfigs.GetByConfigKey(
                        EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.EvisShareFolderRootPath));
                return toRet.ConfigValue;
            }
        }

        public static string RepoTempPath
        {
            get
            {
                var toRet =
                    GeneralConfigs.GetByConfigKey(
                        EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.EvisShareFolderTemporaryPath));
                return toRet.ConfigValue;
            }
        }

        public static string RepoUser
        {
            get
            {
                var toRet =
                    GeneralConfigs.GetByConfigKey(
                        EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.EvisShareFolderUser));
                return toRet.ConfigValue;
            }
        }

        public static string RepoPassword
        {
            get
            {
                var toRet =
                    GeneralConfigs.GetByConfigKey(
                        EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.EvisShareFolderPassword));
                return toRet.ConfigValue;
            }
        }

        public static bool RepoIsSameDomain
        {
            get
            {
                var toRet =
                    GeneralConfigs.GetByConfigKey(
                        EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.EvisShareFolderIsSameDomain));
                return Convert.ToBoolean(toRet.ConfigValue);
            }
        }

        public static string CompanyCode
        {
            get
            {
                var toRet =
                    GeneralConfigs.GetByConfigKey(
                        EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.CompanyCode));
                return toRet.ConfigValue;
            }
        }

    }

    public class FileManagerSapConfiguration
    {
        public static string DataFolderWatcherInboxService
        {
            get
            {
                var toRet =
                    GeneralConfigs.GetByConfigKey(
                        EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.DataFolderWatcherInboxService));
                return toRet.ConfigValue;
            }
        }

        public static string UserNameWatcherInboxService
        {
            get
            {
                var toRet =
                    GeneralConfigs.GetByConfigKey(
                        EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.USERNAMEWatcherInboxService));
                return toRet.ConfigValue;
            }
        }

        public static string PasswordWatcherInboxService
        {
            get
            {
                var toRet =
                    GeneralConfigs.GetByConfigKey(
                        EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.PASSWORDWatcherInboxService));
                return toRet.ConfigValue;
            }
        }

        public static string ServerAddessWatcherInboxService
        {
            get
            {
                var toRet =
                    GeneralConfigs.GetByConfigKey(
                        EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.SERVER_ADDRESSWatcherInboxService));
                return toRet.ConfigValue;
            }
        }

        public static string DataFolder2WatcherInboxService
        {
            get
            {
                return GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DataFolder2WatcherInboxService).ConfigValue;
            }
        }

        public const string PrefixUploadPpnCredit = "UploadPPNCredit";
        public const string PrefixOutUploadPpnCredit = "outUploadPPNCredit";

    }

}
