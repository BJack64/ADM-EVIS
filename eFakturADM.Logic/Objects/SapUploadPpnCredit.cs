using System.Data;
using eFakturADM.Logic.Core;

namespace eFakturADM.Logic.Objects
{
    public partial class SapUploadPpnCredit
    {
        public string FakturPajak { get; set; }
        public string Npwp { get; set; }
        public int PembetulanKe { get; set; }
        public int MasaPajakBulan { get; set; }
        public int MasaPajakTahun { get; set; }
        public string AccountingDocDebet { get; set; }
        public int FiscalYearDebet { get; set; }
        public string LineItem { get; set; }
        public string GlAccount { get; set; }
        public decimal AmountPpn { get; set; }
    }

    public partial class SapUploadPpnCredit : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            throw new System.NotImplementedException();
        }
    }

}
