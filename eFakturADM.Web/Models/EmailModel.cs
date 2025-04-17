using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eFakturADM.Web.Helpers;

namespace eFakturADM.Web.Models
{
    /// <summary>
    /// EmailModel
    /// </summary>
    public class EmailModel
    {
        public EmailModel()
        {
            NotificationType = MailNotificationType.General;
        }
        /// <summary>
        /// FromAddress
        /// </summary>
        public string FromAddress { get; set; }
        /// <summary>
        /// EmailIds
        /// </summary>
        public List<string> EmailIds { get; set; }
        /// <summary>
        /// EmailSubject
        /// </summary>
        public string EmailSubject { get; set; }
        /// <summary>
        /// CC
        /// </summary>
        public List<string> CC { get; set; }
        /// <summary>
        /// BCC
        /// </summary>
        public List<string> BCC { get; set; }

        public MailNotificationType NotificationType { get; set; }

    }

    /// <summary>
    /// Mail Body
    /// </summary>
    public class EmailBodyModel
    {
        /// <summary>
        /// Header
        /// </summary>
        public string Header { get; set; }
        /// <summary>
        /// To
        /// </summary>
        public string To { get; set; }
        /// <summary>
        /// From
        /// </summary>
        public string From { get; set; }
        public string GeneratedPassword { get; set; }
        public string ScanUrl { get; set; }
        public string LogKey { get; set; }
    }

    public enum MailNotificationType
    {
        ResetPassword,
        General,
        DjpRequestError
    }

}