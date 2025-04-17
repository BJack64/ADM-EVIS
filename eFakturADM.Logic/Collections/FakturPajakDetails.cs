using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class FakturPajakDetails : ApplicationCollection<FakturPajakDetail, SpBase>
    {

        public static List<FakturPajakDetail> GetByFakturPajakId(long fakturPajakId)
        {
            var sp = new SpBase(@"SELECT [FakturPajakDetailId]
      ,[FakturPajakId]
      ,[Nama]
      ,[HargaSatuan]
      ,[JumlahBarang]
      ,[HargaTotal]
      ,[Diskon]
      ,[Dpp]
      ,[Ppn]
      ,[TarifPpnbm]
      ,[Ppnbm]
      ,[IsDeleted]
      ,[Created]
      ,[Modified]
      ,[CreatedBy]
      ,[ModifiedBy]
	  ,COUNT(FakturPajakDetailId) OVER() AS TotalItems
  FROM [dbo].[FakturPajakDetail]
WHERE [FakturPajakId] = @FakturPajakId AND [IsDeleted] = 0");

            sp.AddParameter("FakturPajakId", fakturPajakId);

            return GetApplicationCollection(sp);

        }

        public static bool BulkInsert(List<FakturPajakDetail> dats)
        {
            if (dats == null || dats.Count <= 0) return true;
            foreach (var fakturPajakDetail in dats)
            {
                InsertToDb(fakturPajakDetail);
            }
            return true;
        }

        public static bool Insert(FakturPajakDetail dat)
        {
            InsertToDb(dat);
            return true;
        }

        private static void InsertToDb(FakturPajakDetail dat)
        {
            var sp =
                new SpBase(System.Configuration.ConfigurationManager.AppSettings["eFakturADM.Connection.String"],
                    @"INSERT INTO [dbo].[FakturPajakDetail]([FakturPajakId],[Nama],[HargaSatuan],[JumlahBarang],[HargaTotal],[Diskon],[Dpp],[Ppn],[TarifPpnbm],[Ppnbm],[CreatedBy])
VALUES(@FakturPajakId,@Nama,@HargaSatuan,@JumlahBarang,@HargaTotal,@Diskon,@Dpp,@Ppn,@TarifPpnbm,@Ppnbm,@CreatedBy)");

            sp.AddParameter("FakturPajakId", dat.FakturPajakId);
            sp.AddParameter("Nama", dat.Nama);
            sp.AddParameter("HargaSatuan", dat.HargaSatuan);
            sp.AddParameter("JumlahBarang", dat.JumlahBarang);
            sp.AddParameter("HargaTotal", dat.HargaTotal);
            sp.AddParameter("Diskon", dat.Diskon);
            sp.AddParameter("Dpp", dat.Dpp);
            sp.AddParameter("Ppn", dat.Ppn);
            sp.AddParameter("TarifPpnbm", dat.TarifPpnbm);
            sp.AddParameter("Ppnbm", dat.Ppnbm);
            sp.AddParameter("CreatedBy", dat.CreatedBy);
            sp.ExecuteNonQuery();

        }

        public static bool DeleteByFakturPajakId(long fakturPajakId, string usermodify)
        {

            var sp = new SpBase(@"UPDATE [dbo].[FakturPajakDetail] WITH (ROWLOCK, UPDLOCK)
             SET [IsDeleted] = 1
                  ,[Modified] = GETDATE()
                  ,[ModifiedBy] = @ModifiedBy
             WHERE [FakturPajakId] = @FakturPajakId AND IsDeleted = 0");
            sp.AddParameter("ModifiedBy", usermodify);
            sp.AddParameter("FakturPajakId", fakturPajakId);

            return sp.ExecuteNonQuery() >= 0;

        }

    }
}
