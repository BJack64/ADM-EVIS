using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Utilities;
using System.Collections.Generic;

namespace eFakturADM.Logic.Collections
{
    public class Users : ApplicationCollection<User, SpBase>
    {
        /// <summary>
        /// Gets collection of all User in the database.
        /// </summary>
        /// <returns>List of User object instances.</returns>
        public static List<User> Get()
        {
            SpBase sp = new SpBase(@"SELECT [User].*,  ISNULL(STUFF(
                                        (	SELECT ', ' + r.Name
                                            FROM UserRole ur INNER JOIN
                                                    Role r ON ur.RoleId = r.RoleId
                                            WHERE  ur.UserId = [User].UserId AND ur.IsDeleted = 0
                                            FOR XML PATH('')
                                        )
                                    , 1, 1, ''),'') UserRole

            FROM [User] WHERE IsDeleted = 0");
            return GetApplicationCollection(sp);
        }

        /// <summary>
        /// Gets Module object instance by unique identifier
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>Instance of User object.</returns>
        public static User GetById(int UserId)
        {
            SpBase sp = new SpBase(@"SELECT [User].*, '' AS UserRole
                                    FROM [User]
                                    WHERE UserId = @UserId");
            sp.AddParameter("UserId", UserId);
            var dbData = GetApplicationObject(sp);
            return dbData == null || dbData.UserId == 0 ? null : dbData;
        }

        /// <summary>
        /// Gets User object instance by UserName
        /// </summary>
        /// <param name="Name"></param>
        /// <returns>Instance of User object.</returns>
        public static User GetByUserName(string UserName)
        {
            SpBase sp = new SpBase("SELECT [User].*, '' AS UserRole FROM [User] WHERE UserName = @UserName AND IsDeleted = 0");
            sp.AddParameter("UserName", UserName);
            var dbData = GetApplicationObject(sp);
            return dbData == null || dbData.UserId == 0 ? null : dbData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password">plain text, no need to encrypt</param>
        /// <returns></returns>
        public static User GetLogin(string username, string password)
        {
            var sp = new SpBase(@"SELECT  DISTINCT  [User].*,  
                                    ISNULL(STUFF(
                                        (	SELECT ', ' + r.Name
                                            FROM UserRole ur INNER JOIN
                                                    Role r ON ur.RoleId = r.RoleId
                                            WHERE  ur.UserId = [User].UserId AND ur.IsDeleted = 0
                                            FOR XML PATH('')
                                        )
                                    , 1, 1, ''),'') UserRole
                                FROM [User] LEFT JOIN UserRole ON [User].UserId = UserRole.UserId                   
                     WHERE [User].Username = @Username AND [User].Password = @Password");

            sp.AddParameter("Password", Rijndael.Encrypt(password));
            sp.AddParameter("Username", username);

            var data = GetApplicationObject(sp);
            if (data == null || data.UserId == 0) return null;

            return data;
        }

        public static List<User> GetByFakturPajakIds(string fakturPajakIds)
        {
            var sp = new SpBase(@"SELECT	[User].*, '' AS UserRole 
FROM	[User] INNER JOIN
		(
			SELECT	DISTINCT fp.CreatedBy
			FROM	FakturPajak fp
			WHERE	fp.FakturPajakId IN (SELECT Data FROM dbo.Split(@fakturPajakIds))
		) f ON [User].[UserName] = f.CreatedBy
WHERE [User].IsDeleted = 0");

            sp.AddParameter("fakturPajakIds", fakturPajakIds);

            return GetApplicationCollection(sp);

        }

    }
}
