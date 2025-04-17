using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class VwDataIWSReqEfis
    {
        public string InvoiceId { get; set; }
        public DateTime? ReceivingDate { get; set; }
        public string Status { get; set; }
        public string Activity { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string TaxVoucherNo { get; set; }
        public string InvoiceNo { get; set; }
        public decimal? Ppn { get; set; }

    }
    public partial class VwDataIWSReqEfis : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;
            InvoiceId = DBUtil.GetCharField(dr, "INVOICEID");
            ReceivingDate = DBUtil.GetDateTimeNullField(dr, "RECEIVINGDATE");
            Status = DBUtil.GetCharField(dr, "STATUS");
            Activity = DBUtil.GetCharField(dr, "ACTIVITY");
            InvoiceDate = DBUtil.GetDateTimeNullField(dr, "INVOICEDATE");
            VendorId = DBUtil.GetCharField(dr, "VENDORID");
            VendorName = DBUtil.GetCharField(dr, "VENDORNAME");
            TaxVoucherNo = DBUtil.GetCharField(dr, "TAXVOUCHERNO");
            InvoiceNo = DBUtil.GetCharField(dr, "INVOICENO");
            Ppn = DBUtil.GetDecimalNullField(dr, "PPN");
            IsValid = true;
            return IsValid;
        }
    }
}
