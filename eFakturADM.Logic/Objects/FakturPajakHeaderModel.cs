using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eFakturADM.Logic.Objects
{
    public class FakturPajakHeaderModel
    {
        public long ObjectID { get; set; }
        public string CertificateID { get; set; }
        public string StatusPayment { get; set; }
        public string Remark { get; set; }
        public string UrlScan { get; set; }
        public string JenisTransaksi { get; set; }
        public string JenisDokumen { get; set; }
        public string KdJenisTransaksi { get; set; }
        public string FgPengganti { get; set; }
        public string NoFakturPajak { get; set; }
        public DateTime TglFaktur { get; set; }
        public string NPWPPenjual { get; set; }
        public string NamaPenjual { get; set; }
        public string AlamatPenjual { get; set; }
        public string NPWPLawanTransaksi { get; set; }
        public string NamaLawanTransaksi { get; set; }
        public string AlamatLawanTransaksi { get; set; }
        public decimal JumlahDPP { get; set; }
        public decimal JumlahPPN { get; set; }
        public decimal JumlahPPNBM { get; set; }
        public string StatusApproval { get; set; }
        public string StatusFaktur { get; set; }
        public string Referensi { get; set; }

        public string CreatedBy { get; set; }
        public string Source { get; set; }
    }
}
