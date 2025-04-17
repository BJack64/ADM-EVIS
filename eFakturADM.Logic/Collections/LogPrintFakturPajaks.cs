using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class LogPrintFakturPajaks : ApplicationCollection<LogPrintFakturPajak, SpBase>
    {

        public static List<LogPrintFakturPajak> Get()
        {
            var sp = new SpBase(@"SELECT CAST(ROW_NUMBER() OVER(ORDER BY [LogPrintId] ASC) AS int) AS VSequence
	  ,[LogPrintId]
      ,[FakturPajakId]
      ,[HeaderGuid]
      ,[PrintType]
      ,[PrintDate]
      ,[PrintBy]
      ,[Reason]
      ,[IsDeleted]
      ,[Created]
      ,[Modified]
      ,[CreatedBy]
      ,[ModifiedBy]
	  ,COUNT([LogPrintId]) OVER() AS TotalItems
  FROM [dbo].[LogPrintFakturPajak]
WHERE [IsDeleted] = 0");

            return GetApplicationCollection(sp);

        }

        public static List<LogPrintFakturPajak> GetByFakturPajakId(long fakturPajakId)
        {
            var sp = new SpBase(@"SELECT CAST(ROW_NUMBER() OVER(ORDER BY [LogPrintId] ASC) AS int) AS VSequence
	  ,[LogPrintId]
      ,[FakturPajakId]
      ,[HeaderGuid]
      ,[PrintType]
      ,[PrintDate]
      ,[PrintBy]
      ,[Reason]
      ,[IsDeleted]
      ,[Created]
      ,[Modified]
      ,[CreatedBy]
      ,[ModifiedBy]
	  ,COUNT([LogPrintId]) OVER() AS TotalItems
  FROM [dbo].[LogPrintFakturPajak]
WHERE [IsDeleted] = 0 AND [FakturPajakId] = @FakturPajakId");


            sp.AddParameter("FakturPajakId", fakturPajakId);

            return GetApplicationCollection(sp);

        }


        public static List<LogPrintFakturPajak> GetByFakturPajakIdViewReason(long fakturPajakId)
        {
            var iGenConfigId = (int) ApplicationEnums.GeneralConfig.DefaultPrintOrdner;
            var sp = new SpBase(@"SELECT	v.*
FROM	(
SELECT CAST(ROW_NUMBER() OVER(ORDER BY [LogPrintId] ASC) AS int) AS VSequence
	  ,[LogPrintId]
      ,[FakturPajakId]
      ,[HeaderGuid]
      ,[PrintType]
      ,[PrintDate]
      ,[PrintBy]
      ,[Reason]
      ,[IsDeleted]
      ,[Created]
      ,[Modified]
      ,[CreatedBy]
      ,[ModifiedBy]
	  ,COUNT([LogPrintId]) OVER() AS TotalItems
  FROM [dbo].[LogPrintFakturPajak]
WHERE [IsDeleted] = 0 AND [FakturPajakId] = @FakturPajakId 
) v
WHERE v.VSequence > (SELECT CAST(ConfigValue as int) AS defaultPrint FROM GeneralConfig WHERE GeneralConfigId = @configId)");
            
            sp.AddParameter("FakturPajakId", fakturPajakId);
            sp.AddParameter("configId", iGenConfigId);

            return GetApplicationCollection(sp);

        }


        public static LogPrintFakturPajak Insert(LogPrintFakturPajak data)
        {

            data.WasSaved = false;
            data.HeaderGuid = Guid.NewGuid().ToString();
            var sp = new SpBase(@"INSERT INTO [dbo].[LogPrintFakturPajak]([FakturPajakId],[PrintType],[PrintDate],[PrintBy],[Reason],[CreatedBy], [HeaderGuid])
VALUES(@FakturPajakId,@PrintType,@PrintDate,@PrintBy,@Reason,@CreatedBy, @HeaderGuid);  SELECT @LogPrintId = @@IDENTITY");

            sp.AddParameter("LogPrintId", data.LogPrintId, ParameterDirection.Output);
            sp.AddParameter("FakturPajakId", data.FakturPajakId);
            sp.AddParameter("PrintType", data.PrintType);
            sp.AddParameter("PrintBy", data.PrintBy);
            sp.AddParameter("Reason", data.Reason);
            sp.AddParameter("PrintDate", DateTime.Now);
            sp.AddParameter("CreatedBy", data.CreatedBy);
            sp.AddParameter("HeaderGuid", data.HeaderGuid);

            if (sp.ExecuteNonQuery() == 0)
                data.WasSaved = true;

            data.LogPrintId = (long)sp.GetParameter("LogPrintId");

            return data;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fakturPajakIds">Multiple Faktur Pajak Id, separated by comma</param>
        /// <param name="printType"></param>
        /// <param name="printBy"></param>
        /// <param name="reason"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public static bool BulkInsert(string fakturPajakIds, string printType, string printBy, string reason,
            string createdBy)
        {
            var headerGuid = Guid.NewGuid().ToString();
            var sp = new SpBase(@"INSERT INTO [dbo].[LogPrintFakturPajak]
           ([FakturPajakId]
           ,[PrintType]
           ,[PrintDate]
           ,[PrintBy]
           ,[Reason]
           ,[CreatedBy]
           ,[HeaderGuid])
SELECT	CAST(RTRIM(LTRIM(Data)) AS int) AS FakturPajakId,
		@printType AS PrintType,
		GETDATE() AS PrintDate,
		@printBy AS PrintBy,
		@reason AS Reason,
		@createdBy AS CreatedBy,
        @headerGuid AS HeaderGuid
FROM	dbo.Split(@fakturPajakIds)");

            sp.AddParameter("printType", printType);
            sp.AddParameter("printBy", printBy);
            sp.AddParameter("reason", string.IsNullOrEmpty(reason) ? SqlString.Null : reason);
            sp.AddParameter("createdBy", createdBy);
            sp.AddParameter("fakturPajakIds", fakturPajakIds);
            sp.AddParameter("headerGuid", headerGuid);

            return sp.ExecuteNonQuery() >= 0;

        }

        public static bool Delete(long logPrintId, string userModify)
        {
            var sp = new SpBase(@"UPDATE [dbo].[LogPrintFakturPajak]
   SET [IsDeleted] = 1
      ,[Modified] = GETDATE()
      ,[ModifiedBy] = @ModifiedBy
 WHERE [LogPrintId] = @LogPrintId");

            sp.AddParameter("ModifiedBy", userModify);
            sp.AddParameter("LogPrintId", logPrintId);

            return sp.ExecuteNonQuery() >= 0;

        }

    }
}
