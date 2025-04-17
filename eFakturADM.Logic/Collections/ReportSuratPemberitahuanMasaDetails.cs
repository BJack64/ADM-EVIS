using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class ReportSuratPemberitahuanMasaDetails : ApplicationCollection<ReportSuratPemberitahuanMasaDetail, SpBase>
    {
        public static List<ReportSuratPemberitahuanMasaDetail> GetGenerateSubmitSearch(int masaPajak, int tahunPajak)
        {
            var sp = new SpBase(@"EXEC sp_ReportSPMGet @masaPajak, @tahunPajak");
            sp.AddParameter("masaPajak", masaPajak);
            sp.AddParameter("tahunPajak", tahunPajak);

            return GetApplicationCollection(sp);

        }

        public static List<ReportSuratPemberitahuanMasaDetail> GetBySpmId(long spmId)
        {
            var sp = new SpBase(@"EXEC sp_ReportSPMGetById @Id = @IdParam");
            sp.AddParameter("IdParam", spmId);

            return GetApplicationCollection(sp);
        }

        public static ReportSuratPemberitahuanMasaDetail GetByFormatedNoFaktur(string formatedNoFaktur)
        {
            var sp = new SpBase(@"EXEC sp_ReportSPMGetByFormatedNoFaktur @formatedNoFaktur = @formatedNoFakturParam");
            sp.AddParameter("formatedNoFakturParam", formatedNoFaktur);

            var dbData = GetApplicationObject(sp);
            return dbData == null || dbData.Id == 0 ? null : dbData;

        }

        public static List<ReportSuratPemberitahuanMasaDetail> GetByMasaTahunPajak(int masaPajak, int tahunPajak)
        {
            var sp =
                new SpBase(
                    @"EXEC sp_ReportSPMGetByMasaAndTahunPajak @masaPajak = @masaPajakParam, @tahunPajak = @tahunPajakParam");
            sp.AddParameter("masaPajakParam", masaPajak);
            sp.AddParameter("tahunPajakParam", tahunPajak);
            return GetApplicationCollection(sp);
        }

        public static List<ReportSuratPemberitahuanMasaDetail> GetForDownloadExcel(int masaPajak, int tahunPajak, int versi)
        {
            var sp =
                new SpBase(
                    @"EXEC sp_ReportSPMGetForExcel @masaPajak = @masaPajakParam, @tahunPajak = @tahunPajakParam, @versi = @versiParam");
            sp.AddParameter("masaPajakParam", masaPajak);
            sp.AddParameter("tahunPajakParam", tahunPajak);
            sp.AddParameter("versiParam", versi);
            return GetApplicationCollection(sp);
        }
        
    }
}
