using System.Collections.Generic;
using eFakturADM.Logic.Core;

namespace eFakturADM.Web.Models
{
    public class FakturPajakInfoModel
    {
        public long FakturPajakId { get; set; }
        public string UrlScan { get; set; }
        public string KdJenisTransaksi { get; set; }
        public int? VendorId { get; set; }
        public string FgPengganti { get; set; }
        public string NoFakturPajak { get; set; }
        public string TglFaktur { get; set; }
        public string NPWPPenjual { get; set; }
        public string NamaPenjual { get; set; }
        public string AlamatPenjual { get; set; }
        public string NPWPLawanTransaksi { get; set; }
        public string NamaLawanTransaksi { get; set; }
        public string AlamatLawanTransaksi { get; set; }
        public string JumlahDPP { get; set; }
        public string JumlahPPN { get; set; }
        public string JumlahPPNBM { get; set; }
        public string StatusApproval { get; set; }
        public string StatusFaktur { get; set; }
        public string Dikreditkan { get; set; }
        public string MasaPajak { get; set; }
        public string TahunPajak { get; set; }
        public string MasaPajakPengkreditan { get; set; }
        public string TahunPajakPengkreditan { get; set; }
        public string ReceivingDate { get; set; }
        public string Pesan { get; set; }
        public string FPTypeText { get; set; }
        public ApplicationEnums.FPType FPType { get; set; }
        public string ScanTypeText { get; set; }
        public ApplicationEnums.ScanType ScanType { get; set; }
        public string FillingIndex { get; set; }

        public List<DJPLib.Objects.DetailTransaksi> FpDetailTransaksi { get; set; }

        public string FormatedNoFaktur { get; set; }
        public string FormatedNpwpLawanTransaksi { get; set; }
        public string FormatedNpwpPenjual { get; set; }

        public string Referensi { get; set; }

        public string JenisTransaksi { get; set; }
        public string JenisDokumen { get; set; }
        public string NoFakturYangDiganti { get; set; }
    }
}