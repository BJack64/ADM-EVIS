using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class FPJenisDokumens : ApplicationCollection<FPJenisDokumen, SpBase>
    {
        public static List<FPJenisDokumen> Get()
        {
            var sp = new SpBase(@"SELECT [Id]
      ,[Description]
FROM [dbo].[FPJenisDokumen]");
            return GetApplicationCollection(sp);
        }

        public static FPJenisDokumen GetById(string id)
        {
            var sp = new SpBase(@"SELECT [Id]
      ,[Description]
FROM [dbo].[FPJenisDokumen]
WHERE [Id] = @Id");
            sp.AddParameter("Id", id);
            var dbData = GetApplicationObject(sp);
            return dbData == null || string.IsNullOrEmpty(dbData.Id) ? null : dbData;
        }

        public static List<FPJenisDokumen> GetByFCodeAndJnsTransaksi(string fcode, string jenistransaksi)
        {
            var sp = new SpBase(@"SELECT jdoc.[Id]
      ,[Description]
FROM [dbo].[FPJenisDokumen] jdoc INNER JOIN
		dbo.MapJnsTransJnsDok map ON jdoc.Id = map.JenisDokumen
WHERE map.FCode = @FCode AND map.JenisTransaksi = @jenisTransaksi");
            sp.AddParameter("jenisTransaksi", jenistransaksi);
            sp.AddParameter("FCode", fcode);
            return GetApplicationCollection(sp);
        }

        public static List<FPJenisDokumen> GetByFCode(string fCode)
        {
            var sp = new SpBase(@"SELECT jdoc.[Id]
      ,[Description]
FROM [dbo].[FPJenisDokumen] jdoc INNER JOIN
		dbo.MapJnsTransJnsDok map ON jdoc.Id = map.JenisDokumen
WHERE map.FCode = @FCode");
            
            sp.AddParameter("FCode", fCode);

            return GetApplicationCollection(sp);

        }

    }
}
