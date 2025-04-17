using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using eFakturADM.Logic.Collections;


namespace eFakturADM.Logic.Objects
{

    public partial class RoleActivity : ApplicationObject, IApplicationObject
    {
        public int RoleActivityId { get; set; }
        public int RoleId { get; set; }
        public int ActivityId { get; set; }       
        public bool IsDeleted { get; set; }       
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }


        public string RoleName { get; set; }
        public string ActivityName { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }

        /// <summary>
        /// Loads result set field values and saves into properties of class.
        /// </summary>
        /// <param name="dr">DataReader object which represents current row in the resultset.</param>
        /// <returns>Returns true if it was successfully loaded.</returns>
        public virtual bool Load(IDataReader dr)
        {
            this.IsValid = false;

            this.RoleActivityId = DBUtil.GetIntField(dr, "RoleActivityId");
            this.RoleId = DBUtil.GetIntField(dr, "RoleId");
            this.ActivityId = DBUtil.GetIntField(dr, "ActivityId");
            this.IsDeleted = DBUtil.GetBoolField(dr, "IsDeleted");
            this.Created = DBUtil.GetDateTimeField(dr, "Created");
            this.Modified = DBUtil.GetDateTimeField(dr, "Modified");
            this.CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            this.ModifiedBy = DBUtil.GetCharField(dr, "ModifiedBy");

            this.RoleName = DBUtil.GetCharField(dr, "RoleName");
            this.ActivityName = DBUtil.GetCharField(dr, "ActivityName");
            this.ModuleId = DBUtil.GetIntField(dr, "ModuleId");
            this.ModuleName = DBUtil.GetCharField(dr, "ModuleName");

            this.IsValid = true;
            return this.IsValid;
        }

        /// <summary>
        /// Insert or update a RoleActivity in database according to class properties.
        /// </summary>
        /// <returns>Returns true if it was successfully saved.</returns>
        public virtual bool Save()
        {
            this.WasSaved = false;
            SpBase sp = null;

            if (RoleActivityId > 0)
            {
                sp = new SpBase(@"
                        UPDATE dbo.RoleActivity SET 
                                RoleId              = @RoleId, 
                                ActivityId          = @ActivityId,                               
                                Modified            = @Modified,
                                ModifiedBy          = @ModifiedBy
                        WHERE RoleId = @RoleId AND ActivityId = @ActivityId
                    ");

            }
            else
            {
                sp = new SpBase(@"
                        INSERT INTO dbo.RoleActivity (RoleId, ActivityId, CreatedBy, ModifiedBy) 
                                    VALUES(@RoleId, @ActivityId, @CreatedBy, @ModifiedBy); SELECT @RoleActivityId = @@IDENTITY");

                sp.AddParameter("RoleActivityId", RoleActivityId, System.Data.ParameterDirection.Output);              
            }
            sp.AddParameter("RoleId", RoleId);
            sp.AddParameter("ActivityId", ActivityId);               
            sp.AddParameter("CreatedBy", CreatedBy);
            sp.AddParameter("ModifiedBy", ModifiedBy);
            sp.AddParameter("Modified", DateTime.Now);

            if (sp.ExecuteNonQuery() == 0)
                this.WasSaved = true;

            if (this.RoleActivityId <= 0)
            {
                RoleActivityId = (int)sp.GetParameter("RoleActivityId");
            }

            return this.WasSaved;
        }       

        /// <summary>
        /// Delete RoleActivity by update IsDeleted to 1 (Soft Delete).
        /// </summary>
        /// <returns>Returns true if it was successfully deleted.</returns>
        public virtual bool Delete()
        {
            this.WasDeleted = false;

            SpBase sp = new SpBase(@"UPDATE dbo.RoleActivity SET IsDeleted = 1, ModifiedBy = @ModifiedBy, Modified = @Modified WHERE RoleActivityId = @RoleActivityId;");
            sp.AddParameter("RoleActivityId", RoleActivityId);
            sp.AddParameter("ModifiedBy", ModifiedBy);
            sp.AddParameter("Modified", DateTime.Now);

            if (sp.ExecuteNonQuery() == 0)
                this.WasDeleted = true;

            return this.WasDeleted;
        }

        /// <summary>
        /// Delete RoleActivity by update IsDeleted to 1 (Soft Delete) by RoleId.
        /// </summary>
        /// <returns>Returns true if it was successfully deleted.</returns>
        public virtual bool DeleteByRoleId()
        {
            this.WasDeleted = false;

            SpBase sp = new SpBase(@"UPDATE dbo.RoleActivity SET IsDeleted = 1, ModifiedBy = @ModifiedBy, Modified = @Modified WHERE RoleId = @RoleId;");
            sp.AddParameter("RoleId", RoleId);
            sp.AddParameter("ModifiedBy", ModifiedBy);
            sp.AddParameter("Modified", DateTime.Now);

            if (sp.ExecuteNonQuery() == 0)
                this.WasDeleted = true;

            return this.WasDeleted;
        }
    }
}
