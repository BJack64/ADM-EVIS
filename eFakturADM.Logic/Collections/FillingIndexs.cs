using System.Linq;
using eFakturADM.Logic.Core;

namespace eFakturADM.Logic.Collections
{
    public class FillingIndexs 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fakturPajakIds">Separated by comma</param>
        /// <param name="userModify"></param>
        /// <param name="errMessage">Out Param</param>
        /// <returns></returns>
        public static bool CreateFillingIndex(string fakturPajakIds, string userModify, out string errMessage)
        {
            errMessage = CheckUserInitial(fakturPajakIds);

            if (!string.IsNullOrEmpty(errMessage))
            {
                return false;
            }

            var sp = new SpBase(@"EXEC sp_CreateFillingIndex @fakturPajakIds, @userModify");

            sp.AddParameter("fakturPajakIds", fakturPajakIds);
            sp.AddParameter("userModify", userModify);

            return sp.ExecuteNonQuery() >= 0;

        }

        private static string CheckUserInitial(string fakturPajakIds)
        {
            var getUsers = Users.GetByFakturPajakIds(fakturPajakIds);

            if (getUsers.Count <= 0)
            {
                return "User Creator not valid.";
            }

            var chkExistInitial = getUsers.Where(c => string.IsNullOrEmpty(c.UserInitial)).ToList();

            if (chkExistInitial.Count <= 0) return string.Empty;
            var usernames = string.Join(",", chkExistInitial.Select(d => d.UserName));

            return "User Initial not found for Creator : " + usernames;
        }

    }
}
