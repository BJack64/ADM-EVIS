using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using eFakturADM.Logic;
using eFakturADM.Logic.Utilities;
using eFakturADM.Shared.Utility;
using eFakturADM.WebApi.Models;

namespace eFakturADM.WebApi.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class TokenHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        public static string GenerateToken(string ClientID)
        {
            //generate token yang ada expired nya
            var dtnow = DateTime.Now;
            var expiredinMinute = 30; //simpan di config
            string outlogkey;
            try
            {
                var sx = ConfigurationManager.AppSettings["TokenExpiredInMinute"] as string;
                if (!string.IsNullOrEmpty(sx))
                {
                    expiredinMinute = int.Parse(sx);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(out outlogkey, LogLevel.Info, string.Format("Use Default Token Expired : {0} Minute", expiredinMinute), MethodBase.GetCurrentMethod(), ex);
                Logger.WriteLog(out outlogkey, LogLevel.Error, ex.Message, MethodBase.GetCurrentMethod(), ex);
            }

            var expiredtime = dtnow.AddMinutes(expiredinMinute).ToString("dd-MM-yyyy HH:mm:ss");

            var plainttexttoken = string.Format("{0};{1}", ClientID, expiredtime);

            var token = CryptographyHelperExtension.EncryptAndHash(plainttexttoken);
            return token;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="token"></param>
        /// <param name="tokenresult"></param>
        /// <returns></returns>
        //public static bool IsValid(APIClientService svc, string token, out TokenResultStatus tokenresult)
        //{
        //    tokenresult = TokenResultStatus.Invalid;
        //    var result = false;
        //    string outlogkey;
        //    try
        //    {
        //        var dtnow = DateTime.Now;
        //        var plaintexttoken = CryptographyHelperExtension.DecryptWithHash(token);
        //        var strsplit = plaintexttoken.Split(';').ToList();
        //        if(strsplit.Count == 2)
        //        {
        //            var clientID = strsplit[0];
        //            var expiredtime = strsplit[1];
        //            //check valid clientID
        //            Guid clientIDGuid = Guid.NewGuid();
        //            try
        //            {
        //                clientIDGuid = Guid.Parse(clientID);
        //            }
        //            catch (Exception ex2)
        //            {
        //                Logger.WriteLog(out outlogkey, LogLevel.Error, "Invalid ClientID - " + clientID, MethodBase.GetCurrentMethod(), ex2);
        //            }
        //            var dats = svc.GetByClientID(clientIDGuid);
        //            if(dats != null)
        //            {
        //                CultureInfo provider = CultureInfo.InvariantCulture;
        //                var DateExpired = dtnow;
        //                try
        //                {
        //                    //check expired
        //                    DateExpired = DateTime.ParseExact(expiredtime, "dd-MM-yyyy HH:mm:ss", provider);
        //                }
        //                catch (Exception ex)
        //                {
        //                    Logger.WriteLog(out outlogkey, LogLevel.Error, "Invalid ExpiredTime - " + expiredtime, MethodBase.GetCurrentMethod(), ex);
        //                }

        //                if(DateExpired != dtnow)
        //                {
        //                    if(DateExpired <= dtnow)
        //                    {
        //                        Logger.WriteLog(out outlogkey, LogLevel.Error, "Token Expired - " + token, MethodBase.GetCurrentMethod(), null);
        //                        tokenresult = TokenResultStatus.Expired;
        //                    }
        //                    else
        //                    {
        //                        tokenresult = TokenResultStatus.OK;
        //                        result = true;
        //                    }
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteLog(out outlogkey, LogLevel.Error, "Invalid Token", MethodBase.GetCurrentMethod(), ex);
        //    }
        //    return result;
        //}
        public string GetClientIDFromToken(string token)
        {
            string clientID = string.Empty;
            try
            {
                var plaintexttoken = CryptographyHelperExtension.DecryptWithHash(token);
                var strsplit = plaintexttoken.Split(';').ToList();
                if (strsplit.Count == 2)
                {
                    clientID = strsplit[0];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return clientID;
            }
            return clientID;
        }
    }
}