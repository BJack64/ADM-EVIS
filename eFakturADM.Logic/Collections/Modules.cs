using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using System;
using System.Collections.Generic;

namespace eFakturADM.Logic.Collections
{
    public class Modules : ApplicationCollection<Module, SpBase>
    {

        /// <summary>
        /// Gets collection of all Module in the database.
        /// </summary>
        /// <returns>List of Module object instances.</returns>
        public static List<Module> Get()
        {
            var sp = new SpBase(String.Format(@"SELECT Modules.* 
                FROM Modules Modules
                "));

            return GetApplicationCollection(sp);
        }

        public static List<Module> GetForRoleMenu()
        {
            var sp = new SpBase(String.Format(@"SELECT	v.*
FROM	(
SELECT	DISTINCT m.[ModuleId]
      ,(mp.Name + ' - ' + m.[Name]) AS [Name]
      ,m.[ModuleIdParent]
      ,m.[IconUrl]
      ,m.[IconHoverUrl]
      ,m.[Url]
FROM	Modules m INNER JOIN
		Activity a ON m.ModuleId = a.ModuleId INNER JOIN
		Modules mp ON m.ModuleIdParent = mp.ModuleId
UNION
SELECT	DISTINCT m.[ModuleId]
      ,m.Name AS [Name]
      ,m.[ModuleIdParent]
      ,m.[IconUrl]
      ,m.[IconHoverUrl]
      ,m.[Url]
FROM	Modules m INNER JOIN
		Activity a ON m.ModuleId = a.ModuleId
WHERE	m.ModuleIdParent IS NULL) v
ORDER BY v.ModuleId ASC
                "));
            return GetApplicationCollection(sp);
        }

        /// <summary>
        /// Gets Module object instance by unique identifier
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <returns>Instance of Module object.</returns>
        public static Module GetById(long ModuleId)
        {
            SpBase sp = new SpBase(@"SELECT Modules.* FROM Modules
                                    WHERE ModuleId = @ModuleId");
            sp.AddParameter("ModuleId", ModuleId);

            return GetApplicationObject(sp);
        }

        /// <summary>
        /// Gets Module object instance by name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns>Insatnce of Module object.</returns>
        public static Module GetByName(string Name)
        {
            SpBase sp = new SpBase("SELECT * FROM Modules WHERE Name = @Name");
            sp.AddParameter("Name", Name);

            return GetApplicationObject(sp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ModuleIdParent"></param>
        /// <returns></returns>
        public static List<Module> GetByParent(int ModuleIdParent)
        {
            var sp = new SpBase(@"SELECT Modules.* 
                FROM Modules 
                WHERE ModuleIdParent = @ModuleIdParent");

            sp.AddParameter("ModuleIdParent", ModuleIdParent);

            return GetApplicationCollection(sp);
        }

        public static List<Module> GetSideBarMenuParent(int userId)
        {
            var sp = new SpBase(@"SELECT dats.*
FROM (
	SELECT	DISTINCT m.*
	FROM	Modules m INNER JOIN
			Modules mc ON mc.ModuleIdParent = m.ModuleId INNER JOIN
			Activity a ON mc.ModuleId = a.ModuleId INNER JOIN
			RoleActivity ra ON a.ActivityId = ra.ActivityId INNER JOIN
			[UserRole] ur ON ur.RoleId = ra.RoleId
	WHERE	ur.UserId = @userId AND ur.IsDeleted = 0 AND ra.IsDeleted = 0
	UNION ALL
	SELECT	DISTINCT mc.*
	FROM	Modules mc INNER JOIN
			Activity a ON mc.ModuleId = a.ModuleId INNER JOIN
			RoleActivity ra ON a.ActivityId = ra.ActivityId INNER JOIN
			[UserRole] ur ON ur.RoleId = ra.RoleId
	WHERE	ur.UserId = @userId AND ur.IsDeleted = 0 AND ra.IsDeleted = 0
			AND mc.ModuleIdParent IS NULL
	) dats
ORDER BY dats.ModuleId");

            sp.AddParameter("userId", userId);

            return GetApplicationCollection(sp);

        }

        public static List<Module> GetSideBarMenuChilds(int userId)
        {
            var sp = new SpBase(@"SELECT	DISTINCT mc.*
FROM	Modules mc INNER JOIN
		Activity a ON mc.ModuleId = a.ModuleId INNER JOIN
		RoleActivity ra ON a.ActivityId = ra.ActivityId INNER JOIN
		[UserRole] ur ON ur.RoleId = ra.RoleId
WHERE	ur.UserId = @userId AND ur.IsDeleted = 0 AND ra.IsDeleted = 0
		AND mc.ModuleIdParent IS NOT NULL");

            sp.AddParameter("userId", userId);

            return GetApplicationCollection(sp);

        }

    }
}
