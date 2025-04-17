using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Shared.Utility;
using eFakturADM.Web.Models;
using RazorEngine;


namespace eFakturADM.Web.Helpers
{
    public partial class MailHelper
    {
        /// <summary>
        /// Function To Sent Email
        /// </summary>
        /// <param name="emailModel"></param>
        public string SendMail(out bool isError, EmailModel emailModel, EmailBodyModel emailBodyModel)
        {
            isError = false;
            var configSmtpServer = GeneralConfigs.GetById((int) ApplicationEnums.GeneralConfig.MailHelperSMTPServer);
            if (configSmtpServer == null)
            {
                isError = true;
                return "Config Not Found for [MailHelperSMTPServer]";
            }
            string SMTPServer = configSmtpServer.ConfigValue;
            
            var configfromUser = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.MailHelperFromUser);
            if (configfromUser == null)
            {
                isError = true;
                return "Config Not Found for [MailHelperFromUser]";
            }
            string fromUser = configfromUser.ConfigValue;
            
            var configpassword = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.MailHelperPasswd);
            if (configpassword == null)
            {
                isError = true;
                return "Config Not Found for [MailHelperPasswd]";
            }
            string password = configpassword.ConfigValue;

            var configdefaultCredential = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.MailHelperUseDefaultCredential);
            if (configdefaultCredential == null)
            {
                isError = true;
                return "Config Not Found for [MailHelperUseDefaultCredential]";
            }
            string defaultCredential = configdefaultCredential.ConfigValue;

            var configenableSSL = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.MailHelperEnableSSL);
            if (configenableSSL == null)
            {
                isError = true;
                return "Config Not Found for [MailHelperUseDefaultCredential]";
            }
            bool enableSSL = bool.Parse(configenableSSL.ConfigValue);

            var configport  = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.MailHelperPort);
            if (configport == null)
            {
                isError = true;
                return "Config Not Found for [MailHelperPort]";
            }
            string port = configport.ConfigValue;

            var configActivateEmail = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.MailHelperActivateEmail);
            if (configActivateEmail == null)
            {
                isError = true;
                return "Config Not Found for [MailHelperActivateEmail]";
            }
            bool ActivateEmail = bool.Parse(configActivateEmail.ConfigValue);

            string templateFileLocation = HttpContext.Current.Server.MapPath(emailModel.NotificationType == MailNotificationType.ResetPassword ?
                        "~/EmailTemplate/ResetPasswordMailTemplate.cshtml" : "~/EmailTemplate/EmailNotification.cshtml");
            string template = File.ReadAllText(templateFileLocation);
            string body = Razor.Parse<EmailBodyModel>(template, emailBodyModel);

            if (ActivateEmail)
            {
                try
                {
                    SmtpClient SmtpServer = new SmtpClient(SMTPServer);

                    SmtpServer.Timeout = (5 * 60 * 1000);
                    SmtpServer.Port = Convert.ToInt32(port);
                    SmtpServer.Credentials = new System.Net.NetworkCredential(fromUser, password);
                    //SmtpServer.UseDefaultCredentials = true;
                    SmtpServer.EnableSsl = enableSSL;

                    //Send Mail
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(fromUser, "Administrator");
                    mail.Subject = emailModel.EmailSubject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    foreach (string emailTo in emailModel.EmailIds)
                    {
                        mail.To.Add(emailTo);
                    }
                    if (emailModel.CC != null)
                    {
                        foreach (string emailCC in emailModel.CC)
                        {
                            mail.CC.Add(emailCC);
                        }
                    }
                    if (emailModel.BCC != null)
                    {
                        foreach (string emailBcc in emailModel.BCC)
                        {
                            mail.Bcc.Add(emailBcc);
                        }
                    }

                    SmtpServer.Send(mail);
                    return "Email sent successfully";
                }
                catch (Exception ex)
                {
                    isError = true;
                    string logKey2;
                    Logger.WriteLog(out logKey2, LogLevel.Error, ex.Message, MethodBase.GetCurrentMethod());
                    return "Failed to sent mail";
                }
            }
            isError = true;
            return "Email setting not activated, please contact your administrator to activate email";
        }

        public string DjpRequestErrorSendMail(out bool isError, string scanUrl, string logkey)
        {
            isError = false;
            var isReadyToSendMail = true;
            //get config
            var configEmailTo =
                GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DjpRequestErrorMailTo);
            if (configEmailTo == null)
            {
                const string msgError = "Config Data not found for [DjpRequestErrorMailTo]";
                string logKey2;
                Logger.WriteLog(out logKey2, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());
                isReadyToSendMail = false;
            }
            else
            {
                if (string.IsNullOrEmpty(configEmailTo.ConfigValue))
                {
                    const string msgError = "Config Value not found for [DjpRequestErrorMailTo]";
                    string logKey2;
                    Logger.WriteLog(out logKey2, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());
                    isReadyToSendMail = false;
                }
            }

            //get config
            var configEmailToDisplay =
                GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DjpRequestErrorMailToDisplayName);

            if (configEmailToDisplay == null)
            {
                const string msgError = "Config Data not found for [DjpRequestErrorMailToDisplayName]";
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());
                isReadyToSendMail = false;
            }
            else
            {
                if (string.IsNullOrEmpty(configEmailToDisplay.ConfigValue))
                {
                    const string msgError = "Config Value not found for [DjpRequestErrorMailToDisplayName]";
                    string logKey;
                    Logger.WriteLog(out logKey, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());
                    isReadyToSendMail = false;
                }
            }

            if (!isReadyToSendMail)
            {
                isError = true;
                return "Error Send Email Notification";
            }

            //Send Email here for Reset Email Notification
            var em = new EmailModel();
            var emails = configEmailTo.ConfigValue.Split(';').ToList();
            em.EmailIds = emails;
            em.EmailSubject = "DJP Request Error";
            em.NotificationType = MailNotificationType.DjpRequestError;

            var emb = new EmailBodyModel
            {
                To = configEmailToDisplay.ConfigValue,
                From = "eFaktur Application Administrator",
                ScanUrl = scanUrl,
                LogKey = logkey
            };
            var result = SendMail(out isError, em, emb);
            return result;
        }

    }
}