using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eFakturADM.Logic.Collections
{
    public class Roles : ApplicationCollection<Role, SpBase>
    {
        /// <summary>
        /// Gets Module object instance by unique identifier
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <returns>Instance of Role object.</returns>
        public static Role GetById(int RoleId)
        {
            SpBase sp = new SpBase(@"SELECT 
                                                        Role.*
                                                    FROM 
                                                        Role
                                                    WHERE RoleId = @RoleId");
            sp.AddParameter("RoleId", RoleId);
            return GetApplicationObject(sp);
        }

        /// <summary>
        /// Gets Role object instance by name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns>Instance of Role object.</returns>
        public static List<Role> GetByName(string Name)
        {
            SpBase sp = new SpBase("SELECT * FROM Role WHERE Name = @Name AND IsDeleted = 0");
            sp.AddParameter("Name", Name);
            return GetApplicationCollection(sp);
        }

        /// <summary>
        /// Gets collection of all Role in the database.
        /// </summary>
        /// <returns>List of Role object instances.</returns>
        public static List<Role> Get()
        {
            SpBase sp = new SpBase("SELECT * FROM Role WHERE IsDeleted = 0");
            return GetApplicationCollection(sp);
        }
    }
}
