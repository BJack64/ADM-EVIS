using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;
using System.Data;

namespace eFakturADM.Logic.Objects
{

    public partial class GeneralConfig
    {
        public int GeneralConfigId { get; set; }
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
        public string ConfigExtra { get; set; }
        public string ConfigDesc { get; set; }
    }

    public partial class GeneralConfig : ApplicationObject, IApplicationObject
    {
        
        /// <summary>
        /// Loads result set field values and saves into properties of class.
        /// </summary>
        /// <param name="dr">DataReader object which represents current row in the resultset.</param>
        /// <returns>Returns true if it was successfully loaded.</returns>
        public virtual bool Load(IDataReader dr)
        {
            IsValid = false;

            GeneralConfigId = DBUtil.GetIntField(dr, "GeneralConfigId");
            ConfigKey = DBUtil.GetCharField(dr, "ConfigKey");
            ConfigValue = DBUtil.GetCharField(dr, "ConfigValue");
            ConfigExtra = DBUtil.GetCharField(dr, "ConfigExtra");
            ConfigDesc = DBUtil.GetCharField(dr, "ConfigDesc");

            IsValid = true;
            return IsValid;
        }
    }

}
