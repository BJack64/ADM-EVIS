using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;

namespace eFakturADM.Logic.Collections
{
    public class GeneralConfigs : ApplicationCollection<GeneralConfig, SpBase>
    {
        public static List<GeneralConfig> GetAll()
        {
            var sp = new SpBase(@"SELECT [GeneralConfigId],[ConfigKey],[ConfigValue],[ConfigExtra],[ConfigDesc]
FROM [GeneralConfig]");
            return GetApplicationCollection(sp);
        }
        public static GeneralConfig GetById(int eGeneralConfig)
        {
            var sp = new SpBase(@"SELECT [GeneralConfigId],[ConfigKey],[ConfigValue],[ConfigExtra],[ConfigDesc]
FROM [GeneralConfig]
WHERE [GeneralConfigId] = @GeneralConfigId");
            sp.AddParameter("GeneralConfigId", eGeneralConfig);
            var dbData = GetApplicationObject(sp);

            return dbData == null || dbData.GeneralConfigId == 0 ? null : dbData;

        }

        public static GeneralConfig GetByConfigKey(string ConfigKey)
        {
            var sp = new SpBase(@"SELECT [GeneralConfigId],[ConfigKey],[ConfigValue],[ConfigExtra],[ConfigDesc]
FROM [GeneralConfig]
WHERE [ConfigKey] = @ConfigKey");
            sp.AddParameter("ConfigKey", ConfigKey);
            var dbData = GetApplicationObject(sp);
            return dbData == null || dbData.GeneralConfigId == 0 ? null : dbData;
        }

        public static GeneralConfig GetConfigCheckNpwpAdm(string npwpToCheck)
        {
            var sp = new SpBase(@"SELECT [GeneralConfigId],[ConfigKey],[ConfigValue],[ConfigExtra],[ConfigDesc]
FROM [GeneralConfig]
WHERE GeneralConfigId = @configId AND LOWER(ConfigValue) = LOWER(dbo.FormatNpwp(@configValueCheck))");

            sp.AddParameter("configId", (int) ApplicationEnums.GeneralConfig.NpwpAdm);
            sp.AddParameter("configValueCheck", npwpToCheck);

            var dbData = GetApplicationObject(sp);
            return dbData == null || dbData.GeneralConfigId == 0 ? null : dbData;
        }

        public static GeneralConfig GetConfigCheckValue(ApplicationEnums.GeneralConfig eConfigKey, string npwpToCheck)
        {
            var sp = new SpBase(@"SELECT [GeneralConfigId],[ConfigKey],[ConfigValue],[ConfigExtra],[ConfigDesc]
FROM [GeneralConfig]
WHERE GeneralConfigId = @configId AND LOWER(ConfigValue) = LOWER(@configValueCheck)");

            sp.AddParameter("configId", (int)eConfigKey);
            sp.AddParameter("configValueCheck", npwpToCheck);

            var dbData = GetApplicationObject(sp);
            return dbData == null || dbData.GeneralConfigId == 0 ? null : dbData;
        }

        public static GeneralConfig GetByKeyId(ApplicationEnums.GeneralConfig eConfigKey)
        {
            var sp = new SpBase(@"SELECT [GeneralConfigId],[ConfigKey],[ConfigValue],[ConfigExtra],[ConfigDesc]
FROM [GeneralConfig]
WHERE GeneralConfigId = @configId");
            sp.AddParameter("configId", (int)eConfigKey);

            var dbData = GetApplicationObject(sp);
            return dbData == null || dbData.GeneralConfigId == 0 ? null : dbData;
        }


        public static GeneralConfig GetConfigStaticToken(string Token)
        {
            var sp = new SpBase(@"SELECT [GeneralConfigId],[ConfigKey],[ConfigValue],[ConfigExtra],[ConfigDesc]
FROM [GeneralConfig]
WHERE ConfigExtra = @ConfigExtra AND ConfigKey = @ConfigKey");

            sp.AddParameter("ConfigKey", EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.StaticApiToken));
            sp.AddParameter("ConfigExtra", Token);

            var dbData = GetApplicationObject(sp);
            return dbData == null || dbData.GeneralConfigId == 0 ? null : dbData;
        }

    }
}
