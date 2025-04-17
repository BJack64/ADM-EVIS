using System.Collections.Generic;
using System.Data.SqlTypes;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
namespace eFakturADM.Logic.Collections
{
    public class LogProcessSaps : ApplicationCollection<LogProcessSap, SpBase>
    {

        public static List<LogProcessSap> Get()
        {
            var sp = new SpBase(@"SELECT [LogProcessSapId]
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
      ,[XmlFileType]
FROM [dbo].[LogProcessSap]
WHERE [IsDeleted] = 0");
            return GetApplicationCollection(sp);
        }

        public static LogProcessSap GetLastOutputPpnCreditByIdNo(string idNo)
        {
            var sp = new SpBase(@"SELECT TOP 1 [LogProcessSapId]
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
      ,[XmlFileType]
FROM [dbo].[LogProcessSap]
WHERE [IsDeleted] = 0 AND [IdNo] = @IdNo AND XmlFileType = 'outUploadPPNCredit'
ORDER BY LogProcessSapId DESC");

            sp.AddParameter("IdNo", idNo);

            return GetApplicationObject(sp);

        }

        public static List<LogProcessSap> GetByIdNo(string idNo)
        {
            var sp = new SpBase(@"SELECT [LogProcessSapId]
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
      ,[XmlFileType]
FROM [dbo].[LogProcessSap]
WHERE [IsDeleted] = 0 AND [IdNo] = @IdNo");

            sp.AddParameter("IdNo", idNo);

            return GetApplicationCollection(sp);

        }

        public static List<LogProcessSap> GetByFileName(string fileName)
        {
            var sp = new SpBase(@"SELECT [LogProcessSapId]
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
      ,[XmlFileType]
FROM [dbo].[LogProcessSap]
WHERE [IsDeleted] = 0 AND [FilePath] = @FilePath");

            sp.AddParameter("FilePath", fileName);

            return GetApplicationCollection(sp);
        }

        public static LogProcessSap Save(LogProcessSap data)
        {
            //selalu insert, untuk log
            var sp =
                new SpBase(@"INSERT INTO [dbo].[LogProcessSap]([IdNo],[FileName],[FilePath],[Note],[Status],[CreatedBy],[XmlFileType])
VALUES(@IdNo,@FileName,@FilePath,@Note,@Status,@CreatedBy,@XmlFileType);SELECT @LogProcessSapId= @@IDENTITY");

            sp.AddParameter("LogProcessSapId", data.LogProcessSapId, System.Data.ParameterDirection.Output);
            sp.AddParameter("IdNo", string.IsNullOrEmpty(data.IdNo) ? SqlString.Null : data.IdNo);
            sp.AddParameter("FileName", data.FileName);
            sp.AddParameter("FilePath", data.FilePath);
            sp.AddParameter("Note", string.IsNullOrEmpty(data.Note) ? SqlString.Null : data.Note);
            sp.AddParameter("Status", data.Status);
            sp.AddParameter("CreatedBy", data.CreatedBy);
            sp.AddParameter("XmlFileType", data.XmlFileType);

            if (sp.ExecuteNonQuery() == 0)
                data.WasSaved = true;

            data.LogProcessSapId = (long)sp.GetParameter("LogProcessSapId");

            return data;

        }

    }
}
