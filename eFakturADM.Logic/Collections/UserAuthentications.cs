using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class UserAuthentications : ApplicationCollection<UserAuthentication, SpBase>
    {
        public static UserAuthentication GetToken(string Token)
        {
            SpBase sp = new SpBase(String.Format(@"
                            SELECT			*
	                        FROM            dbo.UserAuthentication
	                        WHERE			Token = @Token 
					                        AND TimeEnd > GETDATE()
					                        AND Status = 1", Table));
            sp.AddParameter("Token", Token);
            var data = GetApplicationObject(sp);
            if (data == null || data.UserAuthenticationId == 0) return null;
            return data;
        }

        public static UserAuthentication CheckLoginExists(int UserId, string IP)
        {
            SpBase sp = new SpBase(String.Format(@"
                            SELECT			*
	                        FROM            dbo.UserAuthentication
	                        WHERE			UserId = @UserId 
					                        AND TimeEnd > GETDATE()
					                        AND Status = 1
                                            AND IP != @IP
		                                    ", Table));
            sp.AddParameter("UserId", UserId);
            sp.AddParameter("IP", IP);
            var data = GetApplicationObject(sp);
            if (data == null || data.UserId == 0) return null;
            return data;
        }

        /// <summary>
        /// Update shall change status to false , token is not valid anymore
        /// insert shall be hit each login transaction
        /// </summary>
        /// <returns></returns>
        public static UserAuthentication Save(UserAuthentication data)
        {
            data.WasSaved = false;
            SpBase sp = null;

            if (data.UserAuthenticationId > 0)
            {
                sp = new SpBase(String.Format(@"
                        UPDATE dbo.UserAuthentication
                        SET    Status	= 0
                        WHERE UserID	= @UserID
                    ", Table));
            }
            else
            {
                sp = new SpBase(String.Format(@"
                          INSERT INTO dbo.UserAuthentication
                               (UserId
		                       ,IP
		                       ,UserAgent
		                       ,Token
                               ,TimeStart
                               ,TimeEnd
                               ,Status)
                         VALUES
                               (@UserId
		                       ,@IP
		                       ,@UserAgent
		                       ,@Token
                               ,@TimeStart
                               ,@TimeEnd
                               ,@Status); SELECT @UserAuthenticationId = @@IDENTITY", Table));

                sp.AddParameter("UserAuthenticationId", data.UserAuthenticationId, ParameterDirection.Output);
                sp.AddParameter("IP", data.IP ?? "127.0.0.1");
                sp.AddParameter("UserAgent", data.UserAgent);
                sp.AddParameter("Token", data.Token);
                sp.AddParameter("TimeStart", data.TimeStart);
                sp.AddParameter("TimeEnd", data.TimeEnd);
                sp.AddParameter("Status", data.Status);
            }

            sp.AddParameter("UserId", data.UserId);

            if (sp.ExecuteNonQuery() == 0)
                data.WasSaved = true;

            if (data.UserAuthenticationId <= 0)
            {
                data.UserAuthenticationId = (long)sp.GetParameter("UserAuthenticationId");
            }

            return data;
        }

        public static bool Delete(int userId, string userAgent)
        {
            bool result;
            SpBase sp = null;

            sp = new SpBase(String.Format(@"
                        DELETE FROM dbo.UserAuthentication
                        WHERE UserID	= @UserID AND UserAgent = @UserAgent
                    ", Table));

            sp.AddParameter("UserId", userId);
            sp.AddParameter("UserAgent", userAgent);

            result = sp.ExecuteNonQuery() == 0;

            return result;
        }
    }
}
