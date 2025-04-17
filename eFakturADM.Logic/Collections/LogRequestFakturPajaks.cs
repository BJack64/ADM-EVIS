using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class LogRequestFakturPajaks : ApplicationCollection<LogRequestFakturPajak, SpBase>
    {

        public static LogRequestFakturPajak GetSpecificFakturStatusLog(long fakturPajakId, int status, string errorMessage, string createdBy)
        {
            var sp = new SpBase(@"SELECT * 
FROM	LogRequestFakturPajak
WHERE	FakturPajakId = @FakturPajakId AND IsDeleted = 0 
		AND Status = @status AND ErrorMessage = @ErrorMessage 
		AND CreatedBy = @createdBy");

            sp.AddParameter("FakturPajakId", fakturPajakId);
            sp.AddParameter("status", status);
            sp.AddParameter("ErrorMessage", errorMessage);
            sp.AddParameter("createdBy", createdBy);

            var dbData = GetApplicationObject(sp);
            return dbData == null || dbData.LogRequestFakturPajakId == 0 ? null : dbData;

        }

        public static void Save(LogRequestFakturPajak dat)
        {
            SpBase sp;
            if (dat.LogRequestFakturPajakId > 0)
            {
                //Update
                sp = new SpBase(@"UPDATE [dbo].[LogRequestFakturPajak]
   SET [RequestDate] = GETDATE()
      ,[Modified] = GETDATE()
	  ,[Created] = GETDATE()
      ,[ModifiedBy] = @ModifiedBy
WHERE	[LogRequestFakturPajakId] = @LogRequestFakturPajakId");

                sp.AddParameter("LogRequestFakturPajakId", dat.LogRequestFakturPajakId);
                sp.AddParameter("ModifiedBy", dat.ModifiedBy);
            }
            else
            {
                //Insert
                 sp =
                 new SpBase(
                     @"INSERT INTO [dbo].[LogRequestFakturPajak]([RequestUrl],[FakturPajakId],[Status],[ErrorMessage],[CreatedBy])
VALUES(@RequestUrl,@FakturPajakId,@Status,@ErrorMessage,@CreatedBy)");
                 sp.AddParameter("ErrorMessage", dat.ErrorMessage);
                 sp.AddParameter("CreatedBy", dat.CreatedBy);

                 sp.AddParameter("RequestUrl", dat.RequestUrl);
                 sp.AddParameter("FakturPajakId", dat.FakturPajakId);
                 sp.AddParameter("Status", dat.Status);
            }

            sp.ExecuteNonQuery();
        }
        
    }
}
