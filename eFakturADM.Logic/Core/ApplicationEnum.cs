using System.ComponentModel;

namespace eFakturADM.Logic.Core
{
    public class ApplicationEnums
    {
        public enum GeneralConfig
        {
            [Description("TimeSchedulerWatcherInboxService")]
            TimeSchedulerWatcherInboxService = 1,
            [Description("DataFolderWatcherInboxService")]
            DataFolderWatcherInboxService = 2,
            [Description("ErrorInboxWatcherInboxService")]
            ErrorInboxWatcherInboxService = 3,
            [Description("LogFolderWatcherInboxService")]
            LogFolderWatcherInboxService = 4,
            [Description("FileExtWatcherInboxService")]
            FileExtWatcherInboxService = 5,
            [Description("DelimiterWatcherInboxService")]
            DelimiterWatcherInboxService = 6,
            [Description("ClientSettingsProviderServiceUriWatcherInboxService")]
            ClientSettingsProviderServiceUriWatcherInboxService = 7,
            [Description("TimeSleepWatcherInboxService")]
            TimeSleepWatcherInboxService = 8,
            [Description("DataFolder2WatcherInboxService")]
            DataFolder2WatcherInboxService = 9,
            [Description("USERNAMEWatcherInboxService")]
            USERNAMEWatcherInboxService = 10,
            [Description("PASSWORDWatcherInboxService")]
            PASSWORDWatcherInboxService = 11,
            [Description("SERVER_ADDRESSWatcherInboxService")]
            SERVER_ADDRESSWatcherInboxService = 12,
            [Description("waktujedaWatcherOutboxService")]
            waktujedaWatcherOutboxService = 13,
            [Description("DestinationFolderWatcherOutboxService")]
            DestinationFolderWatcherOutboxService = 14,
            [Description("LogFolderWatcherOutboxService")]
            LogFolderWatcherOutboxService = 15,
            [Description("FileExtWatcherOutboxService")]
            FileExtWatcherOutboxService = 16,
            [Description("DelimiterWatcherOutboxService")]
            DelimiterWatcherOutboxService = 17,
            [Description("ClientSettingsProviderServiceUriWatcherOutboxService")]
            ClientSettingsProviderServiceUriWatcherOutboxService = 18,
            [Description("TimeSleepWatcherOutboxService")]
            TimeSleepWatcherOutboxService = 19,
            [Description("MaxCopyWatcherOutboxService")]
            MaxCopyWatcherOutboxService = 20,
            [Description("DataFolderWatcherOutboxService")]
            DataFolderWatcherOutboxService = 21,
            [Description("USERNAMEWatcherOutboxService")]
            USERNAMEWatcherOutboxService = 22,
            [Description("PASSWORDWatcherOutboxService")]
            PASSWORDWatcherOutboxService = 23,
            [Description("SERVER_ADDRESSWatcherOutboxService")]
            SERVER_ADDRESSWatcherOutboxService = 24,
            [Description("TimeSchedulerWatcherService")]
            TimeSchedulerWatcherService = 25,
            [Description("DataFolderWatcherService")]
            DataFolderWatcherService = 26,
            [Description("ResultFolderWatcherService")]
            ResultFolderWatcherService = 27,
            [Description("LogFolderWatcherService")]
            LogFolderWatcherService = 28,
            [Description("FileExtWatcherService")]
            FileExtWatcherService = 29,
            [Description("DelimiterWatcherService")]
            DelimiterWatcherService = 30,
            [Description("ClientSettingsProviderServiceUriWatcherService")]
            ClientSettingsProviderServiceUriWatcherService = 31,
            [Description("TimeSleepWatcherService")]
            TimeSleepWatcherService = 32,
            [Description("TokenDuration")]
            TokenDuration = 33,
            [Description("MaxBadPasswordCount")]
            MaxBadPasswordCount = 34,
            [Description("MailHelperSMTPServer")]
            MailHelperSMTPServer = 35,
            [Description("MailHelperPort")]
            MailHelperPort = 36,
            [Description("MailHelperEnableSSL")]
            MailHelperEnableSSL = 37,
            [Description("MailHelperUseDefaultCredential")]
            MailHelperUseDefaultCredential = 38,
            [Description("MailHelperFromUser")]
            MailHelperFromUser = 39,
            [Description("MailHelperPasswd")]
            MailHelperPasswd = 40,
            [Description("MailHelperActivateEmail")]
            MailHelperActivateEmail = 41,
            [Description("DJPRequestTimeOutSetting")]
            DJPRequestTimeOutSetting = 42,
            [Description("CompareEvisVsIwsToleransiDiff")]
            CompareEvisVsIwsToleransiDiff = 43,
            [Description("CompareEvisVsSapToleransiDiff")]
            CompareEvisVsSapToleransiDiff = 44,
            [Description("CompanyCode")]
            CompanyCode = 45,
            [Description("EvisShareFolderRootPath")]
            EvisShareFolderRootPath = 46,
            [Description("EvisShareFolderDomain")]
            EvisShareFolderDomain = 47,
            [Description("EvisShareFolderUser")]
            EvisShareFolderUser = 48,
            [Description("EvisShareFolderPassword")]
            EvisShareFolderPassword = 49,
            [Description("NpwpAdm")]
            NpwpAdm = 50,
            [Description("NamaNpwpAdm")]
            NamaNpwpAdm = 51,
            [Description("GLAccountForceSubmitSAP")]
            GLAccountForceSubmitSAP = 52,
            [Description("DefaultPrintOrdner")]
            DefaultPrintOrdner = 53,
            [Description("PelaporanTglFaktur")]
            PelaporanTglFaktur = 54,
            [Description("EvisShareFolderIsSameDomain")]
            EvisShareFolderIsSameDomain = 55,
            [Description("FpKhususJenisTransaksiEmptyNpwp")]
            FpKhususJenisTransaksiEmptyNpwp = 56,
            [Description("BackupFolderWatcherInboxService")]
            BackupFolderWatcherInboxService = 57,
            [Description("MaxCopyWatcherInboxService")]
            MaxCopyWatcherInboxService = 58,
            [Description("DjpRequestErrorMailTo")]
            DjpRequestErrorMailTo = 59,
            [Description("DjpRequestErrorMailToDisplayName")]
            DjpRequestErrorMailToDisplayName = 60,
            [Description("InternetProxy")]
            InternetProxy = 61,
            [Description("InternetProxyPort")]
            InternetProxyPort = 62,
            [Description("UseDefaultCredential")]
            UseDefaultCredential = 63,
            [Description("DefaultAutoHideLeftMenu")]
            DefaultAutoHideLeftMenu = 64,
            [Description("ServiceRequestDetailFakturPajakBatchItem")]
            ServiceRequestDetailFakturPajakBatchItem = 65,
            [Description("ServiceRequestDetailFakturPajakTimeInterval")]
            ServiceRequestDetailFakturPajakTimeInterval = 66,
            [Description("ServiceRequestDetailFakturPajakStartAt")]
            ServiceRequestDetailFakturPajakStartAt = 67,
            [Description("ServiceRequestDetailFakturPajakProcessInterval")]
            ServiceRequestDetailFakturPajakProcessInterval = 68,
            [Description("ServiceRequestDetailFakturPajakDjpRequestInterval")]
            ServiceRequestDetailFakturPajakDjpRequestInterval = 69,
            [Description("FPDigantiOutstandingTglFaktur")]
            FPDigantiOutstandingTglFaktur = 70,
            [Description("MaxItemPerXmlCompareEvisVsSap")]
            MaxItemPerXmlCompareEvisVsSap = 71,
            [Description("MaxProcessFilesCompareEvisVsSap")]
            MaxProcessFilesCompareEvisVsSap = 72,
            [Description("EvisShareFolderTemporaryPath")]
            EvisShareFolderTemporaryPath = 73,
            [Description("EvisShareFolderRootPathLocalServer")]
            EvisShareFolderRootPathLocalServer = 74,
            [Description("EvisShareFolderTemporaryPathLocalServer")]
            EvisShareFolderTemporaryPathLocalServer = 75,
            [Description("MaxUploadSize")]
            MaxUploadSize = 76,
            [Description("AllowExtension")]
            AllowExtension = 77,
            [Description("EcmApiUrl")]
            EcmApiUrl = 78,
            [Description("EcmApiUsername")]
            EcmApiUsername = 79,
            [Description("EcmApiPassword")]
            EcmApiPassword = 80,
            [Description("EcmTempFolder")]
            EcmTempFolder = 81,
            [Description("SchedulerReposting")]
            SchedulerReposting = 82,
            [Description("StaticApiToken")]
            StaticApiToken = 86,
            [Description("MaxRetries")]
            MaxRetries = 88,
            [Description("WorkerCount")]
            WorkerCount = 89,
            [Description("MaxConcurrentThreads")]
            MaxConcurrentThreads = 90,
            [Description("DelviAPIPostFaktur")]
            DelviAPIPostFaktur = 91,
        }

        public enum FPType
        {
            [Description("IWS")]
            ScanIws = 1,
            [Description("Non IWS")]
            ScanNonIws = 2,
            [Description("Khusus")]
            ScanManual = 3,
            [Description("External")]
            External = 4
        }

        public enum ScanType
        {
            [Description("Satuan")]
            Satuan = 1,
            [Description("Bulk")]
            Bulk = 2,
            [Description("Manual")]
            Manual = 3,
            [Description("Pembetulan")]
            Pembetulan = 4
        }
        
        public enum FillingIndexType
        {
            [Description("Blank")]
            Blank = 0,
            [Description("Not Blank")]
            NotBlank = 1
        }

        public enum FpStatusDjp
        {
            [Description("Not Valid")]
            NotValid = 0,
            [Description("Valid")]
            Valid = 1
        }

        public enum FpStatusPayment
        {
            [Description("Blank")]
            Blank = 0,
            [Description("Certified")]
            Certified = 1,
            [Description("Recieved")]
            Recieved = 2,
            [Description("Ready For Payment")]
            ReadyForPayment = 3,
            [Description("Posted")]
            Posted = 4
        }

        public enum FpSource
        {
            [Description("Delvi")]
            Delvi = 0,
            [Description("Epay")]
            Epay = 1,
        }

        public enum CreatedCsvSource
        {
            [Description("Not yet created")]
            NotYetCreated = 0,
            [Description("Done")]
            Done = 1,
        }

        public enum LogPrintType
        {
            [Description("Ordner")]
            Ordner = 1
        }

        public enum SapStatusLog
        {
            [Description("Submitted")]
            Submitted = 0,
            [Description("Success")]
            Success = 1,
            [Description("Error")]
            Error = 2,
            [Description("Error Process Output XML")]
            ErrorProcessOutputXml = 3,
            [Description("Success Process Output XML")]
            SuccessProcessOutputXml = 4
        }

        public enum StatusCompareEvisVsIws
        {
            [Description("All")]
            All = 0,
            [Description("OK")]
            Ok = 1,
            [Description("OK With Notes")]
            OkWithNotes = 2,
            [Description("Not OK")]
            NotOk = 3
        }

        public enum StatusPostingEvisVsSAP
        {
            [Description("All")]
            All = 0,
            [Description("Belum Posting")]
            BelumPosting = 1,
            [Description("Sudah Posting")]
            SudahPosting = 2,
        }

        public enum StatusCompareEvisVsSap
        {
            [Description("All")]
            All = 0,
            [Description("ok")]
            Ok = 1,
            [Description("OK With Notes")]
            OkWithNotes = 2,
            [Description("not ok")]
            NotOk = 3
        }

        public enum VendorPkpDicabut
        {
            [Description("Ya")]
            Ya = 1,
            [Description("Tidak")]
            Tidak = 0
        }

        public enum VendorJenisNPWP
        {
            [Description("Badan Usaha Pusat")]
            BadanUsahaPusat = 0,
            [Description("Badan Usaha Pribadi")]
            BadanUsahaPribadi = 1,
            [Description("Badan Usaha Cabang")]
            BadanUsahaCabang = 2
        }

        public enum DownloadFolderType
        {
            [Description("Upload")]
            Upload = 1,
            [Description("Export")]
            Export = 2,
            [Description("SP2")]
            Sp2 = 3
        }

        public enum DownloadFileType
        {
            [Description("CSV")]
            Csv = 1,
            [Description("Excel")]
            Excel = 2,
            [Description("Vendor")]
            Vendor = 3,
            [Description("FpDigantiOutstanding")]
            FpDigantiOutstanding = 4
        }

        public enum StatusFakturPajak
        {
            [Description("Scanned")]
            Scanned = 1,
            [Description("Success")]
            Success = 2,
            [Description("Error Request")]
            ErrorRequest = 3,
            [Description("Error Validation")]
            ErrorValidation = 4
        }

        public enum StatusFakturPajakFromDjp
        {
            [Description("Faktur Pajak Normal")]
            FakturPajakNormal = 0,
            [Description("Faktur Diganti")]
            FakturDiganti = 1,
            [Description("Faktur Pajak Normal-Pengganti")]
            FakturPajakNormalPengganti = 3
        }

        public enum LogSapTransactionType
        {
            [Description("Reconcile with SAP")]
            ReconcileWithSap
        }

        public enum CompareSapDocumentStatus
        {
            [Description("")]
            BothNotExists = 0,
            [Description("")]
            BothExists = 1,
            [Description("Document in Problem")]
            DocumentInProblem = 2,
            [Description("Document Outstanding")]
            DocumentOutstanding = 3
        }


        public enum DownloadModuleType
        {
            [Description("Daftar Retur Faktur Pajak")]
            DaftarReturFakturPajak = 0,
            [Description("Daftar Faktur Pajak")]
            DaftarFakturPajak = 1,
            [Description("Compare Evis vs IWS")]
            CompareEvisVsIws = 2,
            [Description("Compare Evis vs SAP")]
            CompareEvisVsSap = 3,
            [Description("List Ordner")]
            ListOrdner = 4,
            [Description("Report SPM")]
            ReportSpm = 5,
            [Description("Report Detail Faktur Pajak")]
            ReportDetailFakturPajak = 6,
            [Description("Report Daftar Faktur Pajak Masukan")]
            ReportDaftarFakturPajakMasukan = 7,
            [Description("Report Faktur Pajak Outstanding")]
            ReportFakturPajakOutstanding = 8,
            [Description("Report Faktur Pajak Belum Di Jurnal")]
            ReportFakturPajakBelumDiJurnal = 9,
        }

        public enum SubmitType
        {
            [Description("Submit")]
            Submit,
            [Description("ForceSubmit")]
            ForceSubmit
        }

        public enum FCodeFpKhusus
        {
            [Description("DK")]
            Dk = 0,
            [Description("DM")]
            Dm = 1
        }

        public enum StatusReconcile
        {
            [Description("No Yet")]
            NoYet = 0,
            [Description("On Progress")]
            OnProgress = 1,
            [Description("Success")]
            Success = 2,
            [Description("Error")]
            Error = 3
        }

        public enum StatusDigantiOutstanding
        {
            [Description("Outstanding")]
            Outstanding = 1,
            [Description("Expired")]
            Expired = 2,
            [Description("Complete")]
            Complete = 3
        }

        public enum EnumGeneralCategory
        {
            [Description("FpDigantiOutstandingStatus")]
            FpDigantiOutstandingStatus = 1,
            [Description("FpDigantiOutstandingRemarks")]
            FpDigantiOutstandingRemarks = 2
        }

        public enum EnumSapStatusLogDetail
        {
            [Description("All")]
            All = 0,
            [Description("Sukses")]
            Sukses = 1,
            [Description("Gagal")]
            Gagal = 2,
            [Description("Submitted")]
            Submitted = 3
        }

        public enum EnumLogApiAction 
        {
            [Description("Reposting Date")]
            RepostingDate = 0,
            [Description("Send Faktur")]
            SendFaktur = 1,
            [Description("DJP Error")]
            DJPError = 2
        }

        public enum CheckingStatusValidasi
        {
            [Description("Pending")]
            Pending = 0,
            [Description("Success")]
            Success = 1,
            [Description("Failed")]
            Failed = 2,
        }
        public enum StatusValidasi
        {
            [Description("Verified")]
            Verification = 0,
            [Description("Pending Verification")]
            PendingVerification = 1,
            [Description("Pending Validation")]
            PendingValidation = 2,

        }

        public enum FpStatusOutstanding
        {
            [Description("Blank")]
            Blank = 1,
            [Description("Normal")]
            Normal = 2,
            [Description("Normal Pengganti")]
            NormalPengganti = 3,
            [Description("Pending Verification")]
            PendingVerification = 4
        }

        public enum ByPassOption
        {
            [Description("True")]
            True = 0,
            [Description("False")]
            False = 1,
        }
    }
}
