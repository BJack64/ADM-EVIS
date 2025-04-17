using System.Xml.Serialization;
using System.Collections.Generic;

namespace eFakturADM.DJPLib.Objects
{

    [XmlRoot(ElementName = "detailTransaksi")]
    public class DetailTransaksi
    {
        [XmlElement(ElementName = "nama")]
        public string Nama { get; set; }
        [XmlElement(ElementName = "hargaSatuan")]
        public string HargaSatuan { get; set; }
        [XmlElement(ElementName = "jumlahBarang")]
        public string JumlahBarang { get; set; }
        [XmlElement(ElementName = "hargaTotal")]
        public string HargaTotal { get; set; }
        [XmlElement(ElementName = "diskon")]
        public string Diskon { get; set; }
        [XmlElement(ElementName = "dpp")]
        public string Dpp { get; set; }
        [XmlElement(ElementName = "ppn")]
        public string Ppn { get; set; }
        [XmlElement(ElementName = "tarifPpnbm")]
        public string TarifPpnbm { get; set; }
        [XmlElement(ElementName = "ppnbm")]
        public string Ppnbm { get; set; }
    }

    [XmlRoot(ElementName = "resValidateFakturPm")]
    public class ResValidateFakturPm
    {
        [XmlElement(ElementName = "kdJenisTransaksi")]
        public string KdJenisTransaksi { get; set; }
        [XmlElement(ElementName = "fgPengganti")]
        public string FgPengganti { get; set; }
        [XmlElement(ElementName = "nomorFaktur")]
        public string NomorFaktur { get; set; }
        [XmlElement(ElementName = "tanggalFaktur")]
        public string TanggalFaktur { get; set; }
        [XmlElement(ElementName = "npwpPenjual")]
        public string NpwpPenjual { get; set; }
        [XmlElement(ElementName = "namaPenjual")]
        public string NamaPenjual { get; set; }
        [XmlElement(ElementName = "alamatPenjual")]
        public string AlamatPenjual { get; set; }
        [XmlElement(ElementName = "npwpLawanTransaksi")]
        public string NpwpLawanTransaksi { get; set; }
        [XmlElement(ElementName = "namaLawanTransaksi")]
        public string NamaLawanTransaksi { get; set; }
        [XmlElement(ElementName = "alamatLawanTransaksi")]
        public string AlamatLawanTransaksi { get; set; }
        [XmlElement(ElementName = "jumlahDpp")]
        public string JumlahDpp { get; set; }
        [XmlElement(ElementName = "jumlahPpn")]
        public string JumlahPpn { get; set; }
        [XmlElement(ElementName = "jumlahPpnBm")]
        public string JumlahPpnBm { get; set; }
        [XmlElement(ElementName = "statusApproval")]
        public string StatusApproval { get; set; }
        [XmlElement(ElementName = "statusFaktur")]
        public string StatusFaktur { get; set; }
        [XmlElement(ElementName = "detailTransaksi")]
        public List<DetailTransaksi> DetailTransaksi { get; set; }
        [XmlElement(ElementName = "referensi")]
        public string Referensi { get; set; }
    }

}
