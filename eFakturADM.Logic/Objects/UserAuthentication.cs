using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class UserAuthentication
    {
        public long UserAuthenticationId { get; set; }
        public int UserId { get; set; }
        public string IP { get; set; }
        public string UserAgent { get; set; }
        public string Token { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public bool Status { get; set; }
    }

    public partial class UserAuthentication : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;

            UserAuthenticationId = DBUtil.GetLongField(dr, "UserAuthenticationId");
            UserId = DBUtil.GetIntField(dr, "UserId");
            IP = DBUtil.GetCharField(dr, "IP");
            UserAgent = DBUtil.GetCharField(dr, "UserAgent");
            Token = DBUtil.GetCharField(dr, "Token");
            TimeStart = DBUtil.GetDateTimeField(dr, "TimeStart");
            TimeEnd = DBUtil.GetDateTimeField(dr, "TimeEnd");
            Status = DBUtil.GetBoolField(dr, "Status");

            IsValid = true;
            return IsValid;
        }
    }
}
