using System.Collections.Generic;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class ReportSuratPemberitahuanMasas : ApplicationCollection<ReportSuratPemberitahuanMasa, SpBase>
    {

        public static List<ReportSuratPemberitahuanMasa> Get()
        {
            var sp = new SpBase(@"SELECT [Id]
      ,[MasaPajak]
      ,m.[MonthName] AS NamaMasaPajak
      ,[TahunPajak]
      ,[Versi]
      ,[IsDeleted]
      ,[Created]
      ,[Modified]
      ,[CreatedBy]
      ,[ModifiedBy]
FROM [dbo].[ReportSPM] INNER JOIN
		dbo.GetMonth() m ON MasaPajak = m.MonthNumber
WHERE	IsDeleted = 0");
            return GetApplicationCollection(sp);
        }

        public static ReportSuratPemberitahuanMasa GetLastDataByMasaPajak(int masaPajak, int tahunPajak)
        {
            var sp = new SpBase(@"SELECT TOP 1 [Id]
      ,[MasaPajak]
      ,m.[MonthName] AS NamaMasaPajak
      ,[TahunPajak]
      ,[Versi]
      ,[IsDeleted]
      ,[Created]
      ,[Modified]
      ,[CreatedBy]
      ,[ModifiedBy]
FROM [dbo].[ReportSPM] INNER JOIN
		dbo.GetMonth() m ON MasaPajak = m.MonthNumber
WHERE	IsDeleted = 0 AND [MasaPajak] = @MasaPajak AND [TahunPajak] = @TahunPajak
ORDER BY [Versi] DESC");

            sp.AddParameter("MasaPajak", masaPajak);
            sp.AddParameter("TahunPajak", tahunPajak);

            var dbData = GetApplicationObject(sp);

            return dbData == null || dbData.MasaPajak != masaPajak || dbData.TahunPajak != tahunPajak ? null : dbData;

        }

        public static List<ReportSuratPemberitahuanMasa> GetByMasaPajak(int masaPajak, int tahunPajak)
        {
            var sp = new SpBase(@"SELECT [Id]
      ,[MasaPajak]
      ,m.[MonthName] AS NamaMasaPajak
      ,[TahunPajak]
      ,[Versi]
      ,[IsDeleted]
      ,[Created]
      ,[Modified]
      ,[CreatedBy]
      ,[ModifiedBy]
FROM [dbo].[ReportSPM] INNER JOIN
		dbo.GetMonth() m ON MasaPajak = m.MonthNumber
WHERE	IsDeleted = 0 AND [MasaPajak] = @MasaPajak AND [TahunPajak] = @TahunPajak
ORDER BY [Versi] ASC");

            sp.AddParameter("MasaPajak", masaPajak);
            sp.AddParameter("TahunPajak", tahunPajak);


            return GetApplicationCollection(sp);

        }

        public static void GenerateCreateSpm(int masaPajak, int tahunPajak, string userNameLogin, out long reportSpmId)
        {
            reportSpmId = 0;
            var sp = new SpBase(@"EXEC sp_ReportSPMGenerateInsert @masaPajak = @masaPajakParam,
		@tahunPajak = @tahunPajakParam,
		@userNameLogin = @userNameLoginParam,
		@reportSpmId = @reportSpmIdParam OUTPUT");
            sp.AddParameter("masaPajakParam", masaPajak);
            sp.AddParameter("tahunPajakParam", tahunPajak);
            sp.AddParameter("userNameLoginParam", userNameLogin);
            sp.AddParameter("reportSpmIdParam", reportSpmId, ParameterDirection.Output);

            sp.ExecuteNonQuery();

            reportSpmId = (long)sp.GetParameter("reportSpmIdParam");

        }

        public static ReportSuratPemberitahuanMasa GetById(long id)
        {
            var sp = new SpBase(@"SELECT [Id]
      ,[MasaPajak]
      ,m.[MonthName] AS NamaMasaPajak
      ,[TahunPajak]
      ,[Versi]
      ,[IsDeleted]
      ,[Created]
      ,[Modified]
      ,[CreatedBy]
      ,[ModifiedBy]
FROM [dbo].[ReportSPM] INNER JOIN
		dbo.GetMonth() m ON MasaPajak = m.MonthNumber
WHERE	IsDeleted = 0 AND [Id] = @Id");

            sp.AddParameter("Id", id);

            var dbData = GetApplicationObject(sp);

            return dbData == null || dbData.Id == id ? null : dbData;
        }

        public static ReportSuratPemberitahuanMasa GetSpecificSpm(int masaPajak, int tahunPajak, int versi)
        {
            var sp = new SpBase(@"SELECT [Id]
      ,[MasaPajak]
      ,m.[MonthName] AS NamaMasaPajak
      ,[TahunPajak]
      ,[Versi]
      ,[IsDeleted]
      ,[Created]
      ,[Modified]
      ,[CreatedBy]
      ,[ModifiedBy]
FROM [dbo].[ReportSPM] INNER JOIN
		dbo.GetMonth() m ON MasaPajak = m.MonthNumber
WHERE	IsDeleted = 0 AND [MasaPajak] = @MasaPajak AND [TahunPajak] = @TahunPajak AND [Versi] = @Versi");

            sp.AddParameter("MasaPajak", masaPajak);
            sp.AddParameter("TahunPajak", tahunPajak);
            sp.AddParameter("Versi", versi);

            var dbData = GetApplicationObject(sp);

            return dbData == null || dbData.MasaPajak != masaPajak ? null : dbData;
        }

        public static List<ReportSuratPemberitahuanMasa> GetForDownloadExcel(int masaPajak, int tahunPajak, int versi)
        {
            var sp = new SpBase(@"SELECT [Id]
      ,[MasaPajak]
      ,m.[MonthName] AS NamaMasaPajak
      ,[TahunPajak]
      ,[Versi]
      ,[IsDeleted]
      ,[Created]
      ,[Modified]
      ,[CreatedBy]
      ,[ModifiedBy]
FROM [dbo].[ReportSPM] INNER JOIN
		dbo.GetMonth() m ON MasaPajak = m.MonthNumber
WHERE	IsDeleted = 0 AND [MasaPajak] = @MasaPajak AND [TahunPajak] = @TahunPajak AND [Versi] <= @Versi");

            sp.AddParameter("MasaPajak", masaPajak);
            sp.AddParameter("TahunPajak", tahunPajak);
            sp.AddParameter("Versi", versi);

            return GetApplicationCollection(sp);

        }

    }
}
 