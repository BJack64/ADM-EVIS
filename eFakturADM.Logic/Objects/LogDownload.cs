using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class LogDownload
    {
        public long Id { get; set; }
        public string Requestor { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime RequestDate { get; set; }
        public string FileType { get; set; }
        public string ClientIp { get; set; }
    }
    public partial class LogDownload : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;
            Id = DBUtil.GetLongField(dr, "Id");
            Requestor = DBUtil.GetCharField(dr, "Requestor");
            FileName = DBUtil.GetCharField(dr, "FileName");
            FilePath = DBUtil.GetCharField(dr, "FilePath");
            RequestDate = DBUtil.GetDateTimeField(dr, "RequestDate");
            FileType = DBUtil.GetCharField(dr, "FileType");
            ClientIp = DBUtil.GetCharField(dr, "ClientIp");
            IsValid = true;
            return IsValid;
        }
    }
}
