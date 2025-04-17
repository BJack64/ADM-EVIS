using eFakturADM.DJPLib;
using eFakturADM.DJPLib.Models;
using eFakturADM.DJPService;
using eFakturADM.Web.Helpers;
using eFakturADM.WebApi.Attributes;
using eFakturADM.WebApi.Controllers.Base;
using eFakturADM.WebApi.Models;
using System.Net;
using System.Web.Http;

namespace eFakturADM.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [CustomAuthorize]
    [CustomExceptionFilter]
    [Log]
    [RoutePrefix("api/general")]
    public class GeneralController :  BaseApiController
    {
        #region Validasi Faktur Pajak
        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        [Route("Validasi")]
        public FakturPajakResultModel ValidasiFakturPajak(string Url,  string FPdjpID)
        {
            bool isValid = true;
            string errMsg = string.Empty;

            #region Validation Empty Field

            if (isValid && string.IsNullOrEmpty(Url))
            {
                isValid = false;
                errMsg = "Url is required.";
            }

            #endregion


            if (isValid)
            {
                WebExceptionStatus eStatus;
                string msgError;
                string logKey;

                DjpServiceConfiguration.LoadConfig();

                var inetProxy = DjpServiceConfiguration.InternetProxy;
                var inetProxyPort = DjpServiceConfiguration.InternetProxyPort;
                var inetProxyUseCredential = DjpServiceConfiguration.UseDefaultCredential;
                var itimeoutsetting = DjpServiceConfiguration.DJPRequestTimeOutSetting;
                var reqinterval = DjpServiceConfiguration.ServiceRequestDetailFakturPajakDjpRequestInterval;
                bool isUseProxy = !string.IsNullOrEmpty(inetProxy);
                //bool isUseProxy = false;


                var result = ValidateFakturLib.ValidateFakturObjectAPI(Url, UserAuth(Request)?.UserInitial, UserAuth(Request)?.UserName, FPdjpID,
                        itimeoutsetting, isUseProxy, inetProxy, inetProxyPort
                        , inetProxyUseCredential, out msgError, out eStatus, out logKey);


                if (eStatus == WebExceptionStatus.Timeout)
                {
                    var mh = new MailHelper();
                    bool isErrorSendMail;
                    var logkey2 = "";
                    mh.DjpRequestErrorSendMail(out isErrorSendMail, Url, logkey2);

                    if (isErrorSendMail)
                    {
                        result.StatusValidasi = "true";
                        result.KeteranganValidasi = result.KeteranganValidasi + "\n Failed Get Data From DJP \n Send Email Notification error.";
                    }

                }

                return result;
            }
            else
            {
                return new FakturPajakResultModel();
            }
        }
        #endregion

    }
}