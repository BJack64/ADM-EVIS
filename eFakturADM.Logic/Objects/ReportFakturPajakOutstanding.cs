using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class ReportFakturPajakOutstanding
    {
        public int TotalItems { get; set; }
        public string TglFakturString { get; set; }
        public string PostingDateString { get; set; }
        public string AmountLocalString { get; set; }
        public string AmountDocCurrencyString { get; set; }
    }

    public partial class ReportFakturPajakOutstanding
    {
        public int VSequenceNumber { get; set; }
        public DateTime PostingDate { get; set; }
        public string AccountingDocNo { get; set; }
        public string ItemNo { get; set; }
        public string ItemText { get; set; }
        public string TaxInvoiceNumber { get; set; }
        public DateTime? TglFaktur { get; set; }
        public string DocumentHeaderText { get; set; }
        public string AssignmentNo { get; set; }
        public long Id { get; set; }
        public decimal AmountLocal { get; set; }
        public int? StatusReconcile { get; set; }
        public int FiscalYearDebet { get; set; }
        public string GLAccount { get; set; }
        public decimal? AmountDocCurrency { get; set; }
        public string UserName { get; set; }
    }
    public partial class ReportFakturPajakOutstanding : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;

            VSequenceNumber = DBUtil.GetIntField(dr, "VSequenceNumber");
            PostingDate = DBUtil.GetDateTimeField(dr, "PostingDate");
            AccountingDocNo = DBUtil.GetCharField(dr, "AccountingDocNo");
            ItemNo = DBUtil.GetCharField(dr, "ItemNo");
            ItemText = DBUtil.GetCharField(dr, "ItemText");
            TglFaktur = DBUtil.GetDateTimeNullField(dr, "TglFaktur");
            DocumentHeaderText = DBUtil.GetCharField(dr, "DocumentHeaderText");
            AssignmentNo = DBUtil.GetCharField(dr, "AssignmentNo");
            Id = DBUtil.GetLongField(dr, "Id");
            StatusReconcile = DBUtil.GetIntNullField(dr, "StatusReconcile");
            FiscalYearDebet = DBUtil.GetIntField(dr, "FiscalYearDebet");
            GLAccount = DBUtil.GetCharField(dr, "GLAccount");
            TotalItems = DBUtil.GetIntField(dr, "TotalItems");
            TaxInvoiceNumber = DBUtil.GetCharField(dr, "TaxInvoiceNumber");
            AmountLocal = DBUtil.GetDecimalField(dr, "AmountLocal");
            AmountDocCurrency = DBUtil.GetDecimalNullField(dr, "AmountDocCurrency");

            UserName = DBUtil.GetCharField(dr, "UserName");

            TglFakturString = ConvertHelper.DateTimeConverter.ToShortDateString(TglFaktur);  
            
            PostingDateString = ConvertHelper.DateTimeConverter.ToShortDateString(PostingDate);
            AmountLocalString = ConvertHelper.DecimalConverter.ToString(AmountLocal, 2);
            AmountDocCurrencyString = ConvertHelper.DecimalConverter.ToString(AmountDocCurrency, 2);
            
            IsValid = true;
            return IsValid;
        }
    }
}
