using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class ReportDetailFakturPajak
    {
        public int TotalItems { get; set; }
        public string HargaSatuanstring { get; set; }
        public string JumlahBarangstring { get; set; }
        public string HargaTotalstring { get; set; }
        public string Diskonstring { get; set; }
        public string Dppstring { get; set; }
        public string Ppnstring { get; set; }
        public string TarifPpnbmstring { get; set; }
        public string Ppnbmstring { get; set; }
        public string JumlahDppString { get; set; }
        public string JumlahPpnString { get; set; }
        public string JumlahPpnBmString { get; set; }

    }
    public partial class ReportDetailFakturPajak
    {
        public int VSequenceNumber { get; set; }
        //Faktur Pajak
        public long FakturPajakId { get; set; }
        public string FCode { get; set; }
        public string UrlScan { get; set; }
        public string KdJenisTransaksi { get; set; }
        public string FgPengganti { get; set; }
        public string NoFakturPajak { get; set; }
        public DateTime? TglFaktur { get; set; }
        public string NPWPPenjual { get; set; }
        public string NamaPenjual { get; set; }
        public string AlamatPenjual { get; set; }
        public int? VendorId { get; set; }
        public string NPWPLawanTransaksi { get; set; }
        public string NamaLawanTransaksi { get; set; }
        public string AlamatLawanTransaksi { get; set; }
        public decimal? JumlahDPP { get; set; }
        public decimal? JumlahPPN { get; set; }
        public decimal? JumlahPPNBM { get; set; }
        public string StatusApproval { get; set; }
        public string StatusFaktur { get; set; }
        public bool Dikreditkan { get; set; }
        public int MasaPajak { get; set; }
        public int TahunPajak { get; set; }
        public DateTime? ReceivingDate { get; set; }
        public string Pesan { get; set; }
        public int FPType { get; set; }
        public string FillingIndex { get; set; }
        public int ScanType { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public string TglFakturString { get; set; }
        public string ReceivingDateString { get; set; }
        

        public string FormatedNoFaktur { get; set; }
        public string FormatedNpwpPenjual { get; set; }

        public string FormatedNpwpLawanTransaksi { get; set; }
        
        public int Status { get; set; }
        public string StatusText { get; set; }

        public int? StatusReconcile { get; set; }
        public string Referensi { get; set; }

        //Faktur Pajak Detail

        public long FakturPajakDetailId { get; set; }
        public string Nama { get; set; }
        public decimal? HargaSatuan { get; set; }
        public decimal? JumlahBarang { get; set; }
        public decimal? HargaTotal { get; set; }
        public decimal? Diskon { get; set; }
        public decimal? Dpp { get; set; }
        public decimal? Ppn { get; set; }
        public decimal? TarifPpnbm { get; set; }
        public decimal? Ppnbm { get; set; }

    }

    public partial class ReportDetailFakturPajak : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;

            //FakturPajak
            FakturPajakId = DBUtil.GetLongField(dr, "FakturPajakId");
            FCode = DBUtil.GetCharField(dr, "FCode");
            UrlScan = DBUtil.GetCharField(dr, "UrlScan");
            NPWPPenjual = DBUtil.GetCharField(dr, "NPWPPenjual");
            KdJenisTransaksi = DBUtil.GetCharField(dr, "KdJenisTransaksi");
            FgPengganti = DBUtil.GetCharField(dr, "FgPengganti");
            NoFakturPajak = DBUtil.GetCharField(dr, "NoFakturPajak");
            TglFaktur = DBUtil.GetDateTimeNullField(dr, "TglFaktur");
            NPWPPenjual = DBUtil.GetCharField(dr, "NPWPPenjual");
            NamaPenjual = DBUtil.GetCharField(dr, "NamaPenjual");
            AlamatPenjual = DBUtil.GetCharField(dr, "AlamatPenjual");
            VendorId = DBUtil.GetIntNullField(dr, "VendorId");
            NPWPLawanTransaksi = DBUtil.GetCharField(dr, "NPWPLawanTransaksi");
            NamaLawanTransaksi = DBUtil.GetCharField(dr, "NamaLawanTransaksi");
            AlamatLawanTransaksi = DBUtil.GetCharField(dr, "AlamatLawanTransaksi");
            JumlahDPP = DBUtil.GetDecimalNullField(dr, "JumlahDPP");
            JumlahPPN = DBUtil.GetDecimalNullField(dr, "JumlahPPN");
            JumlahPPNBM = DBUtil.GetDecimalNullField(dr, "JumlahPPNBM");
            StatusApproval = DBUtil.GetCharField(dr, "StatusApproval");
            StatusFaktur = DBUtil.GetCharField(dr, "StatusFaktur");
            Dikreditkan = DBUtil.GetBoolField(dr, "Dikreditkan");
            MasaPajak = DBUtil.GetIntField(dr, "MasaPajak");
            TahunPajak = DBUtil.GetIntField(dr, "TahunPajak");
            ReceivingDate = DBUtil.GetDateTimeNullField(dr, "ReceivingDate");
            Pesan = DBUtil.GetCharField(dr, "Pesan");

            FPType = DBUtil.GetIntField(dr, "FPType");
            FillingIndex = DBUtil.GetCharField(dr, "FillingIndex");
            ScanType = DBUtil.GetIntField(dr, "ScanType");

            IsDeleted = DBUtil.GetBoolField(dr, "IsDeleted");
            Created = DBUtil.GetDateTimeField(dr, "Created");
            Modified = DBUtil.GetDateTimeNullField(dr, "Modified");
            CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            ModifiedBy = DBUtil.GetCharField(dr, "ModifiedBy");

            TotalItems = DBUtil.GetIntField(dr, "TotalItems");

            TglFakturString = ConvertHelper.DateTimeConverter.ToShortDateString(TglFaktur);
            ReceivingDateString = ConvertHelper.DateTimeConverter.ToShortDateString(ReceivingDate);

            JumlahDppString = ConvertHelper.DecimalConverter.ToString(JumlahDPP, 2);
            JumlahPpnString = ConvertHelper.DecimalConverter.ToString(JumlahPPN, 2);
            JumlahPpnBmString = ConvertHelper.DecimalConverter.ToString(JumlahPPNBM, 2);

            FormatedNoFaktur = DBUtil.GetCharField(dr, "FormatedNoFaktur");
            FormatedNpwpPenjual = DBUtil.GetCharField(dr, "FormatedNpwpPenjual");
            FormatedNpwpLawanTransaksi = DBUtil.GetCharField(dr, "FormatedNpwpLawanTransaksi");

            Status = DBUtil.GetIntField(dr, "Status");
            StatusText = DBUtil.GetCharField(dr, "StatusText");
            
            StatusReconcile = DBUtil.GetIntNullField(dr, "StatusReconcile");

            //FakturPajakDetail
            FakturPajakDetailId = DBUtil.GetLongField(dr, "FakturPajakDetailId");
            Nama = DBUtil.GetCharField(dr, "Nama");
            HargaSatuan = DBUtil.GetDecimalNullField(dr, "HargaSatuan");
            JumlahBarang = DBUtil.GetDecimalNullField(dr, "JumlahBarang");
            HargaTotal = DBUtil.GetDecimalNullField(dr, "HargaTotal");
            Diskon = DBUtil.GetDecimalNullField(dr, "Diskon");
            Dpp = DBUtil.GetDecimalNullField(dr, "Dpp");
            Ppn = DBUtil.GetDecimalNullField(dr, "Ppn");
            TarifPpnbm = DBUtil.GetDecimalNullField(dr, "TarifPpnbm");
            Ppnbm = DBUtil.GetDecimalNullField(dr, "Ppnbm");

            HargaSatuanstring = ConvertHelper.DecimalConverter.ToString(HargaSatuan,"-");
            JumlahBarangstring = ConvertHelper.DecimalConverter.ToString(JumlahBarang,"-");
            HargaTotalstring = ConvertHelper.DecimalConverter.ToString(HargaTotal,"-");
            Diskonstring = ConvertHelper.DecimalConverter.ToString(Diskon,"-");
            Dppstring = ConvertHelper.DecimalConverter.ToString(Dpp,"-");
            Ppnstring = ConvertHelper.DecimalConverter.ToString(Ppn,"-");
            TarifPpnbmstring = ConvertHelper.DecimalConverter.ToString(TarifPpnbm,"-");
            Ppnbmstring = ConvertHelper.DecimalConverter.ToString(Ppnbm, "-");

            VSequenceNumber = DBUtil.GetIntField(dr, "VSequenceNumber");

            Referensi = DBUtil.GetCharField(dr, "Referensi");

            IsValid = true;
            return IsValid;
        }
    }
}
