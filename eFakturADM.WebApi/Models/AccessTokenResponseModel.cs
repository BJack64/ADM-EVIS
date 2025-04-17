
using System;
using System.ComponentModel;

namespace eFakturADM.WebApi.Models
{
    public class AccessTokenResponseModel : RequestResultModel
    {
        public string Token { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }

    public enum TokenResultStatus
    {
        [Description("OK")]
        OK = 1,
        [Description("Expired")]
        Expired = 2,
        [Description("Invalid")]
        Invalid = 3
    }

}