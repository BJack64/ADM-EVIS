using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eFakturADM.Logic.Collections
{
    public class  RoleActivities: ApplicationCollection<RoleActivity, SpBase>
    {
        /// <summary>
        /// Gets collection of all roleactivity in the database.
        /// </summary>
        /// <returns>List of RoleActivity object instances.</returns>
        public static List<RoleActivity> Get()
        {
            SpBase sp = new SpBase(@"SELECT		RoleActivity.*, 
			                                    Role.Name AS RoleName, 
			                                    Activity.ActivityName AS ActivityName, 
			                                    Modules.Name AS ModuleName,
                                                Modules.ModuleId AS ModuleId		
                                    FROM		RoleActivity
			                                    LEFT JOIN Role ON Role.RoleId = RoleActivity.RoleId
			                                    LEFT JOIN Activity ON Activity.ActivityId = RoleActivity.ActivityId
			                                    LEFT JOIN Modules ON Activity.ModuleId = Modules.ModuleId      
                                    WHERE       RoleActivity.IsDeleted = 0");

            return GetApplicationCollection(sp);
        }

        /// <summary>
        /// Gets roleactivity object instance by roleactivityid
        /// </summary>
        /// <param name="TaskHazardMappingId"></param>
        /// <returns>Instance of roleactivity object.</returns>
        public static RoleActivity GetById(long RoleActivityId)
        {
            SpBase sp = new SpBase(@"SELECT		RoleActivity.*, 
			                                    Role.Name AS RoleName, 
			                                    Activity.ActivityName AS ActivityName, 
			                                    Modules.Name AS ModuleName,
                                                Modules.ModuleId AS ModuleId		                                                   

                                    FROM		RoleActivity
			                                    LEFT JOIN Role ON Role.RoleId = RoleActivity.RoleId
			                                    LEFT JOIN Activity ON Activity.ActivityId = RoleActivity.ActivityId
			                                    LEFT JOIN Modules ON Activity.ModuleId = Modules.ModuleId      
                                    WHERE       RoleActivity.RoleActivityId = @RoleActivityId  AND RoleActivity.IsDeleted = 0");
            sp.AddParameter("RoleActivityId", RoleActivityId);
            return GetApplicationObject(sp);
        }   

        /// <summary>
        /// Gets role activity object instance by role id
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns>Instance of roleactivity object.</returns>
        public static List<RoleActivity> GetByRoleId(int RoleId)
        {
            SpBase sp = new SpBase(@"SELECT		RoleActivity.*, 
			                                    Role.Name AS RoleName, 
			                                    Activity.ActivityName AS ActivityName, 
			                                    Modules.Name AS ModuleName,
                                                Modules.ModuleId AS ModuleId	
                                    FROM		RoleActivity
			                                    LEFT JOIN Role ON Role.RoleId = RoleActivity.RoleId
			                                    LEFT JOIN Activity ON Activity.ActivityId = RoleActivity.ActivityId
			                                    LEFT JOIN Modules ON Activity.ModuleId = Modules.ModuleId      
                                    WHERE       RoleActivity.RoleId = @RoleId  AND RoleActivity.IsDeleted = 0");
            sp.AddParameter("RoleId", RoleId);
            return GetApplicationCollection(sp);
        }

        /// <summary>
        /// Gets role activity object instance by activityid and roleid
        /// </summary>
        /// <param name="ActivityId"></param>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public static RoleActivity GetByActivityId_RoleId(int ActivityId, int RoleId)
        {
            SpBase sp = new SpBase(@"SELECT		RoleActivity.*, 
			                                    Role.Name AS RoleName, 
			                                    Activity.ActivityName AS ActivityName, 
			                                    Modules.Name AS ModuleName,
                                                Modules.ModuleId AS ModuleId	
                                    FROM		RoleActivity
			                                    LEFT JOIN Role ON Role.RoleId = RoleActivity.RoleId
			                                    LEFT JOIN Activity ON Activity.ActivityId = RoleActivity.ActivityId
			                                    LEFT JOIN Modules ON Activity.ModuleId = Modules.ModuleId      
                                    WHERE       RoleActivity.ActivityId = @ActivityId AND RoleActivity.RoleId = @RoleId  AND RoleActivity.IsDeleted = 0");
            sp.AddParameter("RoleId", RoleId);
            sp.AddParameter("ActivityId", ActivityId);
            return GetApplicationObject(sp);
        }

        public static List<RoleActivity> GetByActivityId_RoleId_Auth(string ActivityId, int RoleId)
        {
            SpBase sp = new SpBase(@"SELECT		RoleActivity.*, 
			                                    Role.Name AS RoleName, 
			                                    Activity.ActivityName AS ActivityName, 
			                                    Modules.Name AS ModuleName,
                                                Modules.ModuleId AS ModuleId	
                                    FROM		RoleActivity
			                                    LEFT JOIN Role ON Role.RoleId = RoleActivity.RoleId
			                                    LEFT JOIN Activity ON Activity.ActivityId = RoleActivity.ActivityId
			                                    LEFT JOIN Modules ON Activity.ModuleId = Modules.ModuleId      
                                    WHERE       RoleActivity.ActivityId IN (select * from dbo.Split(@ActivityId)) AND RoleActivity.RoleId = @RoleId  AND RoleActivity.IsDeleted = 0");
            sp.AddParameter("RoleId", RoleId);
            sp.AddParameter("ActivityId", ActivityId);
            return GetApplicationCollection(sp);
        }
    }    
}
