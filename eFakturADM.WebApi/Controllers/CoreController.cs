using eFakturADM.Shared.Utility;
using eFakturADM.WebApi.Attributes;
using eFakturADM.WebApi.Helper;
using eFakturADM.WebApi.Models;
using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace eFakturADM.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [CustomExceptionFilter]
    [Log]
    [RoutePrefix("api/core")]
    public class CoreController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        [Route("token")]
        [HttpPost]        
        public AccessTokenResponseModel Token(AccessTokenRequestModel data)
        {
            var result = new AccessTokenResponseModel()
            {
                Status = false, Message = "", Token = ""
            };
            try
            {
                result = data.IsValid(data.Username, data.Password, "eFakturADM.WebApi" ,Request.GetClientIp());  
            }
            catch (Exception ex)
            {
                string outlogkey;
                Logger.WriteLog(out outlogkey, LogLevel.Error, "Unhandle exception", MethodBase.GetCurrentMethod(), ex);
                result.Message = "Unhandle Exception : " + outlogkey;
            }
            return result;
        }
    }
}