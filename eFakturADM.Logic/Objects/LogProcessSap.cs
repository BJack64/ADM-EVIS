using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;
using eFakturADM.Shared.Utility;

namespace eFakturADM.Logic.Objects
{
    public partial class LogProcessSap
    {
        public long LogProcessSapId { get; set; }
        public string IdNo { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string StatusString { get; set; }
        public string XmlFileType { get; set; }
    }
    public partial class LogProcessSap : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;
            LogProcessSapId = DBUtil.GetLongField(dr, "LogProcessSapId");
            IdNo = DBUtil.GetCharField(dr, "IdNo");
            FileName = DBUtil.GetCharField(dr, "FileName");
            FilePath = DBUtil.GetCharField(dr, "FilePath");
            Status = DBUtil.GetIntField(dr, "Status");
            Note = DBUtil.GetCharField(dr, "Note");

            Created = DBUtil.GetDateTimeField(dr, "Created");
            Modified = DBUtil.GetDateTimeNullField(dr, "Modified");
            CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            ModifiedBy = DBUtil.GetCharField(dr, "ModifiedBy");
            IsDeleted = DBUtil.GetBoolField(dr, "IsDeleted");

            XmlFileType = DBUtil.GetCharField(dr, "XmlFileType");

            StatusString = EnumHelper.GetDescription((ApplicationEnums.SapStatusLog)Status);

            IsValid = true;
            return IsValid;
        }
    }
}
