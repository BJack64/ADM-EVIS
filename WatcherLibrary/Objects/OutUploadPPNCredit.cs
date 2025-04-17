namespace WatcherLibrary.Objects
{
    public class OutUploadPPNCredit
    {
        public string IDNo { get; set; }
        public string Confirm { get; set; }
        public string Message { get; set; }
        /// <summary>
        /// AccountingDocNo Credit dari SAP
        /// </summary>
        public string AccountingDocNo { get; set; }
        /// <summary>
        /// FiscalYear Credit dari SAP
        /// </summary>
        public string FiscalYear { get; set; }
    }
    public class OutUploadPPNCreditItem
    {
        public string FP { get; set; }
        public string NPWP { get; set; }
        public string PembetulanKe { get; set; }
        public string MasaPajakBulan { get; set; }
        public string MasaPajakTahun { get; set; }
        public string AccountingDocDebet { get; set; }
        public string FiscalYearDebet { get; set; }
        public string LineItem { get; set; }
        public string GLAccount { get; set; }
        public string AmountPPN { get; set; }
        public string Message { get; set; }
    }
}
