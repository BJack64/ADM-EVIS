using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class FakturPajakBatchRequestSetting
    {
        public int Id { get; set; }
        public string UrlScan { get; set; }
        public string FormatedNoFaktur { get; set; }
        public DateTime BatchDate { get; set; }
        public int BatchOrder { get; set; }
        public int? ProcessStatus { get; set; }
    }

    public partial class FakturPajakBatchRequestSetting : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;

            Id = DBUtil.GetIntField(dr, "Id");
            UrlScan = DBUtil.GetCharField(dr, "UrlScan");
            FormatedNoFaktur = DBUtil.GetCharField(dr, "FormatedNoFaktur");
            BatchDate = DBUtil.GetDateTimeField(dr, "BatchDate");
            BatchOrder = DBUtil.GetIntField(dr, "BatchOrder");
            ProcessStatus = DBUtil.GetIntNullField(dr, "ProcessStatus");

            IsValid = true;
            return IsValid;
        }
    }
}
