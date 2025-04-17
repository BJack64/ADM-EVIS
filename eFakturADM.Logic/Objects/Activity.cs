using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;
using System.Data;

namespace eFakturADM.Logic.Objects
{
    public partial class Activity : ApplicationObject, IApplicationObject
    {   	
        public int ActivityId { get; set; }
        public int ModuleId { get; set; }
        public string ActivityName { get; set; }   

        /// <summary>
        /// Loads result set field values and saves into properties of class.
        /// </summary>
        /// <param name="dr">DataReader object which represents current row in the resultset.</param>
        /// <returns>Returns true if it was successfully loaded.</returns>
        public virtual bool Load(IDataReader dr)
        {
            this.IsValid = false;

            this.ActivityId = DBUtil.GetIntField(dr, "ActivityId");
            this.ModuleId = DBUtil.GetIntField(dr, "ModuleId");
            this.ActivityName = DBUtil.GetCharField(dr, "ActivityName");

            this.IsValid = true;
            return this.IsValid;
        }
    }
}
