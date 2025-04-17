using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class FakturPajakPenampungDetails : ApplicationCollection<FakturPajakPenampungDetail, SpBase>
    {

        public static List<FakturPajakPenampungDetail> GetByFakturPajakId(long fakturPajakPenampungId)
        {
            var sp = new SpBase(@"SELECT [FakturPajakPenampungDetailId]
      ,[FakturPajakPenampungId]
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
	  ,COUNT(FakturPajakPenampungDetailId) OVER() AS TotalItems
  FROM [dbo].[FakturPajakPenampungDetail]
WHERE [FakturPajakPenampungId] = @FakturPajakPenampungId AND [IsDeleted] = 0");

            sp.AddParameter("FakturPajakPenampungId", fakturPajakPenampungId);

            return GetApplicationCollection(sp);

        }

        public static bool BulkInsert(List<FakturPajakPenampungDetail> dats)
        {
            if (dats == null || dats.Count <= 0) return true;
            foreach (var fakturPajakPenampungDetail in dats)
            {
                InsertToDb(fakturPajakPenampungDetail);
            }
            return true;
        }

        public static bool Insert(FakturPajakPenampungDetail dat)
        {
            InsertToDb(dat);
            return true;
        }

        private static void InsertToDb(FakturPajakPenampungDetail dat)
        {
            var sp =
                new SpBase(
                    @"INSERT INTO [dbo].[FakturPajakPenampungDetail]([FakturPajakPenampungId],[Nama],[HargaSatuan],[JumlahBarang],[HargaTotal],[Diskon],[Dpp],[Ppn],[TarifPpnbm],[Ppnbm],[CreatedBy])
VALUES(@FakturPajakPenampungId,@Nama,@HargaSatuan,@JumlahBarang,@HargaTotal,@Diskon,@Dpp,@Ppn,@TarifPpnbm,@Ppnbm,@CreatedBy)");

            sp.AddParameter("FakturPajakPenampungId", dat.FakturPajakPenampungId);
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

        public static bool DeleteByFakturPajakId(long fakturPajakPenampungId, string usermodify)
        {

            var sp = new SpBase(@"UPDATE [dbo].[FakturPajakPenampungDetail]
   SET [IsDeleted] = 1
      ,[Modified] = GETDATE()
      ,[ModifiedBy] = @ModifiedBy
 WHERE [FakturPajakPenampungId] = @FakturPajakPenampungId AND IsDeleted = 0");
            sp.AddParameter("ModifiedBy", usermodify);
            sp.AddParameter("FakturPajakPenampungId", fakturPajakPenampungId);

            return sp.ExecuteNonQuery() >= 0;

        }

    }
}
