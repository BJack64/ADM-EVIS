using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace eFakturADM.Logic.Objects
{
    /// <summary>
    /// Provides a wrapper on single item in the Location database table. The properties of this class mapped on appropriate database fields and methods provide saving and loading into/from database.
    /// An instance of this class can be created by new word or loaded from a database using Location class which returns collection of Location by different condition.  
    /// </summary>     
    public partial class Module : ApplicationObject, IApplicationObject
    {      
        public int ModuleId { get; set; }
        public string Name { get; set; }
        public int? ModuleIdParent { get; set; }
        public string IconUrl { get; set; }
        public string IconHoverUrl { get; set; }
        public string Url { get; set; }

        /// <summary>
        /// Loads result set field values and saves into properties of class.
        /// </summary>
        /// <param name="dr">DataReader object which represents current row in the resultset.</param>
        /// <returns>Returns true if it was successfully loaded.</returns>
        public virtual bool Load(IDataReader dr)
        {
            this.IsValid = false;

            this.ModuleId = DBUtil.GetIntField(dr, "ModuleId");
            this.Name = DBUtil.GetCharField(dr, "Name");
            this.ModuleIdParent = DBUtil.GetIntNullField(dr, "ModuleIdParent");
            this.IconUrl = DBUtil.GetCharField(dr, "IconUrl");
            this.IconHoverUrl = DBUtil.GetCharField(dr, "IconHoverUrl");
            this.Url = DBUtil.GetCharField(dr, "Url");

            this.IsValid = true;
            return this.IsValid;            
        }
    }
}
