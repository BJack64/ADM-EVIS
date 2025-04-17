namespace eFakturADM.Web.Models
{
    public class FakturPajakReturInfoModel
    {
        public long FakturPajakReturId { get; set; }
        public int FakturPajakId { get; set; }
        public string Fcode { get; set; }        
        public string NoDocRetur { get; set; }
        public string TglRetur { get; set; }
        public string MasaPajakLapor { get; set; }
        public string TahunPajakLapor { get; set; }
        public string JumlahDPP { get; set; }
        public string JumlahPPN { get; set; }
        public string JumlahPPNBM { get; set; }
        public string Pesan { get; set; }

        public string NoFaktur { get; set; }
        public string NamaVendor { get; set; }
        public string NPWPVendor { get; set; }
        public string AlamatVendor { get; set; }
        public string TanggalFaktur { get; set; }
        public string KdJenisTransaksi { get; set; }
        public string FgPengganti { get; set; }
        public string Dikreditkan { get; set; }
    }
}