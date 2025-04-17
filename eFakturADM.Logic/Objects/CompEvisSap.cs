using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{

    public partial class CompEvisSap
    {
        public int TotalItems { get; set; }
        public string PostingDateString { get; set; }
        public string TglFakturString { get; set; }
        public string ScanDateString { get; set; }
        public string AmountEvisString { get; set; }
        public string AmountSapString { get; set; }
        public string AmountDiffString { get; set; }
    }

    public partial class CompEvisSap
    {
        public long Id { get; set; }
        public int VSequence { get; set; }
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
        public string NPWPPenjual { get; set; }
        public decimal? AmountEvis { get; set; }
        public decimal? AmountSap { get; set; }
        public decimal? AmountDiff { get; set; }
        public string StatusCompare { get; set; }
        public string Notes { get; set; }
        public string UserNameCreator { get; set; }
        public string ItemText { get; set; }
        public int? MasaPajak { get; set; }
        public int? TahunPajak { get; set; }
        public int Pembetulan { get; set; }
        public int? FiscalYearDebet { get; set; }
        public string GLAccount { get; set; }
        public int DocumentStatus { get; set; }
        public string StatusFaktur { get; set; }
        public int? FPType { get; set; }

        public string StatusPosting { get; set; }
    }

    public partial class CompEvisSap : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;
            Id = DBUtil.GetLongField(dr, "Id");
            VSequence = DBUtil.GetIntField(dr, "vSequence");
            PostingDate = DBUtil.GetDateTimeNullField(dr, "PostingDate");
            AccountingDocNo = DBUtil.GetCharField(dr, "AccountingDocNo");
            ItemNo = DBUtil.GetCharField(dr, "ItemNo");
            TglFaktur = DBUtil.GetDateTimeNullField(dr, "TglFaktur");
            NamaVendor = DBUtil.GetCharField(dr, "NamaVendor");
            ScanDate = DBUtil.GetDateTimeNullField(dr, "ScanDate");
            TaxInvoiceNumberEvis = DBUtil.GetCharField(dr, "TaxInvoiceNumberEvis");
            TaxInvoiceNumberSap = DBUtil.GetCharField(dr, "TaxInvoiceNumberSap");
            DocumentHeaderText = DBUtil.GetCharField(dr, "DocumentHeaderText");
            Npwp = DBUtil.GetCharField(dr, "NPWP");
            NPWPPenjual = DBUtil.GetCharField(dr, "NPWPPenjual");
            AmountEvis = DBUtil.GetDecimalNullField(dr, "AmountEvis");
            AmountSap = DBUtil.GetDecimalNullField(dr, "AmountSap");
            AmountDiff = DBUtil.GetDecimalNullField(dr, "AmountDiff");

            StatusCompare = DBUtil.GetCharField(dr, "StatusCompare");
            Notes = DBUtil.GetCharField(dr, "Notes");

            TotalItems = DBUtil.GetIntField(dr, "TotalItems");
            PostingDateString = ConvertHelper.DateTimeConverter.ToShortDateString(PostingDate);
            TglFakturString = ConvertHelper.DateTimeConverter.ToShortDateString(TglFaktur);
            ScanDateString = ConvertHelper.DateTimeConverter.ToShortDateString(ScanDate);
            AmountEvisString = ConvertHelper.DecimalConverter.ToString(AmountEvis);
            AmountSapString = ConvertHelper.DecimalConverter.ToString(AmountSap);
            AmountDiffString = ConvertHelper.DecimalConverter.ToString(AmountDiff);

            UserNameCreator = DBUtil.GetCharField(dr, "UserNameCreator");
            ItemText = DBUtil.GetCharField(dr, "ItemText");
            Pembetulan = DBUtil.GetIntField(dr, "Pembetulan");
            MasaPajak = DBUtil.GetIntNullField(dr, "MasaPajak");
            TahunPajak = DBUtil.GetIntNullField(dr, "TahunPajak");

            FiscalYearDebet = DBUtil.GetIntNullField(dr, "FiscalYearDebet");
            GLAccount = DBUtil.GetCharField(dr, "GLAccount");

            DocumentStatus = DBUtil.GetIntField(dr, "DocumentStatus");
            StatusFaktur = DBUtil.GetCharField(dr, "StatusFaktur");

            FPType = DBUtil.GetIntNullField(dr, "FPType");
            StatusPosting = DBUtil.GetCharField(dr, "StatusPosting");

            IsValid = true;
            return IsValid;
        }
    }

}
