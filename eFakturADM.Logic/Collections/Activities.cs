using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using System.Collections.Generic;

namespace eFakturADM.Logic.Collections
{
    public class Activities : ApplicationCollection<Activity, SpBase>
    {
        /// <summary>
        /// Gets collection of all Module in the database.
        /// </summary>
        /// <returns>List of Module object instances.</returns>
        public static List<Activity> Get()
        {
            SpBase sp = new SpBase("SELECT * FROM Activity");
            return GetApplicationCollection(sp);
        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="GGI"></param>
        /// <returns></returns>
        public static List<Activity> GetActiveActivityByGGI(string GGI)
        {
            SpBase sp = new SpBase(@"SELECT Activity.* FROM [User] INNER JOIN UserRole on [User].UserId = UserRole.UserId INNER JOIN RoleActivity on UserRole.RoleId = RoleActivity.RoleId INNER JOIN Activity on RoleActivity.ActivityId = Activity.ActivityId
                            WHERE [User].GGI = @GGI AND RoleActivity.IsDeleted = 0 AND UserRole.IsDeleted = 0");

            sp.AddParameter("GGI", GGI);

            return GetApplicationCollection(sp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ActivityId"></param>
        /// <returns></returns>
        public static Activity GetById(int ActivityId)
        {
            SpBase sp = new SpBase("SELECT * FROM Activity WHERE ActivityId = @ActivityId");

            sp.AddParameter("ActivityId", ActivityId);
            return GetApplicationObject(sp);
        }
    }
}
