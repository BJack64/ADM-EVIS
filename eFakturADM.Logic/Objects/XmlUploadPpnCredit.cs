using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class XmlUploadPpnCredit
    {
        public long Id { get; set; }
        public DateTime? PostingDate { get; set; }
        public string AccountingDocNo { get; set; }
        public string ItemNo { get; set; }
        public DateTime? TglFaktur { get; set; }
        public string NamaVendor { get; set; }
        public DateTime? ScanDate { get; set; }
        public string TaxInvoiceNumberEvis { get; set; }
        public string TaxInvoiceNumberSap { get; set; }
        public string DocumentHeaderText { get; set; }
        public string Npwp { get; set; }
        public string NpwpPenjual { get; set; }
        public decimal? AmountEvis { get; set; }
        public decimal? AmountSap { get; set; }
        public decimal? AmountDiff { get; set; }
        public string StatusCompare { get; set; }
        public string Notes { get; set; }
        public string UserNameCreator { get; set; }
        
        public int Pembetulan { get; set; }
        public int? MasaPajak { get; set; }
        public int? TahunPajak { get; set; }
        public string ItemText { get; set; }
        public int? FiscalYearDebet { get; set; }
        public string GLAccount { get; set; }
        public string StatusFaktur { get; set; }
        public string FpOriginal { get; set; }
        public string NpwpOriginal { get; set; }
        public int? PembetulanOriginal { get; set; }
        public string AccountingDocDebetOriginal { get; set; }
        public int? FiscalYearDebetOriginal { get; set; }
        public string GLAccountOriginal { get; set; }
        public decimal? AmountPPNOriginal { get; set; }
        public int TotalItems { get; set; }
        public string ItemTextOriginal { get; set; }

        public string ItemText2 { get; set; }
        public string NpwpPenjual2 { get; set; }
        public decimal? AmountEvis2 { get; set; }
        public int? MasaPajak2 { get; set; }
        public int? TahunPajak2 { get; set; }
    }


    public partial class XmlUploadPpnCredit : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;
            Id = DBUtil.GetLongField(dr, "Id");
            PostingDate = DBUtil.GetDateTimeNullField(dr, "PostingDate");
            AccountingDocNo = DBUtil.GetCharField(dr, "AccountingDocNo");
            ItemNo = DBUtil.GetCharField(dr, "ItemNo");
            TglFaktur = DBUtil.GetDateTimeNullField(dr, "TglFaktur");
            NamaVendor = DBUtil.GetCharField(dr, "NamaVendor");
            ScanDate = DBUtil.GetDateTimeNullField(dr, "ScanDate");
            TaxInvoiceNumberEvis = DBUtil.GetCharField(dr, "TaxInvoiceNumberEvis");
            TaxInvoiceNumberSap = DBUtil.GetCharField(dr, "TaxInvoiceNumberSap");
            DocumentHeaderText = DBUtil.GetCharField(dr, "DocumentHeaderText");
            Npwp = DBUtil.GetCharField(dr, "Npwp");
            NpwpPenjual = DBUtil.GetCharField(dr, "NpwpPenjual");
            AmountEvis = DBUtil.GetDecimalNullField(dr, "AmountEvis");
            AmountSap = DBUtil.GetDecimalNullField(dr, "AmountSap");
            AmountDiff = DBUtil.GetDecimalNullField(dr, "AmountDiff");
            StatusCompare = DBUtil.GetCharField(dr, "StatusCompare");
            Notes = DBUtil.GetCharField(dr, "Notes");
            TotalItems = DBUtil.GetIntField(dr, "TotalItems");
            UserNameCreator = DBUtil.GetCharField(dr, "UserNameCreator");
            ItemText = DBUtil.GetCharField(dr, "ItemText");
            Pembetulan = DBUtil.GetIntField(dr, "Pembetulan");
            MasaPajak = DBUtil.GetIntNullField(dr, "MasaPajak");
            TahunPajak = DBUtil.GetIntNullField(dr, "TahunPajak");
            FiscalYearDebet = DBUtil.GetIntNullField(dr, "FiscalYearDebet");
            GLAccount = DBUtil.GetCharField(dr, "GLAccount");
            StatusFaktur = DBUtil.GetCharField(dr, "StatusFaktur");

            FpOriginal = DBUtil.GetCharField(dr, "FpOriginal");
            NpwpOriginal = DBUtil.GetCharField(dr, "NpwpOriginal");
            PembetulanOriginal = DBUtil.GetIntNullField(dr, "PembetulanOriginal");
            AccountingDocDebetOriginal = DBUtil.GetCharField(dr, "AccountingDocDebetOriginal");
            FiscalYearDebetOriginal = DBUtil.GetIntNullField(dr, "FiscalYearDebetOriginal");
            GLAccountOriginal = DBUtil.GetCharField(dr, "GLAccountOriginal");
            AmountPPNOriginal = DBUtil.GetDecimalNullField(dr, "AmountPPNOriginal");
            ItemTextOriginal = DBUtil.GetCharField(dr, "ItemTextOriginal");

            //ItemText2 = DBUtil.GetCharField(dr, "ItemText2");
            //NpwpPenjual2 = DBUtil.GetCharField(dr, "NpwpPenjual2");
            //AmountEvis2 = DBUtil.GetDecimalNullField(dr, "AmountEvis2");
            //MasaPajak2 = DBUtil.GetIntNullField(dr, "MasaPajak2");
            //TahunPajak2 = DBUtil.GetIntNullField(dr, "TahunPajak2");

            IsValid = true;
            return IsValid;
        }
    }

}
