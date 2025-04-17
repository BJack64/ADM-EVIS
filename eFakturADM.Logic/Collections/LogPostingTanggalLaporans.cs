using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace eFakturADM.Logic.Collections
{
    public class LogPostingTanggalLaporans : ApplicationCollection<LogPostingTanggalLaporan, SpBase>
    {
        public static LogPostingTanggalLaporan Save(LogPostingTanggalLaporan data)
        {
            //selalu insert, untuk log
            //var sp =
            //    new SpBase(@"INSERT INTO [dbo].[LogPostingTanggalLaporan]([Source],[Url],[Method],[Payload],[Status],[CreatedBy])
            //    VALUES(@Source,@Url,@Method,@Payload,@Status,@CreatedBy);");


            var sp =
                new SpBase(@"
                    EXEC [dbo].[sp_LogApi_Save]
	                    @ID
                        ,@Source
	                    ,@Url
	                    ,@Payload
	                    ,@FPdjpID
                        ,@UserName
                        ,@Action
                        ,@Message
                ");

            sp.AddParameter("Source", string.IsNullOrEmpty(data.Source) ? SqlString.Null : data.Source);
            sp.AddParameter("Url", string.IsNullOrEmpty(data.Url) ? SqlString.Null : data.Url);
            sp.AddParameter("Payload", string.IsNullOrEmpty(data.Payload) ? SqlString.Null : data.Payload);
            sp.AddParameter("UserName", string.IsNullOrEmpty(data.CreatedBy) ? SqlString.Null : data.CreatedBy);
            sp.AddParameter("FPdjpID", string.IsNullOrEmpty(data.FPdjpID) ? SqlString.Null : data.FPdjpID);
            sp.AddParameter("Action", string.IsNullOrEmpty(data.Action) ? SqlString.Null : data.Action);
            sp.AddParameter("Message", string.IsNullOrEmpty(data.Message) ? SqlString.Null : data.Message);
            sp.AddParameter("ID", data.Id);

            if (sp.ExecuteNonQuery() == 0)
                data.WasSaved = true;

            return data;

        }

        public static List<LogPostingTanggalLaporan> GetPendingData(string action, bool useMaxHitapi = true)
        {
            SpBase sp;

            if (useMaxHitapi)
            {
                sp = new SpBase(@"
                    SELECT [ID]
                          ,[Source]
                          ,[Url]
                          ,[Method]
                          ,[Payload]
                          ,[Status]
                          ,[Created]
                          ,[CreatedBy]
                          ,[FPdjpID]
                          ,[Action]
                          ,[Message]  
                    FROM [dbo].[LogPostingTanggalLaporan] WHERE 
                    (
                    (IsDeleted = 0 AND Status = 0)
                    OR
                    (
                        IsDeleted = 0 
                        AND Status = 2
                        AND 
                        ([Count]  IS NULL 
                        OR ([Count] IS NOT NULL AND [Count] <= CAST((SELECT TOP 1 ConfigValue FROM GeneralConfig WHERE ConfigKey = 'MaxHitApi') AS INT))
                        )
                    )
                    ) 
                    AND Action = @Action AND (IsByPass IS NULL OR (IsByPass IS NOT NULL AND IsByPass = 0))
                    ORDER BY Created");
            }
            else
            {
                sp = new SpBase(@"
                    SELECT [ID]
                          ,[Source]
                          ,[Url]
                          ,[Method]
                          ,[Payload]
                          ,[Status]
                          ,[Created]
                          ,[CreatedBy]
                          ,[FPdjpID]
                          ,[Action]
                          ,[Message]  
                    FROM [dbo].[LogPostingTanggalLaporan] WHERE 
                        IsDeleted = 0 AND Status != 1 AND Action = @Action
                    ORDER BY Created");
            }

            sp.AddParameter("@Action", action);
            var dbData = GetApplicationCollection(sp);
            return dbData;
        }


        public static bool SetStatus(long ID, bool isSuccess, string Message = "")
        {
            bool result = false;
            var sp =
                new SpBase(@"
                    UPDATE [dbo].[LogPostingTanggalLaporan]
                       SET [Status] = @StatusParam
                          ,[Modified] = GETDATE()
                          ,[ModifiedBy] = 'System'
                          ,[Count] = ISNULL([Count],0) + 1
                          ,[Message] = CASE WHEN @Message = '' THEN Message ELSE @Message END
                     WHERE ID = @IDParam
                ");

            sp.AddParameter("IDParam", ID);
            sp.AddParameter("StatusParam", isSuccess ? 1 : 2);
            sp.AddParameter("Message", isSuccess ? "-" : Message );

            if (sp.ExecuteNonQuery() == 0)
                result = true;

            return result;

        }


        public static bool SetByPassLogPosting(string FPdjpIDs, bool value, string By = "System")
        {
            bool result = false;
            var sp =
                new SpBase(@"
                    UPDATE [dbo].[LogPostingTanggalLaporan]
                       SET [IsByPass] = @StatusParam
                          ,[Modified] = GETDATE()
                          ,[ModifiedBy] = @By
                          ,[ByPassOn] = GETDATE()
                          ,[ByPassBy] = @By
                     WHERE FPdjpID  IN (SELECT Data FROM dbo.Split(@IDParam)) AND Action != 'Send Faktur';
                ");

            sp.AddParameter("IDParam", FPdjpIDs);
            sp.AddParameter("StatusParam", value);
            sp.AddParameter("By", By);

            if (sp.ExecuteNonQuery() == 0)
                result = true;

            return result;

        }



        public static bool SetByPassOnFakturPajak(string FPdjpIDs, bool value, string By = "System")
        {
            bool result = false;
            var sp =
                new SpBase(@"
                    UPDATE fp
	                    SET StatusFaktur = 'Pending Verification'
	                    ,Modified = GETDATE()
                        ,[ModifiedBy] = @By
                        ,[IsByPass] = @StatusParam
                    FROM
                    [dbo].[LogPostingTanggalLaporan] lp
	                    INNER JOIN FakturPajak fp ON fp.IsDeleted = 0 AND fp.UrlScan = lp.Url
                    WHERE FPdjpID  IN (SELECT Data FROM dbo.Split(@IDParam)) AND lp.Action != 'Send Faktur'
                ");

            sp.AddParameter("IDParam", FPdjpIDs);
            sp.AddParameter("StatusParam", value);
            sp.AddParameter("By", By);

            if (sp.ExecuteNonQuery() == 0)
                result = true;

            return result;

        }

    }
}
