using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace eFakturADM.Logic.Collections
{
    public class ConfigCollection : ApplicationCollection<CompEvisSap, SpBase>
    {
   
        public static SettingViewModel.FileshareSetting GetFileShareSetting()
        {

            SettingViewModel.FileshareSetting fileshare = new SettingViewModel.FileshareSetting();

            fileshare.FolderUploadTemp = GeneralConfigs.GetByConfigKey(EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.EvisShareFolderTemporaryPath)).ConfigValue;
            fileshare.FolderUploadTempLocalServer = GeneralConfigs.GetByConfigKey(EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.EvisShareFolderTemporaryPathLocalServer)).ConfigValue;

            fileshare.FolderUpload = GeneralConfigs.GetByConfigKey(EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.EvisShareFolderRootPath)).ConfigValue;
            fileshare.FolderUploadLocalServer = GeneralConfigs.GetByConfigKey(EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.EvisShareFolderRootPathLocalServer)).ConfigValue;
            fileshare.AllowedExtension = GeneralConfigs.GetByConfigKey(EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.AllowExtension)).ConfigValue;
            try
            {
                fileshare.MaxUploadSize = int.Parse(GeneralConfigs.GetByConfigKey(EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.MaxUploadSize)).ConfigValue);
            }
            catch (Exception ex)
            {
                fileshare.MaxUploadSize = 1; /*Size in MB*/

                string outlogkey;
                Logger.WriteLog(out outlogkey, LogLevel.Error, "Application Setting File Share Setting [fileshare-max-upload-size] not valid. Using default setting 1 MB", MethodBase.GetCurrentMethod(), ex);

            }

            return fileshare;
        }


    }
}
