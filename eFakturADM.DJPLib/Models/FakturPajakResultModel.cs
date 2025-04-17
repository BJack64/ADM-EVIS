using System.Xml.Serialization;
using System.Collections.Generic;

namespace eFakturADM.DJPLib.Models
{

    public class DetailTransaksi
    {
        public string Nama { get; set; }
        public string HargaSatuan { get; set; }
        public string JumlahBarang { get; set; }
        public string HargaTotal { get; set; }
        public string Diskon { get; set; }
        public string Dpp { get; set; }
        public string Ppn { get; set; }
        public string TarifPpnbm { get; set; }
        public string Ppnbm { get; set; }
    }

    public class DataFakturPajak
    {
        public string KdJenisTransaksi { get; set; }
        public string FgPengganti { get; set; }
        public string NomorFakturPajak { get; set; }
        public string NomorFakturPajakFormatted { get; set; }
        public string TglFaktur { get; set; }
        public string NpwpPenjual { get; set; }
        public string NpwpPenjualFormatted { get; set; }
        public string NamaPenjual { get; set; }
        public string AlamatPenjual { get; set; }
        public string NpwpLawanTransaksi { get; set; }
        public string NpwpLawanTransaksiFormatted { get; set; }
        public string NamaLawanTransaksi { get; set; }
        public string AlamatLawanTransaksi { get; set; }
        public string JumlahDpp { get; set; }
        public string JumlahPpn { get; set; }
        public string JumlahPpnBm { get; set; }
        public string StatusApproval { get; set; }
        public string StatusFaktur { get; set; }
        public List<DetailTransaksi> DetailTransaksi { get; set; }
        public string Referensi { get; set; }
    }

    public class FakturPajakResultModel
    {
        public string FPdjpID { get; set; }
        public string StatusValidasi { get; set; }
        //public string NamaValidasi { get; set; }
        public string KeteranganValidasi { get; set; }
        public List<DataFakturPajak> DataFakturPajak { get; set; }
    }
}
