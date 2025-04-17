using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Collections
{
    public class LogSaps : ApplicationCollection<LogSap, SpBase>
    {

        public static List<LogSap> GetList(Filter filter, out int totalItems, int? status, DateTime? submittedDate)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;
            var sp = new SpBase(string.Format(@"SELECT v.*,CAST(ROW_NUMBER() OVER(ORDER BY {2} {1}) AS int) AS VSequenceNumber
                                                FROM (
                                                SELECT     [LogSapId]
		                                                  ,[TransactionType]
		                                                  ,lsap.[IdNo]
		                                                  ,lsap.[FileName]
		                                                  ,[LocalExecution]
		                                                  ,[LocalPath]
		                                                  ,[TransferDate]
		                                                  ,[SapPath]
		                                                  ,lsap.[Status]
		                                                  ,lsap.[Note]
		                                                  ,lsap.[IsDeleted]
		                                                  ,lsap.[Created]
		                                                  ,lsap.[Modified]
		                                                  ,lsap.[CreatedBy]
		                                                  ,lsap.[ModifiedBy]
		                                                  ,lprocsap.[FileName] AS ReceivedFileName
		                                                  ,lprocsap.[FilePath] AS ReceivedFilePath
		                                                  ,lprocsap.[Status] AS ReceivedStatus
		                                                  ,lprocsap.[Note] AS ReceivedNotes
		                                                  ,COUNT(lsap.LogSapId) OVER() AS TotalItems
                                                          ,lsap.AccountingDocNoCredit
                                                          ,lsap.FiscalYearCredit
	                                                FROM [dbo].[LogSap] lsap LEFT JOIN
		                                                (
			                                                SELECT [LogProcessSapId]
				                                                  ,[IdNo]
				                                                  ,[FileName]
				                                                  ,[FilePath]
				                                                  ,[Note]
				                                                  ,[Status]
				                                                  ,[IsDeleted]
				                                                  ,[Created]
				                                                  ,[Modified]
				                                                  ,[CreatedBy]
				                                                  ,[ModifiedBy]
				                                                  ,ROW_NUMBER() OVER(PARTITION BY [IdNo] ORDER BY [Created] DESC) AS VPartition
			                                                FROM [dbo].[LogProcessSap]
			                                                WHERE [IsDeleted] = 0
		                                                )lprocsap ON lprocsap.VPartition = 1 AND lprocsap.IdNo = lsap.IdNo
	                                                WHERE lsap.[IsDeleted] = 0 AND ((@StatusId IS NOT NULL AND lsap.[Status] = @StatusId) OR @StatusId IS NULL)
			                                                AND ((@SubmittedDate IS NOT NULL AND CAST(@SubmittedDate AS date) = CAST(lsap.[Created] AS date)) OR @SubmittedDate IS NULL)
			                                                AND (
				                                                @Search IS NULL
				                                                OR LOWER(lsap.FileName) LIKE REPLACE(LOWER(@Search), '*', '%')
				                                                OR LOWER(lsap.Note) LIKE REPLACE(LOWER(@Search), '*', '%')
				                                                OR LOWER(CONVERT(varchar, lsap.Created, 103)) LIKE REPLACE(LOWER(@Search), '*', '%')
				                                                OR (lsap.LocalExecution IS NULL OR (lsap.LocalExecution IS NOT NULL AND LOWER(CONVERT(varchar, lsap.LocalExecution, 103)) LIKE REPLACE(LOWER(@Search), '*', '%')))
			                                                )
                                                ) v
	                                                ORDER BY {0} {1}
	                                                OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
	                                                FETCH NEXT @ItemPerPage ROWS ONLY", filter.SortColumnName, sortOrder, (filter.SortColumnName == "Created" ? "LogSapId" : filter.SortColumnName)));
            
            sp.AddParameter("CurrentPage", filter.CurrentPage);
            sp.AddParameter("ItemPerPage", filter.ItemsPerPage);
            sp.AddParameter("Search", string.IsNullOrEmpty(filter.Search) ? SqlString.Null : filter.Search);
            sp.AddParameter("StatusId", status.HasValue ? status.Value : SqlInt32.Null);
            sp.AddParameter("SubmittedDate", submittedDate.HasValue ? submittedDate.Value : SqlDateTime.Null);

            var data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetList(filter, out totalItems, status, submittedDate);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetList(filter, out totalItems, status, submittedDate);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetList(filter, out totalItems, status, submittedDate);
            }

            return data;

        }

        public static LogSap GetByIdNo(string idNo)
        {
            var sp = new SpBase(@"SELECT 
		  CAST(ROW_NUMBER() OVER(ORDER BY lsap.LogSapId ASC) AS int) AS VSequenceNumber
		  ,[LogSapId]
		  ,[TransactionType]
		  ,lsap.[IdNo]
		  ,lsap.[FileName]
		  ,[LocalExecution]
		  ,[LocalPath]
		  ,[TransferDate]
		  ,[SapPath]
		  ,lsap.[Status]
		  ,lsap.[Note]
		  ,lsap.[IsDeleted]
		  ,lsap.[Created]
		  ,lsap.[Modified]
		  ,lsap.[CreatedBy]
		  ,lsap.[ModifiedBy]
		  ,lprocsap.[FileName] AS ReceivedFileName
		  ,lprocsap.[FilePath] AS ReceivedFilePath
		  ,lprocsap.[Status] AS ReceivedStatus
		  ,lprocsap.[Note] AS ReceivedNotes
		  ,COUNT(lsap.LogSapId) OVER() AS TotalItems
          ,lsap.AccountingDocNoCredit
          ,lsap.FiscalYearCredit
	FROM [dbo].[LogSap] lsap LEFT JOIN
		(
			SELECT [LogProcessSapId]
				  ,[IdNo]
				  ,[FileName]
				  ,[FilePath]
				  ,[Note]
				  ,[Status]
				  ,[IsDeleted]
				  ,[Created]
				  ,[Modified]
				  ,[CreatedBy]
				  ,[ModifiedBy]
				  ,ROW_NUMBER() OVER(PARTITION BY [IdNo] ORDER BY [Created] DESC) AS VPartition
			FROM [dbo].[LogProcessSap]
			WHERE [IsDeleted] = 0
		)lprocsap ON lprocsap.VPartition = 1 AND lprocsap.IdNo = lsap.IdNo
	WHERE lsap.[IsDeleted] = 0 AND lsap.[IdNo] = @IdNo");
            
            sp.AddParameter("IdNo", idNo);

            var dbData = GetApplicationObject(sp);

            if (dbData == null || dbData.LogSapId == 0) return null;
            return dbData;

        }

        public static LogSap GetByFileName(string fileName)
        {
            var sp = new SpBase(@"SELECT 
		  CAST(ROW_NUMBER() OVER(ORDER BY lsap.LogSapId ASC) AS int) AS VSequenceNumber
		  ,[LogSapId]
		  ,[TransactionType]
		  ,lsap.[IdNo]
		  ,lsap.[FileName]
		  ,[LocalExecution]
		  ,[LocalPath]
		  ,[TransferDate]
		  ,[SapPath]
		  ,lsap.[Status]
		  ,lsap.[Note]
		  ,lsap.[IsDeleted]
		  ,lsap.[Created]
		  ,lsap.[Modified]
		  ,lsap.[CreatedBy]
		  ,lsap.[ModifiedBy]
		  ,lprocsap.[FileName] AS ReceivedFileName
		  ,lprocsap.[FilePath] AS ReceivedFilePath
		  ,lprocsap.[Status] AS ReceivedStatus
		  ,lprocsap.[Note] AS ReceivedNotes
		  ,COUNT(lsap.LogSapId) OVER() AS TotalItems
          ,lsap.AccountingDocNoCredit
          ,lsap.FiscalYearCredit
	FROM [dbo].[LogSap] lsap LEFT JOIN
		(
			SELECT [LogProcessSapId]
				  ,[IdNo]
				  ,[FileName]
				  ,[FilePath]
				  ,[Note]
				  ,[Status]
				  ,[IsDeleted]
				  ,[Created]
				  ,[Modified]
				  ,[CreatedBy]
				  ,[ModifiedBy]
				  ,ROW_NUMBER() OVER(PARTITION BY [IdNo] ORDER BY [Created] DESC) AS VPartition
			FROM [dbo].[LogProcessSap]
			WHERE [IsDeleted] = 0
		)lprocsap ON lprocsap.VPartition = 1 AND lprocsap.IdNo = lsap.IdNo
	WHERE lsap.[IsDeleted] = 0 AND lsap.[FileName] = @FileName");

            sp.AddParameter("FileName", fileName);

            var dbData = GetApplicationObject(sp);

            if (dbData == null || dbData.LogSapId == 0) return null;
            return dbData;
        }

        public static LogSap GetById(long id)
        {
            var sp = new SpBase(@"SELECT 
		  CAST(ROW_NUMBER() OVER(ORDER BY lsap.LogSapId ASC) AS int) AS VSequenceNumber
		  ,[LogSapId]
		  ,[TransactionType]
		  ,lsap.[IdNo]
		  ,lsap.[FileName]
		  ,[LocalExecution]
		  ,[LocalPath]
		  ,[TransferDate]
		  ,[SapPath]
		  ,lsap.[Status]
		  ,lsap.[Note]
		  ,lsap.[IsDeleted]
		  ,lsap.[Created]
		  ,lsap.[Modified]
		  ,lsap.[CreatedBy]
		  ,lsap.[ModifiedBy]
		  ,lprocsap.[FileName] AS ReceivedFileName
		  ,lprocsap.[FilePath] AS ReceivedFilePath
		  ,lprocsap.[Status] AS ReceivedStatus
		  ,lprocsap.[Note] AS ReceivedNotes
		  ,COUNT(lsap.LogSapId) OVER() AS TotalItems
          ,lsap.AccountingDocNoCredit
          ,lsap.FiscalYearCredit
	FROM [dbo].[LogSap] lsap LEFT JOIN
		(
			SELECT [LogProcessSapId]
				  ,[IdNo]
				  ,[FileName]
				  ,[FilePath]
				  ,[Note]
				  ,[Status]
				  ,[IsDeleted]
				  ,[Created]
				  ,[Modified]
				  ,[CreatedBy]
				  ,[ModifiedBy]
				  ,ROW_NUMBER() OVER(PARTITION BY [IdNo] ORDER BY [Created] DESC) AS VPartition
			FROM [dbo].[LogProcessSap]
			WHERE [IsDeleted] = 0
		)lprocsap ON lprocsap.VPartition = 1 AND lprocsap.IdNo = lsap.IdNo
	WHERE lsap.[IsDeleted] = 0 AND lsap.[LogSapId] = @LogSapId");

            sp.AddParameter("LogSapId", id);

            var dbData = GetApplicationObject(sp);

            if (dbData == null || dbData.LogSapId == 0) return null;
            return dbData;

        }

        public static List<LogSap> GetByIds(string ids)
        {
            var sp = new SpBase(@"SELECT 
		  CAST(ROW_NUMBER() OVER(ORDER BY lsap.LogSapId ASC) AS int) AS VSequenceNumber
		  ,[LogSapId]
		  ,[TransactionType]
		  ,lsap.[IdNo]
		  ,lsap.[FileName]
		  ,[LocalExecution]
		  ,[LocalPath]
		  ,[TransferDate]
		  ,[SapPath]
		  ,lsap.[Status]
		  ,lsap.[Note]
		  ,lsap.[IsDeleted]
		  ,lsap.[Created]
		  ,lsap.[Modified]
		  ,lsap.[CreatedBy]
		  ,lsap.[ModifiedBy]
		  ,lprocsap.[FileName] AS ReceivedFileName
		  ,lprocsap.[FilePath] AS ReceivedFilePath
		  ,lprocsap.[Status] AS ReceivedStatus
		  ,lprocsap.[Note] AS ReceivedNotes
		  ,COUNT(lsap.LogSapId) OVER() AS TotalItems
          ,lsap.AccountingDocNoCredit
          ,lsap.FiscalYearCredit
	FROM [dbo].[LogSap] lsap LEFT JOIN
		(
			SELECT [LogProcessSapId]
				  ,[IdNo]
				  ,[FileName]
				  ,[FilePath]
				  ,[Note]
				  ,[Status]
				  ,[IsDeleted]
				  ,[Created]
				  ,[Modified]
				  ,[CreatedBy]
				  ,[ModifiedBy]
				  ,ROW_NUMBER() OVER(PARTITION BY [IdNo] ORDER BY [Created] DESC) AS VPartition
			FROM [dbo].[LogProcessSap]
			WHERE [IsDeleted] = 0
		)lprocsap ON lprocsap.VPartition = 1 AND lprocsap.IdNo = lsap.IdNo
	WHERE lsap.[IsDeleted] = 0 AND lsap.[LogSapId] IN (SELECT Data FROM dbo.Split(@ids))");

            sp.AddParameter("ids", ids);

            return GetApplicationCollection(sp);

        }

        public static LogSap Save(LogSap data)
        {
            SpBase sp;
            if (data.LogSapId <= 0)
            {
                //Insert
                sp = new SpBase(@"INSERT INTO [dbo].[LogSap]([TransactionType],[IdNo],[FileName],[LocalExecution],[LocalPath],[TransferDate],[SapPath],[Status],[Note],[CreatedBy])
VALUES(@TransactionType,@IdNo,@FileName,@LocalExecution,@LocalPath,@TransferDate,@SapPath,@Status,@Note,@CreatedBy); SELECT @LogSapId = @@IDENTITY;");
                sp.AddParameter("LogSapId", data.LogSapId, System.Data.ParameterDirection.Output);
                sp.AddParameter("CreatedBy", data.CreatedBy);
            }
            else
            {
                //Update
                sp = new SpBase(@"UPDATE [dbo].[LogSap]
   SET [TransactionType] = @TransactionType
      ,[IdNo] = @IdNo
      ,[FileName] = @FileName
      ,[LocalExecution] = CASE WHEN @LocalExecution IS NULL THEN NULL ELSE GETDATE() END
      ,[LocalPath] = @LocalPath
      ,[TransferDate] = @TransferDate
      ,[SapPath] = @SapPath
      ,[Status] = @Status
      ,[Note] = @Note
      ,[Modified] = GETDATE()
      ,[ModifiedBy] = @ModifiedBy
 WHERE [LogSapId] = @LogSapId");
                sp.AddParameter("LogSapId", data.LogSapId);
                sp.AddParameter("ModifiedBy", data.ModifiedBy);
                
            }
            
            sp.AddParameter("TransactionType", data.TransactionType);
            sp.AddParameter("IdNo", data.IdNo);
            sp.AddParameter("FileName", data.FileName);
            sp.AddParameter("LocalExecution", data.LocalExecution.HasValue ? data.LocalExecution.Value : SqlDateTime.Null);
            sp.AddParameter("LocalPath", data.LocalPath);
            sp.AddParameter("TransferDate", data.TransferDate.HasValue ? data.TransferDate.Value : SqlDateTime.Null);
            sp.AddParameter("SapPath", string.IsNullOrEmpty(data.SapPath) ? SqlString.Null : data.SapPath);
            sp.AddParameter("Status", data.Status);
            sp.AddParameter("Note", string.IsNullOrEmpty(data.Note) ? SqlString.Null : data.Note);

            if (sp.ExecuteNonQuery() == 0)
                data.WasSaved = true;

            if (data.LogSapId <= 0)
            {
                data.LogSapId = (long)sp.GetParameter("LogSapId");
            }
            
            return data;
        }
        
        public static bool UpdateStatus(long logSapId, int newStatus, DateTime? localExecution, string modifiedBy)
        {
            var sp = new SpBase(@"UPDATE LogSap
SET [Status] = @Status
	, [Modified] = GETDATE()
	, [ModifiedBy] = @ModifiedBy
	, [LocalExecution] = @LocalExecution
WHERE LogSapId = @LogSapId");

            sp.AddParameter("Status", newStatus);
            sp.AddParameter("ModifiedBy", modifiedBy);
            sp.AddParameter("LocalExecution", localExecution.HasValue ? localExecution.Value : SqlDateTime.Null);
            sp.AddParameter("LogSapId", logSapId);

            return sp.ExecuteNonQuery() >= 0;
        }

        public static bool UpdateFromSap(string idNo, int status, string note, string accountingDocNoCredit, int? fiscalYearCredit, string modifiedBy)
        {
            var sp = new SpBase(@"UPDATE [dbo].[LogSap]
   SET [Status] = @Status
      ,[Note] = @Note
      ,[Modified] = GETDATE()
      ,[ModifiedBy] = @ModifiedBy
      ,[AccountingDocNoCredit] = @AccountingDocNoCredit
      ,[FiscalYearCredit] = @FiscalYearCredit
 WHERE [IdNo] = @IdNo");

            sp.AddParameter("Status", status);
            sp.AddParameter("Note", string.IsNullOrEmpty(note) ? SqlString.Null : note);
            sp.AddParameter("ModifiedBy", modifiedBy);
            sp.AddParameter("AccountingDocNoCredit", string.IsNullOrEmpty(accountingDocNoCredit) ?  SqlString.Null : accountingDocNoCredit);
            sp.AddParameter("FiscalYearCredit", fiscalYearCredit.HasValue ? fiscalYearCredit.Value : SqlInt32.Null);
            sp.AddParameter("IdNo", idNo);

            return sp.ExecuteNonQuery() >= 0;

        }

    }
}
