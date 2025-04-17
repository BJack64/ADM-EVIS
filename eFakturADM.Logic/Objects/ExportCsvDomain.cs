using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class ExportCsvDomain
    {
        public int TotalItems { get; set; }
    }

    public partial class ExportCsvDomain
    {
        public string RowData { get; set; }
        public string RowData2 { get; set; }
        public string RowData3 { get; set; }
        public string Marker { get; set; }
    }
    public partial class ExportCsvDomain : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;

            RowData = DBUtil.GetCharField(dr, "RowData");
            RowData2 = DBUtil.GetCharField(dr, "RowData2");
            RowData3 = DBUtil.GetCharField(dr, "RowData3");
            Marker = DBUtil.GetCharField(dr, "Marker");

            IsValid = true;
            return IsValid;
        }
    }
}
