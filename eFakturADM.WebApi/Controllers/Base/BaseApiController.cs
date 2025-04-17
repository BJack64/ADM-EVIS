using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Objects;
using System.Net.Http;
using System.Web.Http;

namespace eFakturADM.WebApi.Controllers.Base
{
    public class BaseApiController : ApiController
    {
        protected User UserAuth(HttpRequestMessage http)
        {
            var authHeader = http.Headers.Authorization.Parameter;
            if (string.IsNullOrEmpty(authHeader))
                return null;

            var config = GeneralConfigs.GetConfigStaticToken(authHeader);
            if (config != null)
            {
                User user = Users.GetByUserName(config.ConfigValue);
                return user;
            }
            else
            {
                UserAuthentication userAuthentication = UserAuthentications.GetToken(authHeader);
                if (userAuthentication != null)
                {
                    User user = Users.GetById(userAuthentication.UserId);
                    return user;
                }
            }
            return null;
        }
    }
}