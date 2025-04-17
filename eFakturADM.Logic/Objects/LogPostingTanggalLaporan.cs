using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;
using System;
using System.Data;

namespace eFakturADM.Logic.Objects
{
    public partial class LogPostingTanggalLaporan
    {
        public long Id { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string Payload { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public string FPdjpID { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
    }

    public partial class LogPostingTanggalLaporan : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;
            Id = DBUtil.GetLongField(dr, "ID");
            Source = DBUtil.GetCharField(dr, "Source");
            Url = DBUtil.GetCharField(dr, "Url");
            Method = DBUtil.GetCharField(dr, "Method");
            Payload = DBUtil.GetCharField(dr, "Payload");
            Status = DBUtil.GetIntField(dr, "Status");
            Created = DBUtil.GetDateTimeField(dr, "Created");
            CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            FPdjpID = DBUtil.GetCharField(dr, "FPdjpID");
            Action = DBUtil.GetCharField(dr, "Action");
            Message = DBUtil.GetCharField(dr, "Message");
            IsValid = true;
            return IsValid;
        }
    }
}
