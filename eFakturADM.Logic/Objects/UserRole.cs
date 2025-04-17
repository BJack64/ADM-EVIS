using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;
using System;
using System.Data;

namespace eFakturADM.Logic.Objects
{
    public class UserRole : ApplicationObject, IApplicationObject
    {   	
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string Modified_String { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }

        /// <summary>
        /// Loads result set field values and saves into properties of class.
        /// </summary>
        /// <param name="dr">DataReader object which represents current row in the resultset.</param>
        /// <returns>Returns true if it was successfully loaded.</returns>
        public virtual bool Load(IDataReader dr)
        {
            this.IsValid = false;
            this.UserRoleId = DBUtil.GetIntField(dr, "UserRoleId");
            this.UserId = DBUtil.GetIntField(dr, "UserId");
            this.RoleId = DBUtil.GetIntField(dr, "RoleId");

            this.IsDeleted = DBUtil.GetBoolField(dr, "IsDeleted");
            this.Created = DBUtil.GetDateTimeField(dr, "Created");
            this.Modified = DBUtil.GetDateTimeField(dr, "Modified");
            this.CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            this.ModifiedBy = DBUtil.GetCharField(dr, "ModifiedBy");

            this.IsValid = true;
            return this.IsValid;
        }

        /// <summary>
        /// Insert or update a task in database according to class properties.
        /// </summary>
        /// <returns>Returns true if it was successfully saved.</returns>
        public virtual bool Save()
        {
            this.WasSaved = false;
            SpBase sp = null;

            if (UserRoleId > 0)
            {

            }
            else
            {
                sp = new SpBase(@"INSERT INTO UserRole (UserId, RoleId, CreatedBy, ModifiedBy) 
                                    VALUES(@UserId, @RoleId, @CreatedBy, @ModifiedBy); SELECT @UserRoleId = @@IDENTITY");

                sp.AddParameter("UserRoleId", UserRoleId, System.Data.ParameterDirection.Output);
                sp.AddParameter("UserId", UserId);
                sp.AddParameter("RoleId", RoleId);
                sp.AddParameter("CreatedBy", CreatedBy);
                sp.AddParameter("ModifiedBy", ModifiedBy);
                sp.AddParameter("Modified", DateTime.Now);
            }

            if (sp.ExecuteNonQuery() == 0)
                this.WasSaved = true;

            if (this.UserRoleId <= 0)
            {
                UserRoleId = (int)sp.GetParameter("UserRoleId");
            }

            return this.WasSaved;
        }

        /// <summary>
        /// Delete role activity by update IsDeleted to 1 (Soft Delete).
        /// </summary>
        /// <returns>Returns true if it was successfully deleted.</returns>
        public virtual bool Delete()
        {
            this.WasDeleted = false;

            SpBase sp = new SpBase(String.Format(@"UPDATE UserRole SET IsDeleted = 1, ModifiedBy = @ModifiedBy, Modified = @Modified WHERE UserRoleId = @UserRoleId;"));
            sp.AddParameter("UserRoleId", UserRoleId);
            sp.AddParameter("ModifiedBy", ModifiedBy);
            sp.AddParameter("Modified", DateTime.Now);

            if (sp.ExecuteNonQuery() == 0)
                this.WasDeleted = true;

            return this.WasDeleted;
        }

        /// <summary>
        /// Delete role activity by update IsDeleted to 1 (Soft Delete).
        /// </summary>
        /// <returns>Returns true if it was successfully deleted.</returns>
        public virtual bool DeleteByUserId()
        {
            this.WasDeleted = false;

            SpBase sp = new SpBase(String.Format(@"UPDATE UserRole SET IsDeleted = 1, ModifiedBy = @ModifiedBy, Modified = @Modified WHERE UserId = @UserId;"));
            sp.AddParameter("UserId", UserId);
            sp.AddParameter("ModifiedBy", ModifiedBy);
            sp.AddParameter("Modified", DateTime.Now);

            if (sp.ExecuteNonQuery() == 0)
                this.WasDeleted = true;

            return this.WasDeleted;
        }
    }
}
