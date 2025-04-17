using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class FPJenisTransaksis : ApplicationCollection<FPJenisTransaksi, SpBase>
    {
        public static List<FPJenisTransaksi> Get()
        {
            var sp = new SpBase(@"SELECT [Id]
      ,[FCode]
      ,[Description]
FROM [dbo].[FPJenisTransaksi]");
            return GetApplicationCollection(sp);
        }

        public static FPJenisTransaksi GetById(string id)
        {
            var sp = new SpBase(@"SELECT [Id]
      ,[FCode]
      ,[Description]
FROM [dbo].[FPJenisTransaksi]
WHERE [Id] = @Id");
            sp.AddParameter("Id", id);
            var dbData = GetApplicationObject(sp);
            return dbData == null || string.IsNullOrEmpty(dbData.Id) ? null : dbData;
        }

        public static List<FPJenisTransaksi> GetByFCode(string fCode)
        {
            var sp = new SpBase(@"SELECT [Id]
      ,[FCode]
      ,[Description]
FROM [dbo].[FPJenisTransaksi]
WHERE [FCode] = @FCode");
            sp.AddParameter("FCode", fCode);
            return GetApplicationCollection(sp);
        }
    }
}
