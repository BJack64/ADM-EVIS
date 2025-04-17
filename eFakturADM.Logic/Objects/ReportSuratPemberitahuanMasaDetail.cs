using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{

    public partial class ReportSuratPemberitahuanMasaDetail
    {
        public string TglFakturString { get; set; }
        public string CreatedString { get; set; }
        public string ModifiedString { get; set; }
        public string JumlahDppString { get; set; }
        public string JumlahPpnString { get; set; }
        public string JumlahPpnBmString { get; set; }
        public string FgUangMukaString { get; set; }
        public string UangMukaDPPString { get; set; }
        public string UangMukaPPNString { get; set; }
        public string UangMukaPPnBMString { get; set; }
    }

    public partial class ReportSuratPemberitahuanMasaDetail
    {
        public long Id { get; set; }
        public long ReportSPMId { get; set; }
        public string FCode { get; set; }
        public string KdJenisTransaksi { get; set; }
        public string FgPengganti { get; set; }
        public string NoFakturPajak { get; set; }
        public DateTime? TglFaktur { get; set; }
        public string NPWPPenjual { get; set; }
        public string NamaPenjual { get; set; }
        public string AlamatPenjual { get; set; }
        public string NPWPLawanTransaksi { get; set; }
        public string NamaLawanTransaksi { get; set; }
        public string AlamatLawanTransaksi { get; set; }
        public decimal? JumlahDPP { get; set; }
        public decimal? JumlahPPN { get; set; }
        public decimal? JumlahPPNBM { get; set; }
        public string KeteranganTambahan { get; set; }
        public decimal? FgUangMuka { get; set; }
        public decimal? UangMukaDPP { get; set; }
        public decimal? UangMukaPPN { get; set; }
        public decimal? UangMukaPPnBM { get; set; }
        public string FillingIndex { get; set; }
        public string Referensi { get; set; }
        public string FormatedNoFaktur { get; set; }
        public string FormatedNpwpPenjual { get; set; }
        public string FormatedNpwpLawanTransaksi { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public int Versi { get; set; }
        public int VSequenceNumber { get; set; }

        public int MasaPajak { get; set; }
        public int TahunPajak { get; set; }
        public string MasaPajakName { get; set; }
        public DateTime? FPCreatedDate { get; set; }
        
    }

    public partial class ReportSuratPemberitahuanMasaDetail : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;
            Versi = DBUtil.GetIntField(dr, "Versi");
            VSequenceNumber = DBUtil.GetIntField(dr, "VSequenceNumber");

            Id = DBUtil.GetLongField(dr, "Id");
            ReportSPMId = DBUtil.GetLongField(dr, "ReportSPMId");
            FCode = DBUtil.GetCharField(dr, "FCode");
            NPWPPenjual = DBUtil.GetCharField(dr, "NPWPPenjual");
            KdJenisTransaksi = DBUtil.GetCharField(dr, "KdJenisTransaksi");
            FgPengganti = DBUtil.GetCharField(dr, "FgPengganti");
            NoFakturPajak = DBUtil.GetCharField(dr, "NoFakturPajak");
            TglFaktur = DBUtil.GetDateTimeNullField(dr, "TglFaktur");
            NamaPenjual = DBUtil.GetCharField(dr, "NamaPenjual");
            AlamatPenjual = DBUtil.GetCharField(dr, "AlamatPenjual");
            NPWPLawanTransaksi = DBUtil.GetCharField(dr, "NPWPLawanTransaksi");
            NamaLawanTransaksi = DBUtil.GetCharField(dr, "NamaLawanTransaksi");
            AlamatLawanTransaksi = DBUtil.GetCharField(dr, "AlamatLawanTransaksi");
            JumlahDPP = DBUtil.GetDecimalNullField(dr, "JumlahDPP");
            JumlahPPN = DBUtil.GetDecimalNullField(dr, "JumlahPPN");
            JumlahPPNBM = DBUtil.GetDecimalNullField(dr, "JumlahPPNBM");
            FillingIndex = DBUtil.GetCharField(dr, "FillingIndex");
            FormatedNoFaktur = DBUtil.GetCharField(dr, "FormatedNoFaktur");
            FormatedNpwpPenjual = DBUtil.GetCharField(dr, "FormatedNpwpPenjual");
            FormatedNpwpLawanTransaksi = DBUtil.GetCharField(dr, "FormatedNpwpLawanTransaksi");
            Referensi = DBUtil.GetCharField(dr, "Referensi");
            KeteranganTambahan = DBUtil.GetCharField(dr, "KeteranganTambahan");
            FgUangMuka = DBUtil.GetDecimalNullField(dr, "FgUangMuka");
            UangMukaDPP = DBUtil.GetDecimalNullField(dr, "UangMukaDPP");
            UangMukaPPN = DBUtil.GetDecimalNullField(dr, "UangMukaPPN");
            UangMukaPPnBM = DBUtil.GetDecimalNullField(dr, "UangMukaPPnBM");

            IsDeleted = DBUtil.GetBoolField(dr, "IsDeleted");
            Created = DBUtil.GetDateTimeField(dr, "Created");
            Modified = DBUtil.GetDateTimeNullField(dr, "Modified");
            CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            ModifiedBy = DBUtil.GetCharField(dr, "ModifiedBy");
            FPCreatedDate = DBUtil.GetDateTimeNullField(dr, "FPCreatedDate");

            FgUangMukaString = ConvertHelper.DecimalConverter.ToString(FgUangMuka, "-");
            UangMukaDPPString = ConvertHelper.DecimalConverter.ToString(UangMukaDPP, "-");
            UangMukaPPNString = ConvertHelper.DecimalConverter.ToString(UangMukaPPN, "-");
            UangMukaPPnBMString = ConvertHelper.DecimalConverter.ToString(UangMukaPPnBM, "-");
            TglFakturString = ConvertHelper.DateTimeConverter.ToShortDateString(TglFaktur);
            JumlahDppString = ConvertHelper.DecimalConverter.ToString(JumlahDPP, 2);
            JumlahPpnString = ConvertHelper.DecimalConverter.ToString(JumlahPPN, 2);
            JumlahPpnBmString = ConvertHelper.DecimalConverter.ToString(JumlahPPNBM, 2);
            CreatedString = ConvertHelper.DateTimeConverter.ToShortDateString(Created);
            ModifiedString = ConvertHelper.DateTimeConverter.ToShortDateString(Modified);

            MasaPajak = DBUtil.GetIntField(dr, "MasaPajak");
            TahunPajak = DBUtil.GetIntField(dr, "TahunPajak");
            MasaPajakName = DBUtil.GetCharField(dr, "MasaPajakName");

            IsValid = true;
            return IsValid;
        }
    }

}
