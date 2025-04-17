using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class LogDownloads
    {
        public static void Insert(LogDownload dats)
        {
            var sp =
                new SpBase(@"INSERT INTO [dbo].[LogDownload]([Requestor],[FileName],[FilePath],[FileType],[ClientIp])
VALUES(@Requestor,@FileName,@FilePath,@FileType,@ClientIp)");

            sp.AddParameter("Requestor", dats.Requestor);
            sp.AddParameter("FileName", dats.FileName);
            sp.AddParameter("FilePath", dats.FilePath);
            sp.AddParameter("FileType", dats.FileType);
            sp.AddParameter("ClientIp", dats.ClientIp);

            sp.ExecuteNonQuery();

        }
    }
}
