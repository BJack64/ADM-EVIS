using System.Collections.Generic;
namespace eFakturADM.Web.Models
{
    public class DataCompareEvisVsIwsInputNotesDialogModel
    {
        public long Id { get; set; }
        public string Notes { get; set; }
    }

    public class DataCompareEvisVsSapInputNotesDialogModel
    {
        public long Id { get; set; }
        public string Notes { get; set; }
    }

    public class DataCompareEvisVsSapInfoModel
    {
        public List<DataCompareEvisVsSapModel> ListCompare { get; set; }
        public string SubmitType { get; set; }
    }

    public class DataCompareEvisVsSapModel
    {
        public string Id { get; set; }
        public string RowNum { get; set; }
        public string PostingDate { get; set; }
        public string AccountingDocNo { get; set; }
        public string ItemNo { get; set; }
        public string TglFaktur { get; set; }
        public string TaxInvoiceNumberEVIS { get; set; }
        public string TaxInvoiceNumberSAP { get; set; }
        public string DocumentHeaderText { get; set; }
        public string NPWP { get; set; }
        public string NPWPPenjual { get; set; }
        public string AmountEVIS { get; set; }
        public string AmountSAP { get; set; }
        public string AmountDiff { get; set; }
        public string StatusCompare { get; set; }
        public string Notes { get; set; }
        public string IdNo { get; set; }
        public string Pembetulan { get; set; }
        public string MasaPajak { get; set; }
        public string TahunPajak { get; set; }
        public string ItemText { get; set; }
        public string FiscalYearDebet { get; set; }
        public string GLAccount { get; set; }
        public string DocumentStatus { get; set; }
        public string StatusFaktur { get; set; }
        public int? FPType { get; set; }
    }

    public class EvisVsSapSubmitAllModel
    {
        public string SubmitType { get; set; }
        public string TglPostingStart { get; set; }
        public string TglPostingEnd { get; set; }
        public string TglFakturStart { get; set; }
        public string TglFakturEnd { get; set; }
        public string NoFakturStart { get; set; }
        public string NoFakturEnd { get; set; }
        public string ScanDate { get; set; }
        public string MasaPajak { get; set; }
        public string TahunPajak { get; set; }
        public string UserName { get; set; }
        public string StatusId { get; set; }
        public string StatusPosting { get; set; }
    }


}