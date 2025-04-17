using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class LogRequestFakturPajak
    {
        public long LogRequestFakturPajakId { get; set; }
        public DateTime RequestDate { get; set; }
        public string RequestUrl { get; set; }
        public long FakturPajakId { get; set; }
        public int Status { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
    public partial class LogRequestFakturPajak : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;

            LogRequestFakturPajakId = DBUtil.GetLongField(dr, "LogRequestFakturPajakId");
            RequestDate = DBUtil.GetDateTimeField(dr, "RequestDate");
            RequestUrl = DBUtil.GetCharField(dr, "RequestUrl");
            FakturPajakId = DBUtil.GetLongField(dr, "FakturPajakId");
            Status = DBUtil.GetIntField(dr, "Status");
            ErrorMessage = DBUtil.GetCharField(dr, "ErrorMessage");
            
            IsDeleted = DBUtil.GetBoolField(dr, "IsDeleted");
            Created = DBUtil.GetDateTimeField(dr, "Created");
            Modified = DBUtil.GetDateTimeNullField(dr, "Modified");
            CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            ModifiedBy = DBUtil.GetCharField(dr, "ModifiedBy");
            
            IsValid = true;
            return IsValid;
        }
    }
}
