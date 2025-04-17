using eFakturADM.Logic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eFakturADM.Web.Models
{
    public enum NotifyType
    {
        PageInline = 0,
        DialogInline = 1,
        Dialog = 2,
        PageFixedPoupUp = 3
    }

    public enum RequestResultInfoType
    {
        Information = 0,
        Success = 1,
        Warning = 2,
        ErrorOrDanger = 3,
        ErrorWithFileDownload = 4,
        ErrorWithConfirmation = 5
    }

    public class RequestResultModel
    {
        public String Title { get; set; }
        public String Message { get; set; }
        public int HideInSeconds { get; set; }
        public RequestResultInfoType InfoType { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public RequestResultModel()
        {
            HideInSeconds = 5000;
            InfoType = RequestResultInfoType.Success;
            Height = 150;
            Width = 600;
            Title = "";
            Message = "";
        }

        public static string GetFormattedMessage(ApplicationObject Value)
        {
            string Name = "";
            switch (Value.UIMessageType)
            {
                case UIMessageType.Information: Name = "information-white.png"; break;
                case UIMessageType.Success: Name = "tick-circle.png"; break;
                case UIMessageType.Warning: Name = "exclamation-octagon.png"; break;
                case UIMessageType.ErrorOrDanger: Name = "exclamation-red.png"; break;
            }

            return String.Format("<img src=\"{0}content/images/{1}\"/> {2}", HttpRuntime.AppDomainAppVirtualPath, Name, Value.UIMessage);
        }

    }
}