using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class FakturPajakPrintCount
    {
        public long FakturPajakId { get; set; }
        public string NoFakturPajak { get; set; }
        public int PrintCount { get; set; }
    }
    public partial class FakturPajakPrintCount : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;
            FakturPajakId = DBUtil.GetLongField(dr, "FakturPajakId");
            PrintCount = DBUtil.GetIntField(dr, "PrintCount");
            NoFakturPajak = DBUtil.GetCharField(dr, "NoFakturPajak");
            IsValid = true;
            return IsValid;
        }
    }
}
