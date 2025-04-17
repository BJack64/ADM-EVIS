using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;
using eFakturADM.Shared.Utility;

namespace eFakturADM.Logic.Objects
{
    public partial class LogSap
    {
        public int TotalItems { get; set; }
    }
    public partial class LogSap
    {
        public int VSequenceNumber { get; set; }
        public long LogSapId { get; set; }
        public string TransactionType { get; set; }
        public string IdNo { get; set; }
        public string FileName { get; set; }
        public DateTime? LocalExecution { get; set; }
        public string LocalPath { get; set; }
        public DateTime? TransferDate { get; set; }
        public string SapPath { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string LocalExecutionString { get; set; }
        public string TransferDateString { get; set; }
        public string StatusString { get; set; }
        public string CreatedString { get; set; }
        public string ModifiedString { get; set; }

        public string ReceivedFileName { get; set; }
        public string ReceivedFilePath { get; set; }
        public int ReceivedStatus { get; set; }
        public string ReceivedNotes { get; set; }
        public string ReceivedStatusString { get; set; }
        public string AccountingDocNoCredit { get; set; }
        public int? FiscalYearCredit { get; set; }
    }

    public partial class LogSap : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;
            VSequenceNumber = DBUtil.GetIntField(dr, "VSequenceNumber");
            LogSapId = DBUtil.GetLongField(dr, "LogSapId");
            TransactionType = DBUtil.GetCharField(dr, "TransactionType");
            IdNo = DBUtil.GetCharField(dr, "IdNo");
            AccountingDocNoCredit = DBUtil.GetCharField(dr, "AccountingDocNoCredit");
            FiscalYearCredit = DBUtil.GetIntNullField(dr, "FiscalYearCredit");

            FileName = DBUtil.GetCharField(dr, "FileName");
            LocalExecution = DBUtil.GetDateTimeNullField(dr, "LocalExecution");
            LocalPath = DBUtil.GetCharField(dr, "LocalPath");
            TransferDate = DBUtil.GetDateTimeNullField(dr, "TransferDate");
            SapPath = DBUtil.GetCharField(dr, "SapPath");
            Status = DBUtil.GetIntField(dr, "Status");
            Note = DBUtil.GetCharField(dr, "Note");

            Created = DBUtil.GetDateTimeField(dr, "Created");
            Modified = DBUtil.GetDateTimeNullField(dr, "Modified");
            CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            ModifiedBy = DBUtil.GetCharField(dr, "ModifiedBy");
            IsDeleted = DBUtil.GetBoolField(dr, "IsDeleted");

            LocalExecutionString = ConvertHelper.DateTimeConverter.ToLongDateString(LocalExecution);
            TransferDateString = ConvertHelper.DateTimeConverter.ToLongDateString(TransferDate);
            StatusString = EnumHelper.GetDescription((ApplicationEnums.SapStatusLog) Status);
            CreatedString = ConvertHelper.DateTimeConverter.ToLongDateString(Created);
            ModifiedString = ConvertHelper.DateTimeConverter.ToLongDateString(Modified);

            ReceivedFileName = DBUtil.GetCharField(dr, "ReceivedFileName");
            ReceivedFilePath = DBUtil.GetCharField(dr, "ReceivedFilePath");
            ReceivedStatus = DBUtil.GetIntField(dr, "ReceivedStatus");
            ReceivedNotes = DBUtil.GetCharField(dr, "ReceivedNotes");

            ReceivedStatusString = EnumHelper.GetDescription((ApplicationEnums.SapStatusLog) ReceivedStatus);
            
            TotalItems = DBUtil.GetIntField(dr, "TotalItems");
            
            IsValid = true;
            return IsValid;
        }
    }
}
