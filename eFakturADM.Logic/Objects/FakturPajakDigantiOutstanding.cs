using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public class FakturPajakDigantiOutstanding : ApplicationObject, IApplicationObject
    {
        public int VSequenceNumber { get; set; }
        public long Id { get; set; }
        public string NoFakturPajak { get; set; }
        public string FormatedNoFaktur { get; set; }
        public int? MasaPajak { get; set; }
        public int? TahunPajak { get; set; }
        public string NPWPPenjual { get; set; }
        public string NamaPenjual { get; set; }
        public DateTime TglFaktur { get; set; }
        public decimal? JumlahDPP { get; set; }
        public decimal? JumlahPPN { get; set; }
        public decimal? JumlahPPNBM { get; set; }
        public string StatusFaktur { get; set; }
        public string StatusApproval { get; set; }
        /// <summary>
        /// Keterangan ketika upload Faktur Pajak Diganti Outstanding
        /// dari excel
        /// </summary>
        public string KeteranganDjp { get; set; }
        /// <summary>
        /// Keterangan yang di set dari EVIS
        /// </summary>
        public string Keterangan { get; set; }
        public int StatusOutstanding { get; set; }
        public string StatusOutstandingName { get; set; }
        public string ScanByUsername { get; set; }
        public string ScanByUserInitial { get; set; }
        public string FormatedNpwpPenjual { get; set; }
        public string MasaPajakName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public int TotalItems { get; set; }

        public string FillingIndex { get; set; }
        public string TglFakturString { get; set; }
        public string DPPString { get; set; }
        public string PPNString { get; set; }
        public string PPNBMString { get; set; }
        public int? OriginTahunPajak { get; set; }
        public int? OriginMasaPajak { get; set; }
        public string OriginMasaPajakName { get; set; }

        public bool Load(IDataReader dr)
        {
            IsValid = false;
            VSequenceNumber = DBUtil.GetIntField(dr, "VSequenceNumber");
            Id = DBUtil.GetLongField(dr, "Id");
            NoFakturPajak = DBUtil.GetCharField(dr, "NoFakturPajak");
            FormatedNoFaktur = DBUtil.GetCharField(dr, "FormatedNoFaktur");
            MasaPajak = DBUtil.GetIntNullField(dr, "MasaPajak");
            TahunPajak = DBUtil.GetIntNullField(dr, "TahunPajak");
            NPWPPenjual = DBUtil.GetCharField(dr, "NPWPPenjual");
            NamaPenjual = DBUtil.GetCharField(dr, "NamaPenjual");
            TglFaktur = DBUtil.GetDateTimeField(dr, "TglFaktur");
            JumlahDPP = DBUtil.GetDecimalNullField(dr, "JumlahDPP");
            JumlahPPN = DBUtil.GetDecimalNullField(dr, "JumlahPPN");
            JumlahPPNBM = DBUtil.GetDecimalNullField(dr, "JumlahPPNBM");

            StatusFaktur = DBUtil.GetCharField(dr, "StatusFaktur");
            StatusApproval = DBUtil.GetCharField(dr, "StatusApproval");
            KeteranganDjp = DBUtil.GetCharField(dr, "KeteranganDjp");
            Keterangan = DBUtil.GetCharField(dr, "Keterangan");
            StatusOutstanding = DBUtil.GetIntField(dr, "StatusOutstanding");
            StatusOutstandingName = DBUtil.GetCharField(dr, "StatusOutstandingName");

            ScanByUsername = DBUtil.GetCharField(dr, "ScanByUsername");
            ScanByUserInitial = DBUtil.GetCharField(dr, "ScanByUserInitial");
            FormatedNpwpPenjual = DBUtil.GetCharField(dr, "FormatedNpwpPenjual");
            MasaPajakName = DBUtil.GetCharField(dr, "MasaPajakName");

            IsDeleted = DBUtil.GetBoolField(dr, "IsDeleted");
            Created = DBUtil.GetDateTimeField(dr, "Created");
            Modified = DBUtil.GetDateTimeNullField(dr, "Modified");
            CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            ModifiedBy = DBUtil.GetCharField(dr, "ModifiedBy");

            TotalItems = DBUtil.GetIntField(dr, "TotalItems");

            FillingIndex = DBUtil.GetCharField(dr, "FillingIndex");
            TglFakturString = ConvertHelper.DateTimeConverter.ToShortDateString(TglFaktur);

            OriginTahunPajak = DBUtil.GetIntNullField(dr, "OriginTahunPajak");
            OriginMasaPajak = DBUtil.GetIntNullField(dr, "OriginMasaPajak");
            OriginMasaPajakName = DBUtil.GetCharField(dr, "OriginMasaPajakName");

            DPPString = ConvertHelper.DecimalConverter.ToString(JumlahDPP, 2);
            PPNString = ConvertHelper.DecimalConverter.ToString(JumlahPPN, 2);
            PPNBMString = ConvertHelper.DecimalConverter.ToString(JumlahPPNBM, 2);

            IsValid = true;
            return IsValid;
        }

        public static object GetList(Filter filter, out int totalItems, string noFaktur1, string noFaktur2, DateTime? tglFakturStart, DateTime? tglFakturEnd, string nPWP, string nama, int? iStatus, string formatedNpwpPenjual, string namaPenjual, string formatedNoFaktur, string tglFakturString, string masaPajakName, string tahunPajak, string dPPString, string pPNString, string pPNBMString, string statusFaktur, int? idataType, DateTime? dscanDateAwal, DateTime? dscanDateAkhir, string ifillingIndex, string sFillingIndex, string sUserName)
        {
            throw new NotImplementedException();
        }
    }
}
