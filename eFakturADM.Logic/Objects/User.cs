using System.Data.SqlTypes;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;
using System;
using System.Data;

namespace eFakturADM.Logic.Objects
{
    public partial class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public string UserRole { get; set; }
        public string DecryptedPassword { get; set; }
        public bool ResetPassword { get; set; }
        public int BadPasswordCount { get; set; }
        public string UserInitial { get; set; }
    }
    /// <summary>
    /// Provides a wrapper on single item in the Location database table. The properties of this class mapped on appropriate database fields and methods provide saving and loading into/from database.
    /// An instance of this class can be created by new word or loaded from a database using Location class which returns collection of Location by different condition.  
    /// </summary>     
    public partial class User : ApplicationObject, IApplicationObject
    {
        /// <summary>
        /// Loads result set field values and saves into properties of class.
        /// </summary>
        /// <param name="dr">DataReader object which represents current row in the resultset.</param>
        /// <returns>Returns true if it was successfully loaded.</returns>
        public virtual bool Load(IDataReader dr)
        {
            IsValid = false;

            UserId = DBUtil.GetIntField(dr, "UserId");
            UserName = DBUtil.GetCharField(dr, "UserName");
            Password = DBUtil.GetCharField(dr, "Password");
            Email = DBUtil.GetCharField(dr, "Email");
            IsDeleted = DBUtil.GetBoolField(dr, "IsDeleted");
            Created = DBUtil.GetDateTimeField(dr, "Created");
            Modified = DBUtil.GetDateTimeField(dr, "Modified");
            CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            ModifiedBy = DBUtil.GetCharField(dr, "ModifiedBy");

            UserRole = DBUtil.GetCharField(dr, "UserRole");

            ResetPassword = DBUtil.GetBoolField(dr, "ResetPassword");
            var badPwdCnt = DBUtil.GetIntNullField(dr, "BadPasswordCount");
            BadPasswordCount = badPwdCnt.HasValue ? badPwdCnt.Value : 0;

            UserInitial = DBUtil.GetCharField(dr, "UserInitial");

            DecryptedPassword = Rijndael.Decrypt(Password);

            IsValid = true;
            return IsValid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool Save()
        {
            WasSaved = false;
            SpBase sp;

            if (UserId > 0)
            {
                sp = new SpBase(@"UPDATE [dbo].[User]
                               SET [Email] = @Email
                                  ,[UserInitial] = @UserInitial
                                  ,[Modified] = GETDATE()
                                  ,[ModifiedBy] = @ModifiedBy
                             WHERE UserId = @UserId");
                sp.AddParameter("UserId", UserId);
                sp.AddParameter("ModifiedBy", ModifiedBy);
            }
            else
            {
                sp = new SpBase(@"INSERT INTO [dbo].[User]([Username],[Password],[Email],[CreatedBy], [UserInitial])
                                    VALUES(@Username,@Password,@Email,@CreatedBy, @UserInitial); SELECT @UserId = @@IDENTITY");

                sp.AddParameter("UserId", UserId, ParameterDirection.Output);
                sp.AddParameter("UserName", UserName);
                sp.AddParameter("Password", Rijndael.Encrypt(Password));
                sp.AddParameter("CreatedBy", CreatedBy);
            }

            sp.AddParameter("Email", string.IsNullOrEmpty(Email) ? SqlString.Null : Email);
            sp.AddParameter("UserInitial", string.IsNullOrEmpty(UserInitial) ? SqlString.Null : UserInitial);

            if (sp.ExecuteNonQuery() == 0)
                WasSaved = true;

            if (UserId <= 0)
            {
                UserId = (int)sp.GetParameter("UserId");
            }

            return WasSaved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool ChangePassword()
        {
            WasSaved = false;
            SpBase sp;

            sp = new SpBase(@"UPDATE [dbo].[User]
                               SET [Password] = @Password
                                  ,[Modified] = GETDATE()
                                  ,[ModifiedBy] = @ModifiedBy
                             WHERE UserId = @UserId");
            sp.AddParameter("UserId", UserId);
            sp.AddParameter("Password", Rijndael.Encrypt(Password));
            sp.AddParameter("ModifiedBy", ModifiedBy);

            if (sp.ExecuteNonQuery() == 0)
                WasSaved = true;

            if (UserId <= 0)
            {
                UserId = (int)sp.GetParameter("UserId");
            }

            return WasSaved;
        }

        /// <summary>
        /// Delete User by update IsDeleted to 1 (Soft Delete).
        /// </summary>
        /// <returns>Returns true if it was successfully deleted.</returns>
        public virtual bool Delete()
        {
            WasDeleted = false;

            SpBase sp = new SpBase(String.Format(@"UPDATE dbo.[User] SET IsDeleted = 1, Modified = @Modified, ModifiedBy = @ModifiedBy WHERE UserId = @UserId;", Table));
            sp.AddParameter("UserId", UserId);
            sp.AddParameter("ModifiedBy", ModifiedBy);
            sp.AddParameter("Modified", DateTime.Now);

            if (sp.ExecuteNonQuery() == 0)
                WasDeleted = true;

            return WasDeleted;
        }

        public virtual bool UpdateResetPassword()
        {
            var sp = new SpBase(@"UPDATE [dbo].[User]
SET     [ResetPassword]  = @ResetPassword,
        Modified = GETDATE(), 
        ModifiedBy = @ModifiedBy 
WHERE UserID = @UserID");

            sp.AddParameter("ResetPassword", ResetPassword);
            sp.AddParameter("ModifiedBy", ModifiedBy);
            sp.AddParameter("UserID", UserId);

            if (sp.ExecuteNonQuery() == 0)
                this.WasSaved = true;

            return this.WasSaved;
        }

        public virtual bool UpdateBadPasswordCount(int newBadPwdCount)
        {

            WasSaved = false;
            var sp =
                new SpBase(
                    @"UPDATE dbo.[User] SET BadPasswordCount = @BadPasswordCount WHERE UserName = @UserName AND IsDeleted = 0");

            sp.AddParameter("BadPasswordCount", newBadPwdCount);
            sp.AddParameter("UserName", UserName);
            if (sp.ExecuteNonQuery() == 0)
                WasSaved = true;

            return WasSaved;

        }

    }
}
