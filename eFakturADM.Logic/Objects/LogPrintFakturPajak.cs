using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class LogPrintFakturPajak
    {
        public int TotalItems { get; set; }
    }
    public partial class LogPrintFakturPajak
    {
        public int VSequence { get; set; }
        public long LogPrintId { get; set; }
        public long FakturPajakId { get; set; }
        public string PrintType { get; set; }
        public DateTime PrintDate { get; set; }
        public string PrintBy { get; set; }
        public string Reason { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string HeaderGuid { get; set; }
    }
    public partial class LogPrintFakturPajak : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;

            LogPrintId = DBUtil.GetLongField(dr, "LogPrintId");
            FakturPajakId = DBUtil.GetLongField(dr, "FakturPajakId");
            PrintType = DBUtil.GetCharField(dr, "PrintType");

            PrintDate = DBUtil.GetDateTimeField(dr, "PrintDate");
            PrintBy = DBUtil.GetCharField(dr, "PrintBy");
            Reason = DBUtil.GetCharField(dr, "Reason");

            Created = DBUtil.GetDateTimeField(dr, "Created");
            Modified = DBUtil.GetDateTimeNullField(dr, "Modified");
            CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            ModifiedBy = DBUtil.GetCharField(dr, "ModifiedBy");
            IsDeleted = DBUtil.GetBoolField(dr, "IsDeleted");

            HeaderGuid = DBUtil.GetCharField(dr, "HeaderGuid");

            TotalItems = DBUtil.GetIntField(dr, "TotalItems");
            VSequence = DBUtil.GetIntField(dr, "VSequence");

            IsValid = true;
            return IsValid;
        }
    }
}
