using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class CompEvisIws
    {
        public int TotalItems { get; set; }
        public string ReceivedDateString { get; set; }
        public string VatAmountScannedString { get; set; }
        public string VatAmountIwsString { get; set; }
        public string VatAmountDiffString { get; set; }
        public string ScanDateString { get; set; }
    }

    public partial class CompEvisIws
    {
        public long Id { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string TaxInvoiceNumberEvis { get; set; }
        public string TaxInvoiceNumberIws { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal? VatAmountScanned { get; set; }
        public decimal? VatAmountIws { get; set; }
        public decimal? VatAmountDiff { get; set; }
        public string StatusDjp { get; set; }
        public string StatusCompare { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string ScanUserName { get; set; }
        public DateTime? ScanDate { get; set; }
    }
    public partial class CompEvisIws : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;
            Id = DBUtil.GetLongField(dr, "Id");
            ReceivedDate = DBUtil.GetDateTimeField(dr, "ReceivedDate");
            VendorCode = DBUtil.GetCharField(dr, "VendorCode");
            VendorName = DBUtil.GetCharField(dr, "VendorName");
            TaxInvoiceNumberEvis = DBUtil.GetCharField(dr, "TaxInvoiceNumberEVIS");
            TaxInvoiceNumberIws = DBUtil.GetCharField(dr, "TaxInvoiceNumberIWS");
            InvoiceNumber = DBUtil.GetCharField(dr, "InvoiceNumber");
            VatAmountScanned = DBUtil.GetDecimalNullField(dr, "VATAmountScanned");
            VatAmountIws = DBUtil.GetDecimalNullField(dr, "VATAmountIWS");
            VatAmountDiff = DBUtil.GetDecimalNullField(dr, "VATAmountDiff");
            StatusDjp = DBUtil.GetCharField(dr, "StatusDJP");
            StatusCompare = DBUtil.GetCharField(dr, "StatusCompare");
            Notes = DBUtil.GetCharField(dr, "Notes");

            IsDeleted = DBUtil.GetBoolField(dr, "IsDeleted");
            Created = DBUtil.GetDateTimeField(dr, "Created");
            Modified = DBUtil.GetDateTimeNullField(dr, "Modified");
            CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            ModifiedBy = DBUtil.GetCharField(dr, "ModifiedBy");
            ScanUserName = DBUtil.GetCharField(dr, "ScanUserName");
            ScanDate = DBUtil.GetDateTimeNullField(dr, "ScanDate");

            TotalItems = DBUtil.GetIntField(dr, "TotalItems");
            ReceivedDateString = ConvertHelper.DateTimeConverter.ToShortDateString(ReceivedDate);
            VatAmountScannedString = ConvertHelper.DecimalConverter.ToString(VatAmountScanned);
            VatAmountIwsString = ConvertHelper.DecimalConverter.ToString(VatAmountIws);
            VatAmountDiffString = ConvertHelper.DecimalConverter.ToString(VatAmountDiff);
            ScanDateString = ConvertHelper.DateTimeConverter.ToShortDateString(ScanDate);

            IsValid = true;
            return IsValid;
        }
    }
}
