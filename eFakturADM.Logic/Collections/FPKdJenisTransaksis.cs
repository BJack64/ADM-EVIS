using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class FPKdJenisTransaksis : ApplicationCollection<FPKdJenisTransaksi, SpBase>
    {
        public static List<FPKdJenisTransaksi> Get()
        {
            var sp = new SpBase(@"SELECT [Id]
      ,[Description]
FROM [dbo].[FPKdJenisTransaksi]");
            return GetApplicationCollection(sp);
        }

        public static FPKdJenisTransaksi GetById(string id)
        {
            var sp = new SpBase(@"SELECT [Id]
      ,[Description]
FROM [dbo].[FPKdJenisTransaksi]
WHERE [Id] = @Id");
            sp.AddParameter("Id", id);
            var dbData = GetApplicationObject(sp);
            return dbData == null || string.IsNullOrEmpty(dbData.Id) ? null : dbData;
        }

        public static List<FPKdJenisTransaksi> GetByFCode(string fCode)
        {
            var sp = new SpBase(@"SELECT DISTINCT t.[Id]
      ,[Description]
FROM [dbo].[FPKdJenisTransaksi] t inner join
		dbo.MapJnsTransaksiKdJnsTransaksi map ON t.Id = map.KdJenisTransaksi
WHERE map.FCode = @FCode");

            sp.AddParameter("FCode", fCode);

            return GetApplicationCollection(sp);
        }

        public static List<FPKdJenisTransaksi> GetByFCodeAndJnsTransaksi(string fCode, string jenisTransaksi)
        {
            var sp = new SpBase(@"SELECT DISTINCT t.[Id]
      ,[Description]
FROM [dbo].[FPKdJenisTransaksi] t inner join
		dbo.MapJnsTransaksiKdJnsTransaksi map ON t.Id = map.KdJenisTransaksi
WHERE map.FCode = @FCode AND map.JenisTransaksi = @jenisTransaksi");

            sp.AddParameter("FCode", fCode);
            sp.AddParameter("jenisTransaksi", jenisTransaksi);

            return GetApplicationCollection(sp);
        }

    }
}
