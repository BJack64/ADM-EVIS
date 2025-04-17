using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eFakturADM.Logic.Collections
{
    public class UserRoles : ApplicationCollection<UserRole, SpBase>
    {
        /// <summary>
        /// Gets UserRole object instance by unique identifier
        /// </summary>
        /// <param name="UserRoleId"></param>
        /// <returns>Instance of Module object.</returns>
        public static UserRole GetById(int UserRoleId)
        {
            SpBase sp = new SpBase(@"SELECT UserRole.*
                                    FROM UserRole
                                    WHERE UserRoleID = @UserRoleId");

            sp.AddParameter("UserRoleId", UserRoleId);
            return GetApplicationObject(sp);
        }

        /// <summary>
        /// Gets UserRole List instance by UserId
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>Instance of UserRole object.</returns>
        public static List<UserRole> GetByUserId(long UserId)
        {
            SpBase sp = new SpBase(@"SELECT UserRole.*  
                                    FROM UserRole
                                    WHERE UserId = @UserId  AND UserRole.IsDeleted = 0");
            sp.AddParameter("UserId", UserId);
            return GetApplicationCollection(sp);
        }    
    }
}
