using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;

namespace eFakturADM.Logic.Objects
{
    /// <summary>
    /// Provides a wrapper on single item in the Location database table. The properties of this class mapped on appropriate database fields and methods provide saving and loading into/from database.
    /// An instance of this class can be created by new word or loaded from a database using Location class which returns collection of Location by different condition.  
    /// </summary>     
    public partial class Role : ApplicationObject, IApplicationObject
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Loads result set field values and saves into properties of class.
        /// </summary>
        /// <param name="dr">DataReader object which represents current row in the resultset.</param>
        /// <returns>Returns true if it was successfully loaded.</returns>
        public virtual bool Load(IDataReader dr)
        {
            this.IsValid = false;

            this.RoleId = DBUtil.GetIntField(dr, "RoleId");
            this.Name = DBUtil.GetCharField(dr, "Name");
            this.IsDeleted = DBUtil.GetBoolField(dr, "IsDeleted");
            this.Created = DBUtil.GetDateTimeField(dr, "Created");
            this.Modified = DBUtil.GetDateTimeField(dr, "Modified");
            this.CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            this.ModifiedBy = DBUtil.GetCharField(dr, "ModifiedBy");

            this.IsValid = true;
            return this.IsValid;
        }

        /// <summary>
        /// Insert or update a Role in database according to class properties.
        /// </summary>
        /// <returns>Returns true if it was successfully saved.</returns>
        public virtual bool Save()
        {
            this.WasSaved = false;
            SpBase sp = null;

            if (this.RoleId > 0)
            {
                sp = new SpBase(@"
                        UPDATE dbo.Role SET                                
                                Name            = @Name,                      
                                Modified        = @Modified,
                                ModifiedBy      = @ModifiedBy
                                
                        WHERE RoleId = @RoleId
                    ");

                sp.AddParameter("RoleId", RoleId);           
            }
            else
            {
                sp = new SpBase(@"
                          INSERT INTO dbo.Role (Name, CreatedBy, ModifiedBy) 
                               VALUES(@Name, @CreatedBy, @ModifiedBy); SELECT @RoleId = @@IDENTITY");

                sp.AddParameter("RoleId", RoleId, System.Data.ParameterDirection.Output);
            }

            sp.AddParameter("Name", Name);
            sp.AddParameter("CreatedBy", CreatedBy);
            sp.AddParameter("ModifiedBy", ModifiedBy);
            sp.AddParameter("Modified", DateTime.Now);

            if (sp.ExecuteNonQuery() == 0)
                this.WasSaved = true;

            if (this.RoleId <= 0)
            {
                RoleId = (int)sp.GetParameter("RoleId");
            }

            return this.WasSaved;
        }

        /// <summary>
        /// Delete Role by update IsDeleted to 1 (Soft Delete).
        /// </summary>
        /// <returns>Returns true if it was successfully deleted.</returns>
        public virtual bool Delete()
        {
            this.WasDeleted = false;

            SpBase sp = new SpBase(String.Format(@"UPDATE dbo.Role SET IsDeleted = 1, Modified = @Modified, ModifiedBy = @ModifiedBy WHERE RoleId = @RoleId;", Table));
            sp.AddParameter("RoleId", RoleId);
            sp.AddParameter("ModifiedBy", ModifiedBy);
            sp.AddParameter("Modified", DateTime.Now);

            if (sp.ExecuteNonQuery() == 0)
                this.WasDeleted = true;

            return this.WasDeleted;
        }
    }
}
